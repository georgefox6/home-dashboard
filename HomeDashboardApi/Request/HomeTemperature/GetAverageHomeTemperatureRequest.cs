using HomeDashboardApi.Response;
using HomeDashboardData.Enums;
using MediatR;

namespace HomeDashboardApi.Request
{
    public class GetAverageHomeTemperatureRequest : IRequest<GetAverageHomeTemperatureResponse>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Room Room { get; set; }
    }
}
