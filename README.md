Flight Aggregator API
📋 Overview
Flight Aggregator API — это REST‑сервис на .NET 8, который собирает доступные рейсы из нескольких источников (провайдеров), предоставляет единый унифицированный ответ с возможностью фильтрации, сортировки и кэширования, а также позволяет бронировать выбранные рейсы.

🏗 Architecture
java
Копировать
FlightAggregatorSolution
│
├── FlightAggregator.Api           ← ASP.NET Core Web API (Controllers → Endpoints)
│
├── FlightAggregator.Business      ← Бизнес‑логика (Search, Booking, Caching)
│
├── FlightAggregator.Providers     ← ExternalProviders + Interfaces (IFlightProvider)
│
├── FlightAggregator.Models        ← DTOs / Records (Flight, BookingRequest) + Configurations
│
└── FlightAggregator.Tests         ← xUnit Unit Tests
API Layer: обрабатывает HTTP‑запросы, делегирует логику сервисам и возвращает JSON‑ответы.

Business Layer: агрегирует данные, применяет фильтрацию/сортировку и кеширует результаты через IDistributedCache.

Providers Layer: каждый провайдер реализует IFlightProvider, возвращает тестовые наборы рейсов и выполняет бронирование.

Models: доменные сущности и конфигурация тестовых данных.

Logging: Serilog → файл logs/log.txt (UseSerilogRequestLogging).

⚙️ Getting Started
Prerequisites
.NET 8 SDK

Run Locally
git clone <repo-url>
cd FlightAggregator.Api
dotnet restore
dotnet run
Swagger UI → https://localhost:5001/swagger

🚀 API Endpoints
GET /api/flights
Поиск рейсов с фильтрами:

Query Parameter	Required	Description
departure	✔️	Город вылета
destination	✔️	Город прибытия
date	✔️	Дата (YYYY-MM-DD)
maxStops	❌	Макс. пересадок
maxPrice	❌	Макс. цена
airline	❌	Авиакомпания
sortBy	❌	Сортировка (price, stops, date)
Response: 200 OK JSON array of Flight

POST /api/bookings
Бронирование рейса:

json
{
  "flightId": "FP1-001",
  "provider": "Provider1",
  "passengerName": "John Doe",
  "passengerEmail": "john@example.com"
}
Response:

200 OK → { "Message": "Бронирование подтверждено" }

400 Bad Request → { "Message": "Ошибка бронирования" }

📦 Caching
IDistributedCache с sliding expiration 5 мин.

Ключ = комбинация всех входных параметров запроса.

📑 Logging
Serilog → файл logs/log.txt

HTTP request logging через app.UseSerilogRequestLogging()

Логи ошибок и cache hits/misses.

✅ Testing
Запуск unit‑тестов:

bash
cd FlightAggregator.Tests
dotnet test
Покрытие:

Агрегация поиска

Правильный выбор провайдера при бронировании

🚧 Future Improvements
Интеграция с реальными API провайдеров

Аутентификация/Authorization (JWT)

Расширенная политика retry/fallback через Polly

Docker‑образ + CI/CD pipeline
