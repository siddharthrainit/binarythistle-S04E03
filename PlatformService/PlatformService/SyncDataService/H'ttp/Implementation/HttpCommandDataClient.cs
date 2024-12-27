using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.Contracts;

namespace PlatformService.SyncDataService.Http.Implementation;

public sealed class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task SendPlatformToCommand(PlatformRead platformRead)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(platformRead),Encoding.UTF8, "applicaton/json");
        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/commands/Platforms", httpContent);

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync post to command service was {0}", JsonSerializer.Serialize(response));
        }else{
            Console.WriteLine("--> Faulty Sync post to command service was {0}", response.StatusCode);
        }
    }
}
