using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataFetcher.API.Services;
using DataFetcher.Application.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Xunit;

namespace DataFetcher.Tests
{
    public class FuturesFetcherServiceTests
    {
        [Fact]
        public async Task GetPriceAsync_ReturnsDecimal_WhenResponseIsValid()
        {
            var json = @"{ ""symbol"": ""BTCUSDT_250926"", ""price"": ""12345.67"", ""time"": 1620000000000 }";
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(json)
               });
            var httpClient = new HttpClient(handlerMock.Object);
            var svc = new FuturesFetcherService(httpClient, NullLogger<FuturesFetcherService>.Instance);

            var price = await svc.GetPriceAsync("BTCUSDT_250926");

            Assert.NotNull(price);
            Assert.Equal(12345.67m, price.Value);
        }

        [Fact]
        public async Task GetPriceAsync_ReturnsNull_WhenStatusNotSuccess()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.InternalServerError
               });
            var httpClient = new HttpClient(handlerMock.Object);
            var svc = new FuturesFetcherService(httpClient, NullLogger<FuturesFetcherService>.Instance);

            var price = await svc.GetPriceAsync("ANY");

            Assert.Null(price);
        }

        [Fact]
        public async Task GetPriceAsync_ReturnsNull_OnMalformedJson()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{ not a valid json }")
               });
            var httpClient = new HttpClient(handlerMock.Object);
            var svc = new FuturesFetcherService(httpClient, NullLogger<FuturesFetcherService>.Instance);
            
            var price = await svc.GetPriceAsync("ANY");
            
            Assert.Null(price);
        }
    }
}
