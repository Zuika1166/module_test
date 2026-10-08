using System.Globalization;
using BudgetCalculator.Api.Controllers;
using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BudgetCalculator.Tests;

public sealed class BudgetControllerTests
{
    private readonly BudgetController _controller = new();

    [Fact]
    public void Calculate_WithoutInsurance_Returns200And500()
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = false };

        var ok = Assert.IsType<OkObjectResult>(_controller.Calculate(request));
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        var body = Assert.IsType<BudgetResponse>(ok.Value);
        Assert.Equal(500m, body.TotalAmount);
    }

    [Fact]
    public void Calculate_WithInsurance_AddsFivePercent()
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = true };

        var ok = Assert.IsType<OkObjectResult>(_controller.Calculate(request));
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        var body = Assert.IsType<BudgetResponse>(ok.Value);
        Assert.Equal(525m, body.TotalAmount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Calculate_WithNonPositiveDays_Returns400(int days)
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = days, IncludeInsurance = false };

        var badRequest = Assert.IsType<BadRequestObjectResult>(_controller.Calculate(request));
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", badRequest.Value);
    }

    [Fact]
    public void Calculate_WithZeroDailyCost_Returns200AndZero()
    {
        var request = new BudgetRequest { DailyCost = 0m, Days = 5, IncludeInsurance = false };

        var ok = Assert.IsType<OkObjectResult>(_controller.Calculate(request));
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        var body = Assert.IsType<BudgetResponse>(ok.Value);
        Assert.Equal(0m, body.TotalAmount);
    }

    [Theory]
    [InlineData("100.50", 3, "301.50")]
    [InlineData("0.335", 1, "0.34")]
    [InlineData("0.334", 1, "0.33")]
    public void Calculate_WithFractionalDailyCost_UsesDecimalAndRoundsToCents(
        string dailyCost, int days, string expected)
    {
        var request = new BudgetRequest
        {
            DailyCost = decimal.Parse(dailyCost, CultureInfo.InvariantCulture),
            Days = days,
            IncludeInsurance = false
        };

        var ok = Assert.IsType<OkObjectResult>(_controller.Calculate(request));
        var body = Assert.IsType<BudgetResponse>(ok.Value);
        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), body.TotalAmount);
    }

    [Fact]
    public void Calculate_InsuranceFractionalCents_RoundsHalfAwayFromZero()
    {
        var request = new BudgetRequest { DailyCost = 100.50m, Days = 3, IncludeInsurance = true };

        var ok = Assert.IsType<OkObjectResult>(_controller.Calculate(request));
        var body = Assert.IsType<BudgetResponse>(ok.Value);
        Assert.Equal(316.58m, body.TotalAmount);
    }
}
