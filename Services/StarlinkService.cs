using Grpc.Core;
using Grpc.Net.Client;
using SpaceX.API.Device;
using star_map.Models;

namespace star_map.Services;

/// <summary>
/// Service for communicating with Starlink device via gRPC
/// </summary>
public class StarlinkService : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Device.DeviceClient _client;
    private readonly string _endpoint;
    private ulong _requestId = 1;

    public StarlinkService(string endpoint)
    {
        _endpoint = endpoint;

        var httpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        _channel = GrpcChannel.ForAddress($"http://{_endpoint}", new GrpcChannelOptions
        {
            HttpHandler = httpHandler,
            MaxReceiveMessageSize = 1024 * 1024 * 100,
            MaxSendMessageSize = 1024 * 1024 * 100
        });

        _client = new Device.DeviceClient(_channel);
    }

    /// <summary>
    /// Gets location data from Starlink device
    /// </summary>
    /// <param name="source">Position source to use</param>
    /// <returns>Location data</returns>
    public async Task<LocationData> GetLocationAsync(PositionSource source = PositionSource.Auto)
    {
        var request = new Request
        {
            Id = _requestId++,
            EpochId = 0,
            TargetId = "",
            GetLocation = new GetLocationRequest
            {
                Source = source
            }
        };

        try
        {
            var response = await _client.HandleAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            ValidateResponse(response, "GetLocation");
            return LocationData.FromResponse(response.GetLocation);
        }
        catch (RpcException ex)
        {
            throw new Exception($"gRPC не піднявся (Код: {ex.StatusCode}): {ex.Status.Detail}", ex);
        }
    }

    /// <summary>
    /// Gets dish status including azimuth and elevation data
    /// </summary>
    /// <returns>Dish status data</returns>
    public async Task<DishStatus> GetDishStatusAsync()
    {
        var request = new Request
        {
            Id = _requestId++,
            EpochId = 0,
            TargetId = "",
            DishGetStatus = new DishGetStatusRequest()
        };

        try
        {
            var response = await _client.HandleAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            ValidateResponse(response, "GetDishStatus");
            return DishStatus.FromResponse(response.DishGetStatus);
        }
        catch (RpcException ex)
        {
            throw new Exception($"gRPC не піднявся (Код: {ex.StatusCode}): {ex.Status.Detail}", ex);
        }
    }

    public async Task<StarlinkData> GetCompleteDataAsync()
    {
        var location = await GetLocationAsync();

        return new StarlinkData
        {
            Location = location,
            Status = null,
            Endpoint = _endpoint
        };
    }

    private void ValidateResponse(Response response, string operationName)
    {
        if (response.Status?.Code != 0)
        {
            throw new Exception($"{operationName} помилка: {response.Status?.Message ?? "Я їбу що не так"}");
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        GC.SuppressFinalize(this);
    }
}
