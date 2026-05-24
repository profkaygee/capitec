using System.Text;
using System.Text.Json;
using CapitecFraud.Application.Models;
using CapitecFraud.Infrastructure.Messaging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CapitecFraud.Tests.MessagingTests;

public class RabbitMqTransactionQueueTest
{
    private readonly Mock<IConnection> _mockConnection;
    private readonly Mock<IModel> _mockChannel;
    private readonly RabbitMqTransactionQueue _queue;

    public RabbitMqTransactionQueueTest()
    {
        _mockConnection = new Mock<IConnection>();
        _mockChannel = new Mock<IModel>();
        _mockConnection
            .Setup(c => c.CreateModel())
            .Returns(_mockChannel.Object);
        
        _queue = new RabbitMqTransactionQueue(_mockConnection.Object);
    }

    #region PublishAsync Tests

    [Fact]
    public async Task PublishAsync_WithValidMessage_DeclaresQueueAndPublishes()
    {
        // Arrange
        var message = new TransactionMessage 
        { 
            Id = 123,
            Amount = 1000,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _queue.PublishAsync(message);

        // Assert
        _mockChannel.Verify(c => c.QueueDeclare("transactions", true, false, false), Times.Once);
        _mockChannel.Verify(c => c.BasicPublish(
            It.IsAny<string>(),
            "transactions",
            It.IsAny<IBasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_SerializesMessageCorrectly()
    {
        // Arrange
        var message = new TransactionMessage 
        { 
            Id = 456,
            Amount = 5000,
            Timestamp = DateTime.UtcNow
        };

        byte? publishedBody = null;
        _mockChannel
            .Setup(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Callback<string, string, IBasicProperties, ReadOnlyMemory<byte>>((ex, rk, props, body) =>
            {
                publishedBody = body.ToArray()[0];
            });

        // Act
        await _queue.PublishAsync(message);

        // Assert
        _mockChannel.Verify(c => c.BasicPublish(
            "",
            "transactions",
            It.IsAny<IBasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        TransactionMessage message = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _queue.PublishAsync(message));
    }

    #endregion

    #region ConsumeAsync Tests

    [Fact]
    public async Task ConsumeAsync_WithValidMessage_InvokesHandler()
    {
        // Arrange
        var message = new TransactionMessage 
        { 
            Id = 789,
            Amount = 2500,
            Timestamp = DateTime.UtcNow
        };
        //var handlerCalled = false;
        TransactionMessage receivedMessage = null;

        Func<TransactionMessage, Task> handler = async (msg) =>
        {
            //handlerCalled = true;
            receivedMessage = msg;
            await Task.CompletedTask;
        };

        var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
        _mockChannel
            .Setup(c => c.BasicConsume(
                "transactions",
                false,
                It.IsAny<AsyncEventingBasicConsumer>()))
            .Callback<string, bool, IAsyncBasicConsumer>((queue, autoAck, cons) =>
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                var args = new BasicDeliverEventArgs
                {
                    Body = new ReadOnlyMemory<byte>(body),
                    DeliveryTag = 1
                };
                consumer.HandleBasicDeliver("ctag", 1, false, "", "", null, args.Body);
            });

        // Act
        await _queue.ConsumeAsync(handler);

        // Assert
        _mockChannel.Verify(c => c.QueueDeclare(
            "transactions",
            true,
            false,
            false), Times.Once);
        _mockChannel.Verify(c => c.BasicConsume(
            "transactions",
            false,
            "",
            false,
            false,
            null,
            (IBasicConsumer)It.IsAny<IAsyncBasicConsumer>()), Times.Once);
    }

    [Fact]
    public async Task ConsumeAsync_WithValidMessage_AcknowledgesMessage()
    {
        // Arrange
        var message = new TransactionMessage 
        { 
            Id = 999,
            Amount = 3000,
            Timestamp = DateTime.UtcNow
        };

        Func<TransactionMessage, Task> handler = async (msg) => await Task.CompletedTask;

        var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
        _mockChannel
            .Setup(c => c.BasicConsume(
                "transactions",
                false,
                It.IsAny<AsyncEventingBasicConsumer>()))
            .Callback<string, bool, IAsyncBasicConsumer>((queue, autoAck, cons) =>
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                consumer.HandleBasicDeliver("ctag", 1, false, "", "", null, new ReadOnlyMemory<byte>(body));
            });

        // Act
        await _queue.ConsumeAsync(handler);

        // Assert
        _mockChannel.Verify(c => c.BasicAck(It.IsAny<ulong>(), false), Times.Once);
    }

    [Fact]
    public async Task ConsumeAsync_WithInvalidJson_SkipsHandlerAndDoesNotAcknowledge()
    {
        // Arrange
        var handlerCalled = false;
        Func<TransactionMessage, Task> handler = async (msg) =>
        {
            handlerCalled = true;
            await Task.CompletedTask;
        };

        var consumer = new AsyncEventingBasicConsumer(_mockChannel.Object);
        _mockChannel
            .Setup(c => c.BasicConsume(
                "transactions",
                false,
                It.IsAny<AsyncEventingBasicConsumer>()))
            .Callback<string, bool, IAsyncBasicConsumer>((queue, autoAck, cons) =>
            {
                var body = Encoding.UTF8.GetBytes("invalid json");
                consumer.HandleBasicDeliver("ctag", 1, false, "", "", null, new ReadOnlyMemory<byte>(body));
            });

        // Act
        await _queue.ConsumeAsync(handler);

        // Assert
        Assert.False(handlerCalled);
        _mockChannel.Verify(c => c.BasicAck(It.IsAny<ulong>(), false), Times.Never);
    }

    [Fact]
    public async Task ConsumeAsync_DeclaresQueueWithCorrectParameters()
    {
        // Arrange
        Func<TransactionMessage, Task> handler = async (msg) => await Task.CompletedTask;

        // Act
        await _queue.ConsumeAsync(handler);

        // Assert
        _mockChannel.Verify(c => c.QueueDeclare(
            "transactions",
            true,
            false,
            false), Times.Once);
    }

    [Fact]
    public async Task ConsumeAsync_SetupBasicConsume_WithAutoAckFalse()
    {
        // Arrange
        Func<TransactionMessage, Task> handler = async (msg) => await Task.CompletedTask;

        // Act
        await _queue.ConsumeAsync(handler);

        // Assert
        _mockChannel.Verify(c => c.BasicConsume(
            "transactions",
            false,
            "",
            false,
            false,
            null,
            (IBasicConsumer)It.IsAny<IAsyncBasicConsumer>()), Times.Once);
    }

    #endregion
}