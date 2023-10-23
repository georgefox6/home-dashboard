using HomeDashboardApi.Request;
using HomeDashboardApi.Response;
using MediatR;
using Azure.Data.Tables;
using Azure;
using HomeDashboardData.Models;
using HomeDashboardApi.Exceptions.NoBinSchedulesFound;
using HomeDashboardApi.StorageClients;

namespace HomeDashboardApi.Handlers.BinSchedule;
public class GetBinScheduleHandler : IRequestHandler<GetBinScheduleRequest, GetBinScheduleResponse>
{
    private readonly BinScheduleTableClient _binScheduleTableClient;
    private readonly ILogger<GetBinScheduleHandler> logger;

    public GetBinScheduleHandler(BinScheduleTableClient binScheduleTableClient, ILogger<GetBinScheduleHandler> logger)
    {
        _binScheduleTableClient = binScheduleTableClient;
        this.logger = logger;
    }

    public async Task<GetBinScheduleResponse> Handle(GetBinScheduleRequest request, CancellationToken cancellationToken)
    {
        var currentDateFilter = $"PartitionKey eq 'BinSchedule' and RowKey ge '{DateTime.Now.ToString("yyyyMMdd")}'";

        var queryResponse = _binScheduleTableClient.Client.Query<BinScheduleEntity>(filter: currentDateFilter);

        var binSchedules = new List<BinScheduleDto>();

        foreach (var entity in queryResponse)
        {
            var binSchedule = new BinScheduleDto
            {
                BinColour = entity.BinColour,
                BinDate = entity.BinDate,
            };
            binSchedules.Add(binSchedule);
        }

        if (binSchedules.Count == 0)
        {
            logger.LogWarning($"No bin schedules exist after the current date {DateTime.Now}");
            throw new NoBinSchedulesFoundException();
        }

        var response = new GetBinScheduleResponse
        {
            BinSchedules = binSchedules
        };

        return response;
    }
}

public class BinScheduleEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string BinColour { get; set; }
    public DateTime BinDate { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}