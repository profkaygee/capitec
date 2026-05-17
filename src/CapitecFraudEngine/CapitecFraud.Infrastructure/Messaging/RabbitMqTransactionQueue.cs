using System.Text;
using System.Text.Json;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using RabbitMQ.Client;

namespace CapitecFraud.Infrastructure.Messaging;

public class RabbitMqTransactionQueue : ITransactionQueue
{
    private readonly IConnection _connection;

    public RabbitMqTransactionQueue(IConnection connection)
    {
        _connection = connection;
    }

    public Task PublishAsync(TransactionMessage message)
    {
        using var channel = _connection.CreateModel();

        channel.QueueDeclare(
            queue: "fraud.transactions",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: "fraud.transactions",
            body: body);

        return Task.CompletedTask;
    }
}