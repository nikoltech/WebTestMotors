using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using WebTestMotors.DataAccess;
using WebTestMotors.DataAccess.Entities;
using WebTestMotors.DataAccess.MongoDb;
using WebTestMotors.Integration.Tests.Fixtures;
using Xunit;

namespace WebTestMotors.Integration.Tests.Scenarios
{
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;


        // Mocks
        private Mock<IOptions<MongoDbSettings>> _mockMongoOptions;

        private Mock<IMongoDatabase> _mockMongoDB;

        private Mock<IMongoClient> _mockMongoClient;


        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            this._factory = factory;

            this._mockMongoOptions = new Mock<IOptions<MongoDbSettings>>();
            this._mockMongoDB = new Mock<IMongoDatabase>();
            this._mockMongoClient = new Mock<IMongoClient>();
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
        public async Task AddCarTestAsync()
        {
            // Arange
            Mock<IBsonSerializer<Car>> mockCarDocumentSerializer = new Mock<IBsonSerializer<Car>>();

            Mock<IMongoIndexManager<Car>> mockCarIndexManager = new Mock<IMongoIndexManager<Car>>();


            Mock<IMongoCollection<Car>> mockCarMongoCollection = new Mock<IMongoCollection<Car>>();
            mockCarMongoCollection.SetupAllProperties();
            IMongoCollection<Car> carMongoCollection = mockCarMongoCollection.Object;

            //List<Car> carList = new List<Car>();
            //carMongoCollection.InsertMany(carList);

            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(carMongoCollection).Verifiable();

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
            Car newCar = new Car
            {
                Name = "SomeCar2",
                Description = "SomeDesc"
            };
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
    }
}
