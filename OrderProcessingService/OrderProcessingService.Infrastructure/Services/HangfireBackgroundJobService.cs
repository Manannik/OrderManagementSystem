using Hangfire;
using System.Linq.Expressions;
using OrderProcessingService.Application.Abstractions;

namespace OrderProcessingService.Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    public void Enqueue(Expression<Func<Task>> methodCall)
    {
        BackgroundJob.Enqueue(methodCall);
    }
}