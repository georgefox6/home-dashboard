using HomeDashboardApi.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeDashboardApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BinScheduleController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ILogger<BinScheduleController> _logger;

    public BinScheduleController(IMediator mediator, ILogger<BinScheduleController> logger)
    {
        this.mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get Bin Schedule
    /// </summary>
    /// <returns> A BinScheduleResponse <see cref="BinScheduleResponse"/>.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetBinSchedule()
    {
        var request = new GetBinScheduleRequest{};
        var result = await mediator.Send(request);

        return Ok(result);
    }
}