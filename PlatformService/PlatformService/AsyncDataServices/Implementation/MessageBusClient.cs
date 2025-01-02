using System.Text;
using System.Text.Json;
using PlatformService.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PlatformService.AysncDataServices.Implementation;

internal sealed class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IChannel _channel;

    // Delegate for getting configuration values
    public Func<string, string> GetConfigurationValue { get; private set; }

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        GetConfigurationValue = key => _configuration[key] ?? throw new ArgumentNullException(key);
        CreateConsumerChannel().GetAwaiter().GetResult();
    }

    private async Task<bool> CreateConsumerChannel()
    {
        var factory = new ConnectionFactory()
        {
            HostName = GetConfigurationValue("RabbitMQHost"),
            Port = int.Parse(GetConfigurationValue("RabbitMQPort")),
        };

        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync("Trigger", ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;
            Console.WriteLine("RabbitMQ Connection Created");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create a connection to the message bus {ex}");
            return false;
        }
    }

    public async Task PublishNewPlatformAsyc(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("RabbitMQ Connection Open, Sending Message");
            await SendMessageAsync(message);
        }
        else
        {
            Console.WriteLine("RabbitMQ Connection is Closed, Not Sending");
        }
    }

    private async Task SendMessageAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: "Trigger", routingKey: "", body: body);

        Console.WriteLine($"Platform Published to RabbitMQ: {message}");
    }

    public async Task DisposeASync()
    {
        Console.WriteLine("MessageBus Disposed");
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
        }
    }

    private Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }
}
