using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Configuration = Microsoft.Extensions.Configuration.IConfiguration;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    private readonly Configuration _config;
    private readonly IEventProcessor _eventProcessor;

    // Delegate for getting configuration values
    public Func<string, string> GetConfigurationValue { get; private set; }

    public MessageBusSubscriber(Configuration config, IEventProcessor eventProcessor)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));

        GetConfigurationValue = key => _config[key] ?? throw new ArgumentNullException(key);
        InitializeRabbitMQ().GetAwaiter().GetResult();
    }

    public override void Dispose()
    {
        CloseConnectionAsyc().GetAwaiter().GetResult();

        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("--> ExecuteAsync Started");
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event received");

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                try
                {
                    _eventProcessor.ProcessEvent(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error processing event: {ex.Message}");
                }

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
        }

        Console.WriteLine("--> ExecuteAsync Ended");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await CloseConnectionAsyc();

        await base.StopAsync(cancellationToken);
    }

    private async Task CloseConnectionAsyc()
    {
        Console.WriteLine("--> Closing RabbitMQ connection...");
        try
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                Console.WriteLine("--> Channel closed...");
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
                Console.WriteLine("--> Connection closed...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error closing RabbitMQ connection: {ex.Message}");
        }

        Console.WriteLine("--> RabbitMQ connection closed...");
    }

    private async Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs e)
    {
        await CloseConnectionAsyc();
    }

    private async Task InitializeRabbitMQ()
    {
        Console.WriteLine("--> Connecting to RabbitMQ: {0}", GetConfigurationValue("RabbitMQHost"));
        var factory = new ConnectionFactory
        {
            HostName = GetConfigurationValue("RabbitMQHost"),
            Port = int.Parse(GetConfigurationValue("RabbitMQPort")),
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(exchange: "Trigger", type: ExchangeType.Fanout);
        var queue = await _channel.QueueDeclareAsync();
        _queueName = queue.QueueName;
        await _channel.QueueBindAsync(queue: _queueName, exchange: "Trigger", routingKey: "");
        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;
        Console.WriteLine("--> Listening on the message bus...");
    }
}
