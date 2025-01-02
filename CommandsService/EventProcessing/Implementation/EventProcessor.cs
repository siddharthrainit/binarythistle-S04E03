using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory =
            serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public void ProcessEvent(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Console.WriteLine("Empty message received");
            return;
        }

        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message).GetAwaiter().GetResult();
                break;
            default:
                Console.WriteLine("Could not determine event type");
                break;
        }
    }

    private async Task AddPlatform(string message)
    {
        PlatformPublishedDto platformPublishedDto = ConvertToPlatformPublishDto(message);

        using var scope = _serviceScopeFactory.CreateScope();
        var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        if (commandRepo is null)
        {
            Console.WriteLine("Could not retrieve ICommandRepository");
            throw new InvalidOperationException(nameof(commandRepo));
        }

        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);
            if (await commandRepo.ExternalPlatformExistsAsync(platform.PlatformId))
            {
                Console.WriteLine("--> Platform already exists");
                return;
            }

            await commandRepo.CreatePlatformAsync(platform);
            await commandRepo.SaveChangesAsync();
            Console.WriteLine("--> Platform added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not add Platform to DB - {ex.Message}");
        }
    }

    private static PlatformPublishedDto ConvertToPlatformPublishDto(string message)
    {
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
        if (platformPublishedDto is null)
        {
            Console.WriteLine("Could not deserialize PlatformPublishedDto");
            throw new ArgumentNullException(nameof(platformPublishedDto));
        }

        return platformPublishedDto;
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        if (eventType is null || string.IsNullOrEmpty(eventType.Event))
        {
            Console.WriteLine("Could not determine event type");
            return EventType.Undetermined;
        }

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("Platform Published Event Detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("Could not determine event type");
                return EventType.Undetermined;
        }
    }
}
