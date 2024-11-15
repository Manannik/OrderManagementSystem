﻿using Confluent.Kafka;

namespace Order.Application.Abstractions
{
    public interface IKafkaProducer<in TMessage> : IDisposable
    {
        Task ProduceAsync(TMessage message, CancellationToken cancellationToken);
    }
}