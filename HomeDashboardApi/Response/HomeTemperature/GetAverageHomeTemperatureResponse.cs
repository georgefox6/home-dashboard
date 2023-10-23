using HomeDashboardData.Models;

namespace HomeDashboardApi.Response;
public class GetAverageHomeTemperatureResponse
{
    public HomeTemperatureDto? Temperature { get; set; }
}