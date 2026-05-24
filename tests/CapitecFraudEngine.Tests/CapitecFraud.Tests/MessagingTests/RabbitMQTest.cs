using System.Text;
using System.Text.Json;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Messaging;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CapitecFraud.Tests.MessagingTests;

public class RabbitMqTransactionQueueTest
{
    private readonly Mock<IModel> _mockChannel;
    private readonly RabbitMqTransactionQueue _queue;

    public RabbitMqTransactionQueueTest()
    {
        var mockConnection = new Mock<IConnection>();
        _mockChannel = new Mock<IModel>();
        mockConnection
            .Setup(c => c.CreateModel())
            .Returns(_mockChannel.Object);
        
        _queue = new RabbitMqTransactionQueue(mockConnection.Object);
    }

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
        _mockChannel.Verify(c => c.QueueDeclare(
                "transactions",
                true,
                false,
                false,
                It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        _mockChannel.Verify(c => c.BasicPublish(
                "",
                "transactions",
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);
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

        string publishedJson = null;

        _mockChannel
            .Setup(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>(
                (ex, rk, mandatory, props, body) =>
                {
                    publishedJson = Encoding.UTF8.GetString(body.ToArray());
                });

        // Act
        await _queue.PublishAsync(message);

        // Assert
        publishedJson.Should().NotBeNull();

        var deserialized = JsonSerializer.Deserialize<TransactionMessage>(publishedJson!);

        deserialized!.Id.Should().Be(456);
        deserialized.Amount.Should().Be(5000);
    }

    [Fact]
    public async Task PublishAsync_WithNullMessage_Pass()
    {
        // Arrange
        TransactionMessage message = null;

        // Act & Assert
        var result = await _queue.PublishAsync(message);
        Assert.True(result);
    }

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

        TransactionMessage received = null;

        Func<TransactionMessage, Task> handler = async (msg) =>
        {
            received = msg;
            await Task.CompletedTask;
        };

        AsyncEventingBasicConsumer capturedConsumer = null;

        _mockChannel
            .Setup(c => c.BasicConsume(
                "transactions",
                false,
                "",
                false,
                false,
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<IBasicConsumer>()))
            .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                (queue, autoAck, tag, noLocal, exclusive, args, consumer) =>
                {
                    capturedConsumer = (AsyncEventingBasicConsumer)consumer;
                });

        // Act
        await _queue.ConsumeAsync(handler);

        // simulate message delivery
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await capturedConsumer!.HandleBasicDeliver(
            "ctag",
            1,
            false,
            "",
            "",
            null,
            new ReadOnlyMemory<byte>(body));

        // Assert
        received.Should().NotBeNull();
        received!.Id.Should().Be(789);
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

        AsyncEventingBasicConsumer capturedConsumer = null;

        _mockChannel
            .Setup(c => c.BasicConsume(
                It.IsAny<string>(),
                false,
                "",
                false,
                false,
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<IBasicConsumer>()))
            .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                (q, a, t, n, e, args, consumer) =>
                {
                    capturedConsumer = (AsyncEventingBasicConsumer)consumer;
                });

        // Act
        await _queue.ConsumeAsync(handler);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await capturedConsumer!.HandleBasicDeliver(
            "ctag",
            1,
            false,
            "",
            "",
            null,
            new ReadOnlyMemory<byte>(body));

        // Assert
        _mockChannel.Verify(c => c.BasicAck(1, false), Times.Once);
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

        AsyncEventingBasicConsumer capturedConsumer = null;

        _mockChannel
            .Setup(c => c.BasicConsume(
                It.IsAny<string>(),
                false,
                "",
                false,
                false,
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<IBasicConsumer>()))
            .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>(
                (q, a, t, n, e, args, consumer) =>
                {
                    capturedConsumer = (AsyncEventingBasicConsumer)consumer;
                });

        // Act
        await _queue.ConsumeAsync(handler);

        var body = Encoding.UTF8.GetBytes("invalid json");

        await capturedConsumer!.HandleBasicDeliver(
            "ctag",
            1,
            false,
            "",
            "",
            null,
            new ReadOnlyMemory<byte>(body));

        // Assert
        handlerCalled.Should().BeFalse();
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
                false,
                It.IsAny<IDictionary<string, object>>()),
            Times.Once);
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
}