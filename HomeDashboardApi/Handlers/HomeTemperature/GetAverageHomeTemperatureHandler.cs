using HomeDashboardApi.Request;
using HomeDashboardApi.Response;
using MediatR;
using HomeDashboardData.Enums;
using HomeDashboardApi.Exceptions.TemperatureNotFound;
using HomeDashboardData.Models;
using HomeDashboardApi.StorageClients;

namespace HomeDashboardApi.Handlers.HomeTemperature;
public class GetAverageHomeTemperatureHandler : IRequestHandler<GetAverageHomeTemperatureRequest, GetAverageHomeTemperatureResponse>
{
    private readonly TemperatureTableClient _temperatureTableClient;
    private readonly ILogger<GetAverageHomeTemperatureHandler> logger;

    public GetAverageHomeTemperatureHandler(TemperatureTableClient temperatureTableClient, ILogger<GetAverageHomeTemperatureHandler> logger)
    {
        _temperatureTableClient = temperatureTableClient;
        this.logger = logger;
    }

    public async Task<GetAverageHomeTemperatureResponse> Handle(GetAverageHomeTemperatureRequest request, CancellationToken cancellationToken)
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

        var averageTemperature = temperatures.Select(x => x.Temperature).Average();
        var averageHumidity = temperatures.Select(x => x.Humidity).Average();

        var referenceDate = new DateTime(2000, 1, 1);
        TimeSpan averageTimeSpan = TimeSpan.FromTicks((long)temperatures.Select(x => x.Date).Select(date => (date - referenceDate).Ticks).Average());
        DateTime averageDate = referenceDate + averageTimeSpan;


        var response = new GetAverageHomeTemperatureResponse
        {
            Temperature = new HomeTemperatureDto
            {
                Room = request.Room,
                Date = averageDate,
                Temperature = averageTemperature,
                Humidity = averageHumidity
            }
        };

        return response;
    }
}