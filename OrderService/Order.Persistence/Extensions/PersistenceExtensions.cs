﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Abstractions;
using Order.Persistence.Services;

namespace Order.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddDbContext<OrderDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OrderServiceConnectionString"));
        });
        return services;
    }
}