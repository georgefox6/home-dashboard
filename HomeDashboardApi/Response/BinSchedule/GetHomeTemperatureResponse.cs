using HomeDashboardData.Models;

namespace HomeDashboardApi.Response;
public class GetBinScheduleResponse
{
    public List<BinScheduleDto>? BinSchedules { get; set; }
}

