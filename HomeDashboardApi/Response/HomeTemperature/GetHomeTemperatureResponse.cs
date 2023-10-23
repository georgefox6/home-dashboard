using HomeDashboardData.Models;

namespace HomeDashboardApi.Response;
public class GetHomeTemperatureResponse
{
    public List<HomeTemperatureDto>? Temperatures { get; set; }
}

