using System.Linq.Expressions;

namespace OrderProcessingService.Application.Abstractions;

public interface IBackgroundJobService
{
    void Enqueue(Expression<Func<Task>> methodCall);
}