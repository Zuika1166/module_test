# Калькулятор бюджета — тестирование ASP.NET Core API

Учебное задание: покрыть готовую логику расчёта бюджета xUnit-тестами, обнаружить ошибку расчёта страховки и исправить её.

> Исходный GitHub-репозиторий был пустым. Поэтому добавлен минимальный ASP.NET Core Web API по коду из ТЗ вместе с отдельным тестовым проектом.

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

Правила: число дней должно быть положительным; страховка увеличивает сумму на 5%; денежные суммы округляются до двух знаков по правилу AwayFromZero. При `days <= 0` возвращается 400 Bad Request.

## Проверки

- Базовый расчёт без страховки: 100 × 5 = 500, HTTP 200.
- Страховка: 100 × 5 × 1.05 = 525.
- Отрицательные и нулевые дни: 400.
- Нулевая стоимость дня: 0, HTTP 200.
- Дробная стоимость: 100.50 × 3 = 301.50.
- Округление до копеек на половинных значениях и при страховке.
- Интеграционные HTTP-тесты (маршрутизация, сериализация и статус ответа).

CI: `.github/workflows/dotnet-tests.yml` запускает сборку и тесты, собирает Cobertura-отчёт и проверяет покрытие обеих веток `if` в `BudgetController.Calculate`. Покрытие контроллера должно составлять 100% по веткам.
