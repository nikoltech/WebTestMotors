using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTestMotors.DataAccess;
using WebTestMotors.DataAccess.Entities;
using WebTestMotors.DataAccess.MongoDb;
using WebTestMotors.DataAccess.Repositories;
using Xunit;

namespace WebTestMotors.Integration.Tests.Scenarios
{
    // TODO: Refactoring with setup & launch WebApplicationFactory an http client
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task CarReturnsOkResponseTestAsync()
        {
            // Arange
            var client = this._factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/car");

            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content?.ReadAsStringAsync();
            Assert.Equal("Started!", responseString);
        }

        [Fact]
        public async Task GetCarListTestAsync()
        {
            // Arange
            Car newCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            List<Car> carList = new List<Car>() { newCar };

            Mock<IRepository<Car>> carRepoMock = new Mock<IRepository<Car>>();
            carRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(carList);

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carRepoMock.Object).Verifiable();

            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext.Object);
                    }))
                .CreateClient();

            var response = await client.GetAsync($"/api/car/list");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            List<Car> resultCar = (List<Car>)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(List<Car>));
            Assert.Equal(newCar.Name, resultCar.FirstOrDefault()?.Name);
            Assert.Equal(newCar.Description, resultCar.FirstOrDefault()?.Description);
        }

        [Fact]
        public async Task GetCarByIdTestAsync()
        {
            // Arange
            Car newCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            List<Car> carList = new List<Car>() { newCar };

            Mock<IRepository<Car>> carRepoMock = new Mock<IRepository<Car>>();
            carRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(carList);

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carRepoMock.Object).Verifiable();

            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext.Object);
                    }))
                .CreateClient();

            var response = await client.GetAsync($"/api/car/{newCar.Id}");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();
            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Equal(newCar.Name, resultCar.Name);
            Assert.Equal(newCar.Description, resultCar.Description);
        }

        [Fact]
        public async Task AddCarTestAsync()
        {
            // Arange
            Car newCar = new Car
            {
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Mock<IRepository<Car>> carRepoMock = new Mock<IRepository<Car>>();
            carRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Car, bool>>>()))
                .Returns(() => null);

            carRepoMock.Setup(r => r.InsertOneAsync(It.IsAny<Car>()));

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carRepoMock.Object).Verifiable();

            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext.Object);
                    }))
                .CreateClient();

            var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);

            StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();
            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Equal(newCar.Name, resultCar.Name);
            Assert.Equal(newCar.Description, resultCar.Description);
        }

        [Fact]
        public async Task PartialUpdateCarTestAsync()
        {
            // Arange
            Car existCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Car updateToCarInfo = new Car
            {
                Id = existCar.Id,
                Name = "ChangedNameCar"
            };

            List<Car> listCar = new List<Car>() { existCar };

            Mock<IRepository<Car>> carRepoMock = new Mock<IRepository<Car>>();

            Mock<IFindFluent<Car, Car>> mockIFindFluentCar = new Mock<IFindFluent<Car, Car>>();
            //mockIFindFluentCar

            carRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(listCar);

            Mock<IReturnsResult<Car>> mockReturnsResultCar = new Mock<IReturnsResult<Car>>();
            carRepoMock.Setup(r => r.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Car>()))
                .ReturnsAsync(existCar);

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carRepoMock.Object).Verifiable();

            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext.Object);
                    }))
                .CreateClient();

            // Act
            var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(updateToCarInfo);

            StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Equal(updateToCarInfo.Name, resultCar.Name);
            Assert.Equal(existCar.Description, resultCar?.Description);
        }

        [Fact]
        public async Task DeleteCarTestAsync()
        {
            // Arange
            Car existCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            List<Car> listCar = new List<Car>() { existCar };

            Mock<IRepository<Car>> carRepoMock = new Mock<IRepository<Car>>();

            Mock<IFindFluent<Car, Car>> mockIFindFluentCar = new Mock<IFindFluent<Car, Car>>();
            //mockIFindFluentCar

            carRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(listCar);

            Mock<IReturnsResult<Car>> mockReturnsResultCar = new Mock<IReturnsResult<Car>>();
            Mock<DeleteResult> mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(r => r.DeletedCount).Returns(1);
            carRepoMock.Setup(r => r.DeleteOneAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .Callback(() => listCar.Remove(existCar))
                .ReturnsAsync(mockDeleteResult.Object);

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carRepoMock.Object).Verifiable();

            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext.Object);
                    }))
                .CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/car/Remove/{existCar.Id}");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            bool result = (bool)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(bool));
            Assert.True(result);
            Assert.Empty(listCar);
        }
    }
}
