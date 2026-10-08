using System.Net;
using System.Net.Http.Json;
using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BudgetCalculator.Tests;

public sealed class BudgetEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BudgetEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCalculate_WithoutInsurance_Returns200WithJsonTotal()
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = false };

        using var response = await _client.PostAsJsonAsync("/api/budget/calculate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BudgetResponse>();
        Assert.NotNull(body);
        Assert.Equal(500m, body.TotalAmount);
    }

    [Fact]
    public async Task PostCalculate_WithInsurance_Returns525()
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = 5, IncludeInsurance = true };

        using var response = await _client.PostAsJsonAsync("/api/budget/calculate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BudgetResponse>();
        Assert.NotNull(body);
        Assert.Equal(525m, body.TotalAmount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task PostCalculate_WithNonPositiveDays_Returns400(int days)
    {
        var request = new BudgetRequest { DailyCost = 100m, Days = days, IncludeInsurance = false };

        using var response = await _client.PostAsJsonAsync("/api/budget/calculate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Contains("Количество дней должно быть больше нуля.", message);
    }

    [Fact]
    public async Task PostCalculate_WithZeroDailyCost_ReturnsZero()
    {
        var request = new BudgetRequest { DailyCost = 0m, Days = 5, IncludeInsurance = false };

        using var response = await _client.PostAsJsonAsync("/api/budget/calculate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BudgetResponse>();
        Assert.NotNull(body);
        Assert.Equal(0m, body.TotalAmount);
    }

    [Fact]
    public async Task PostCalculate_WithDecimalDailyCost_ReturnsExactCents()
    {
        var request = new BudgetRequest { DailyCost = 100.50m, Days = 3, IncludeInsurance = false };

        using var response = await _client.PostAsJsonAsync("/api/budget/calculate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BudgetResponse>();
        Assert.NotNull(body);
        Assert.Equal(301.50m, body.TotalAmount);
    }
}
