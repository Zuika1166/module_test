using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Api.Controllers;

[ApiController]
[Route("api/budget")]
public sealed class BudgetController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] BudgetRequest request)
    {
        if (request.Days <= 0)
        {
            return BadRequest("Количество дней должно быть больше нуля.");
        }

        var total = request.DailyCost * request.Days;

        if (request.IncludeInsurance)
        {
            // Deliberate original defect for the red/green test demonstration.
            total *= 5m;
        }

        return Ok(new BudgetResponse(decimal.Round(total, 2, MidpointRounding.AwayFromZero)));
    }
}
