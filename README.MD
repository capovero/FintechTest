
# Fintech Futures Spread Analyzer

This is a .NET 9 microservice-based system designed to fetch BTC quarterly futures prices, calculate the spread between near and far contracts, and persist the result. It uses RabbitMQ for inter-service messaging and PostgreSQL for storage.

---

##  Use Case

The application analyzes the spread between two BTC futures contracts (e.g., `BTCUSDT_250926` and `BTCUSDT_251226`) in real-time and stores the spread for further analysis.

---

##  Architecture Overview

### 1. **DataFetcher**

* Fetches prices from Binance API.
* Publishes them as messages to RabbitMQ.
* Hangfire is used to run periodic fetch jobs.

### 2. **SpreadCalculator**

* Listens to RabbitMQ messages.
* Caches received prices.
* Calculates the spread when both prices are available.
* Stores the result in PostgreSQL.

---

##  Getting Started

### Requirements

* Docker
* Docker Compose

### Launch Project

```bash
docker-compose up --build
```

Services started:

* DataFetcher API (`localhost:5001`)
* SpreadCalculator API (`localhost:5002`)
* RabbitMQ Management UI (`localhost:15672`)
* PostgreSQL DB

---

##  API Endpoints

###  Manually Fetch Prices

```
GET http://localhost:5001/api/Prices/fetch
```

Triggers a fetch from Binance and sends messages to the queue.

###  Get Spread Data

```
GET http://localhost:5002/api/Spread
```

Example response:

```json
[
  {
    "id": 1,
    "timestamp": "2025-07-08T12:53:02.6311129Z",
    "nearPrice": 109957.3,
    "farPrice": 111871.9,
    "spread": 1914.6
  }
]
```

---

##  Entities in SpreadCalculator

### 1. `FuturePrice`

Represents individual prices received from the queue.

```csharp
public class FuturePrice
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string ContractCode { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }
}
```


### 2. `SpreadResult`

Represents the final spread between two prices and is saved in the DB.

```csharp
public class SpreadResult
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal NearPrice { get; set; }
    public decimal FarPrice { get; set; }
    public decimal Spread => FarPrice - NearPrice;
}
```

---


##  Testing

### Run unit tests:

```bash
dotnet test SpreadCalculator/tests/SpreadCalculator.Tests

dotnet test DataFetcher/tests/DataFetcher.Tests
```

---

##  Technologies Used

* .NET 9
* ASP.NET Core Web API
* Entity Framework Core
* RabbitMQ
* Hangfire
* PostgreSQL
* xUnit (unit testing)
* Clean Architecture

---

##  Hangfire Scheduler

In `DataFetcher`, Hangfire runs the background job:

```csharp
RecurringJob.AddOrUpdate<FetchPricesJob>(
    "fetch-prices-every-minute",
    job => job.ExecuteAsync(),
    Cron.Minutely
);
```

Dashboard: [http://localhost:5001/hangfire](http://localhost:5001/hangfire)
