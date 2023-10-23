using HomeDashboardApi.Request;
using HomeDashboardData.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeDashboardApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeTemperatureController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger<HomeTemperatureController> _logger;

    public HomeTemperatureController(IMediator mediator, ILogger<HomeTemperatureController> logger)
    {
        this.mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get Home Temperature
    /// </summary>
    /// <param name="startDate">The start date of the data that you want to return.</param>
    /// <param name="endDate">The end date of the data that you want to return.</param>
    /// <param name="room">The room which you want to return data for.</param>
    /// <returns> A GetHomeTemperatureResponse <see cref="GetHomeTemperatureResponse"/>.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetHomeTemperature(DateTime startDate, DateTime endDate, Room room)
    {
        var request = new GetHomeTemperatureRequest
        {
            StartDate = startDate,
            EndDate = endDate,
            Room = room,
        };
        var result = await mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Get the average home temperature over a period of time for a specific room
    /// </summary>
    /// <param name="startDate">The start date of the data that you want to return.</param>
    /// <param name="endDate">The end date of the data that you want to return.</param>
    /// <param name="room">The room which you want to return data for.</param>
    /// <returns> A GetHomeTemperatureResponse <see cref="GetHomeTemperatureResponse"/>.</returns>
    [HttpGet]
    [Route("Average")]
    public async Task<IActionResult> GetHomeTemperatureAverage(DateTime startDate, DateTime endDate, Room room)
    {
        var request = new GetAverageHomeTemperatureRequest
        {
            StartDate = startDate,
            EndDate = endDate,
            Room = room,
        };
        var result = await mediator.Send(request);

        return Ok(result);
    }
}