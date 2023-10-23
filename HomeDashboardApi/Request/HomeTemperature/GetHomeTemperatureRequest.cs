using HomeDashboardApi.Response;
using HomeDashboardData.Enums;
using MediatR;

namespace HomeDashboardApi.Request
{
    public class GetHomeTemperatureRequest : IRequest<GetHomeTemperatureResponse>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Room Room { get; set; }
    }
}
