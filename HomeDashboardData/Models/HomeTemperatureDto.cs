using HomeDashboardData.Enums;

namespace HomeDashboardData.Models;
public class HomeTemperatureDto
{
    public Room Room { get; set; }
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}