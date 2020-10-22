namespace WebTestMotors.Integration.Tests.Scenarios
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MongoDB.Driver;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using WebTestMotors.DataAccess;
    using WebTestMotors.DataAccess.Entities;
    using WebTestMotors.DataAccess.MongoDb;
    using WebTestMotors.Integration.Tests.Fixtures;
    using Xunit;

    public class CarTests
    {
        private readonly IHostBuilder hostBuilder;
        private HashSet<Car> _data = new HashSet<Car>();
        // private readonly Mock<IDataContext> dataContextMock;

        public CarTests()
        {
            // Arange
            this.hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        //.UseEnvironment("Test")
                        .UseStartup<Startup>()
                        .ConfigureTestServices(services =>
                        {
                            //var serviceDescriptors = services.Where(x => x.ServiceType == typeof(MongoDbSettings)).FirstOrDefault();
                            //foreach (var serviceDescriptor in serviceDescriptors)
                            //{
                            //    services.Remove(serviceDescriptor);
                            //}

                            services.Configure<MongoDbSettings>(_ => new MongoDbSettings
                            {
                                ConnectionString = "mongod --storageEngine inMemory",
                                DatabaseName = "testDB"
                            });

                            //var serviceDescriptors = services.Where(x => x.ServiceType == typeof(IDataContext)).ToList();
                            //foreach (var serviceDescriptor in serviceDescriptors)
                            //{
                            //    services.Remove(serviceDescriptor);
                            //}

                            //Mock<IMongoCollection<Car>> mockMongoCar = new Mock<IMongoCollection<Car>>();
                            //Mock<IDataContext> mockContext = new Mock<IDataContext>();
                            //mockContext.Setup(_ => _.Cars).Returns(mockMongoCar.Object);
                            //services.AddScoped(typeof(IDataContext), _ => mockContext.Object);

                        });
                });
        }

        [Fact]
        public async Task CarReturnsOkResponseTestAsync()
        {
            // Arange
            var host = await this.hostBuilder.StartAsync();
            var client = host.GetTestClient();

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
            var host = await this.hostBuilder.StartAsync();
            var client = host.GetTestClient();

            // Act
            Car newCar = new Car
            {
                Name = "SomeCar",
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

        [Fact]
        public async Task GetCarListTestAsync()
        {
            // Arange
            var host = await this.hostBuilder.StartAsync();
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync("/api/car/List");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();
            object resultCar = (List<Car>)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(List<Car>));
            Assert.NotNull(resultCar);
            Assert.IsType<List<Car>>(resultCar);
            Assert.Equal(0, ((List<Car>)resultCar).Count);
        }
    }
}
