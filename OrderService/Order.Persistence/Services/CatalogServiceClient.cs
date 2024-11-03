using System.Text.Json;
using Microsoft.Extensions.Logging;
using Order.Domain.Abstractions;
using Order.Domain.Entities;

namespace Order.Persistence.Services;

public class CatalogServiceClient(HttpClient httpClient) : ICatalogServiceClient
{
    private readonly ILogger<CatalogServiceClient> _logger;
    
    private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<ProductItem> ChangeProductQuantityAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Запуск метода ChangeProductQuantityAsync для продукта с ID: {Id}", id);
        
        using (var response = await httpClient.GetAsync($"{id}",
                   HttpCompletionOption.ResponseContentRead,
                   ct))
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync(ct);
            var productItem = await JsonSerializer.DeserializeAsync<ProductItem>(stream, jsonSerializerOptions,ct);
            _logger.LogInformation("Успешное завершение метода ChangeProductQuantityAsync, получен продукт: {@ProductItem}", productItem);
            return productItem;
        }
    }

    public async Task<bool> ProductExistsAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Запуск метода ChangeProductQuantityAsync для продукта с ID: {Id}", id);
        using (var response = await httpClient.GetAsync($"{id}",
                   HttpCompletionOption.ResponseContentRead,
                   ct))
        {
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync(ct);
                var productItem = await JsonSerializer.DeserializeAsync<ProductItem>(stream, jsonSerializerOptions,ct);
                _logger.LogInformation("Успешное завершение метода ChangeProductQuantityAsync, получен продукт: {@ProductItem}", productItem);
                return productItem != null;
            }
            _logger.LogWarning("Продукт с ID: {Id} не найден. Статус ответа: {StatusCode}", id, response.StatusCode);
            return false;
        }
    }
}