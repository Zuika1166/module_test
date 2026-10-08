# Калькулятор бюджета

## Запуск

Требуется .NET SDK 8.0.

```bash
dotnet restore BudgetCalculator.sln
dotnet test BudgetCalculator.sln --configuration Release
dotnet test BudgetCalculator.sln --configuration Release --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
dotnet run --project src/BudgetCalculator.Api
```

API: `POST /api/budget/calculate`

```json
{"dailyCost":100,"days":5,"includeInsurance":true}
```

Ответ: `200 OK` — `{"totalAmount":525}`.

