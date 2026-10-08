namespace BudgetCalculator.Api.Models;

public sealed class BudgetRequest
{
    public decimal DailyCost { get; set; }
    public int Days { get; set; }
    public bool IncludeInsurance { get; set; }
}
