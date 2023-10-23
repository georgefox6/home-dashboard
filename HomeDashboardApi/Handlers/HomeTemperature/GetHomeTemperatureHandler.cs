using HomeDashboardApi.Request;
using HomeDashboardApi.Response;
using MediatR;
using Azure.Data.Tables;
using HomeDashboardData.Enums;
using Azure;
using HomeDashboardApi.Exceptions.TemperatureNotFound;
using HomeDashboardData.Models;
using HomeDashboardApi.StorageClients;

namespace HomeDashboardApi.Handlers.HomeTemperature;
public class GetHomeTemperatureHandler : IRequestHandler<GetHomeTemperatureRequest, GetHomeTemperatureResponse>
{
    private readonly TemperatureTableClient _temperatureTableClient;
    private readonly ILogger<GetHomeTemperatureHandler> logger;

    public GetHomeTemperatureHandler(TemperatureTableClient temperatureTableClient, ILogger<GetHomeTemperatureHandler> logger)
    {
        _temperatureTableClient = temperatureTableClient;
        this.logger = logger;
    }

    public async Task<GetHomeTemperatureResponse> Handle(GetHomeTemperatureRequest request, CancellationToken cancellationToken)
    {
        var roomFilter = $"PartitionKey eq '{request.Room.ToString()}'";
        var startDateFilter = $"DateTime ge '{request.StartDate:yyyy-MM-dd HH:mm:ss.fffffff}'";
        var endDateFilter = $"DateTime le '{request.EndDate:yyyy-MM-dd HH:mm:ss.fffffff}'";
        var combinedFilter = $"{roomFilter} and {startDateFilter} and {endDateFilter}";

        var queryResponse = _temperatureTableClient.Client.Query<HomeTemperatureEntity>(filter: combinedFilter);

        var temperatures = new List<HomeTemperatureDto>();

        foreach (var entity in queryResponse)
        {
            var temp = new HomeTemperatureDto
            {
                Room = (Room)Enum.Parse(typeof(Room), entity.Room),
                Date = entity.DateTime,
                Temperature = entity.Temperature,
                Humidity = entity.Humidity
            };
            temperatures.Add(temp);
        }

        if (temperatures.Count == 0)
        {
            logger.LogWarning($"Temperature data not found for room: {request.Room} between dates: {request.StartDate} and {request.EndDate}");
            throw new TemperatureNotFoundException();
        }

        var response = new GetHomeTemperatureResponse
        {
            Temperatures = temperatures
        };

        return response;
    }
}

public class HomeTemperatureEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Room { get; set; }
    public DateTime DateTime { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}