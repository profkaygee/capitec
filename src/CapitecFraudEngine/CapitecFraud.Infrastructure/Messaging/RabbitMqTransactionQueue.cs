using System.Text;
using System.Text.Json;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CapitecFraud.Infrastructure.Messaging;

public class RabbitMqTransactionQueue(IConnection connection, ILogger<RabbitMqTransactionQueue> logger) : ITransactionQueue
{
    public Task<bool> PublishAsync(TransactionMessage message)
    {
        var channel = connection.CreateModel();

        channel.QueueDeclare("transactions", true, false, false);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        channel.BasicPublish(
            exchange: "",
            routingKey: "transactions",
            body: body);

        return Task.FromResult(true);
    }

    public Task ConsumeAsync(Func<TransactionMessage, Task> handler)
    {
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "transactions",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<TransactionMessage>(json);

                if (message is null)
                {
                    return;
                }

                await handler(message);
                channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (JsonException jsonException)
            {
                channel.BasicNack(args.DeliveryTag, false, true);
                logger.LogError(jsonException, "Error parsing JSON message");
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                logger.LogError(ex, "Error processing transaction message");
            }
        };

        channel.BasicConsume(
            queue: "transactions",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }
}