using System.Text.Json;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Domain.Entities;

namespace Order.Infrastructure.Services
{
    public class CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> _logger) : ICatalogServiceClient
    {
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<bool> ChangeProductQuantityAsync(Guid id, int newQuantity, decimal price, CancellationToken ct)
        {
            _logger.LogInformation("Запуск метода ChangeProductQuantityAsync для продукта с ID: {Id}", id);

            using (var response = await httpClient.PutAsync($"ChangeQuantity/{id}/{newQuantity}",
                       null, ct))
            {
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(ct);
                    var productItem = await JsonSerializer.DeserializeAsync<ProductItem>(stream, jsonSerializerOptions, ct);
                    _logger.LogInformation(
                        "Успешное завершение метода ChangeProductQuantityAsync, получен продукт: {@ProductItem}",
                        productItem);
                    return true;
                }

                _logger.LogInformation(
                    "Ошибка при запуске метода (нет указанного товара или недостаточно количество товара)" +
                    " ChangeProductQuantityAsync для продукта с ID: {Id}", id);
                return false;
            }
        }

        public async Task<bool> ProductExistsAsync(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Запуск метода ProductExistsAsync для продукта с ID: {Id}", id);
            using (var response = await httpClient.GetAsync($"{id}",
                       HttpCompletionOption.ResponseContentRead,
                       ct))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(ct);
                    var productItem = await JsonSerializer.DeserializeAsync<ProductItem>(stream, jsonSerializerOptions, ct);
                    _logger.LogInformation("Успешное завершение метода ProductExistsAsync, получен продукт: {@ProductItem}",
                        productItem);
                    return productItem != null;
                }

                _logger.LogWarning("Продукт с ID: {Id} не найден. Статус ответа: {StatusCode}", id, response.StatusCode);
                return false;
            }
        }
    }
}