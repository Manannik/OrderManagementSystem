using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Order.Application.Abstractions;
using Order.Application.Enums;
using Order.Application.Helpers;
using Order.Application.Models;
using Order.Application.Models.Kafka;
using Order.Domain.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Exceptions;

namespace OrderService.UnitTests;

public class OrderServiceTests
{
    private Mock<IOrderRepository> _mockOrderRepository;
    private IOrderService _orderService;
    private Mock<IQuantityService> _mockQuantityService;
    private Mock<ICatalogServiceClient> _catalogServiceClient;
    private Mock<IKafkaProducer<CreateOrderKafkaModel>> _mockCreateOrderProducer;
    private Mock<IKafkaProducer<UpdatedOrderKafkaModel>> _mockUpdatedOrderProducer;
    private Mock<ILogger<Order.Application.Services.OrderService>> _mockLogger;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockQuantityService = new Mock<IQuantityService>();
        _mockCreateOrderProducer = new Mock<IKafkaProducer<CreateOrderKafkaModel>>();
        _mockUpdatedOrderProducer = new Mock<IKafkaProducer<UpdatedOrderKafkaModel>>();
        _mockLogger = new Mock<ILogger<Order.Application.Services.OrderService>>();
        
        _orderService = new Order.Application.Services.OrderService(
            _mockLogger.Object,
            _mockOrderRepository.Object,
            _mockCreateOrderProducer.Object,
            _mockUpdatedOrderProducer.Object,
            _mockQuantityService.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ReturnsOrderModelResponse_WhenRequestIsValid()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var request = fixture.Create<CreateOrderRequest>();

        fixture.Customize<Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>>(
            f => f.FromFactory(
                (List<ProductItem> successValue) =>
                    Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Success(successValue)
            )
        );
        var successResult = fixture.Create<Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>>();

        _mockQuantityService.Setup(service => service.TryChangeQuantityAsync(It.IsAny<List<ProductItemModel>>(),
                CancellationToken.None))
            .ReturnsAsync(successResult)
            .Verifiable();

        var productItems = successResult.Value;
        var newOrder = fixture.Build<Order.Domain.Entities.Order>()
            .With(f => f.ProductItems, productItems)
            .Create();
        newOrder.CalculateCost(productItems);

        _mockOrderRepository.Setup(service => service.CreateAsync(It.IsAny<List<ProductItem>>(),
                CancellationToken.None))
            .ReturnsAsync(newOrder);
        
        var orderKafkaModel = fixture.Build<CreateOrderKafkaModel>()
            .With(f => f.Id, newOrder.Id)
            .With(f => f.OrderStatus, newOrder.OrderStatus.ToString())
            .With(f => f.Cost, newOrder.Cost)
            .With(f => f.CreatedAt, DateTime.UtcNow)
            .Create();

        _mockCreateOrderProducer
            .Setup(k => k.ProduceAsync(orderKafkaModel, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var newOrderResponse = fixture.Build<OrderModelResponse>()
            .With(f => f.Id, newOrder.Id)
            .With(f => f.OrderStatus, (OrderStatusModel)newOrder.OrderStatus)
            .With(f => f.Cost, newOrder.Cost)
            .Create();
        
        var result = await _orderService.CreateAsync(request, CancellationToken.None);
        
        _mockQuantityService.Verify(service=>service.TryChangeQuantityAsync(It.IsAny<List<ProductItemModel>>(),
            It.IsAny<CancellationToken>()),Times.Once);
        
        _mockOrderRepository.Verify(service=>service.CreateAsync(It.IsAny<List<ProductItem>>(),
            CancellationToken.None),Times.Once);
        
        _mockCreateOrderProducer.Verify(k => k.ProduceAsync(It.IsAny<CreateOrderKafkaModel>(), It.IsAny<CancellationToken>()), Times.Once);
        
        Assert.Equal(newOrderResponse.Id, result.Id);
        Assert.Equal(newOrderResponse.OrderStatus, result.OrderStatus);
        Assert.Equal(newOrderResponse.Cost, result.Cost);
    }

    [Fact]
    public async Task CreateAsync_ThrowsCatalogServiceException_WhenTryChangeQuantityAsyncFails()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var request = fixture.Create<CreateOrderRequest>();
        
        var errors = new List<(Guid id, string Message, int StatusCode)>
        {
            (Guid.NewGuid(), "Error 1", 400),
            (Guid.NewGuid(), "Error 2", 404)
        };

        var failureResult = Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Failure(errors);
        
        _mockQuantityService
            .Setup(s => s.TryChangeQuantityAsync(It.IsAny<List<ProductItemModel>>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);
        
        var exception = await Assert.ThrowsAsync<AggregateException>(() =>
            _orderService.CreateAsync(request, CancellationToken.None));
        
        Assert.Equal(2, exception.InnerExceptions.Count);
        var catalogException = exception.InnerExceptions.First() as CatalogServiceException;
        Assert.NotNull(catalogException);
        Assert.Equal(errors[0].id, catalogException!.Id);
        Assert.Equal(errors[0].Message, catalogException.Message);
        Assert.Equal(errors[0].StatusCode, catalogException.StatusCode);
    }
    
    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenRequestIsInvalid()
    {
        var invalidRequest = new CreateOrderRequest();
        
        await Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.CreateAsync(invalidRequest, CancellationToken.None));
    }

}