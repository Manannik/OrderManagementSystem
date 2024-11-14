using System.Text.Json;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Exceptions;

namespace Order.Infrastructure.Services
{
    public class CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> _logger)
        : ICatalogServiceClient
    {
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<ProductItem> ChangeProductQuantityAsync(Guid id, int newQuantity, CancellationToken ct)
        {
            _logger.LogInformation("Запуск метода ChangeProductQuantityAsync для продукта с ID: {Id}", id);

            using (var response = await httpClient.PutAsync($"ChangeQuantity/{id}/{newQuantity}",
                       null, ct))
            {
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(ct);
                    var productItem =
                        await JsonSerializer.DeserializeAsync<ProductItem>(stream, jsonSerializerOptions, ct);
                    _logger.LogInformation(
                        "Успешное завершение метода ChangeProductQuantityAsync, получен продукт: {@ProductItem}",
                        productItem);
                    return productItem;
                }

                var errorContent = await response.Content.ReadAsStringAsync(ct);
                var errorDetails =
                    JsonSerializer.Deserialize<CatalogServiceException>(errorContent, jsonSerializerOptions);
                _logger.LogWarning(
                    "Ошибка при выполнении метода ChangeProductQuantityAsync для продукта с ID: {Id}. Ошибка: {Error}. Код ошибки:{Code}",
                    id, errorDetails?.Message, errorDetails.StatusCode);
                throw new CatalogServiceException(errorDetails.Id, errorDetails.Message, errorDetails.StatusCode);
            }
        }
    }
}