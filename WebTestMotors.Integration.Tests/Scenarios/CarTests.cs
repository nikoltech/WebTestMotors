namespace WebTestMotors.Integration.Tests.Scenarios
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Moq;
    using System.Net;
    using System.Threading.Tasks;
    using WebTestMotors.DataAccess;
    using WebTestMotors.DataAccess.MongoDb;
    using Xunit;

    public class CarTests
    {
        private readonly IHostBuilder hostBuilder;

        public Mock<IDataContext> DataContextServiceMock { get; private set; }

        public CarTests()
        {
            // Arange
            this.hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseTestServer().ConfigureTestServices(
                    services =>
                    {
                        foreach (var item in services)
                        {
                            if (item.ServiceType == typeof(IMongoDbSettings))
                            {
                                services.Remove(item);
                                break;
                            }
                        }

                        services.AddSingleton<MongoDbSettings>(t => new MongoDbSettings 
                        { 
                            ConnectionString = "mongod --storageEngine inMemory",
                            DatabaseName = "testDB"
                        });
                    });
                    webBuilder.UseStartup<Startup>();
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

        //[Fact]
        //public async Task AddCarTestAsync()
        //{
        //    var host = await this.hostBuilder.StartAsync();
        //    var client = host.GetTestClient();

        //    // Act
        //    Car newCar = new Car
        //    {
        //        Name = "SomeCar",
        //        Description = "SomeDesc"
        //    };
        //    var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);

        //    StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);

        //    response.EnsureSuccessStatusCode();

        //    // Assert
        //    var responseString = await response.Content?.ReadAsStringAsync();
        //    Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
        //    Assert.Equal(newCar.Name, resultCar.Name);
        //    Assert.Equal(newCar.Description, resultCar.Description);
        //}

        //[Fact]
        //public async Task GetCarListTestAsync()
        //{
        //    // Arange
        //    var host = await this.hostBuilder.StartAsync();
        //    var client = host.GetTestClient();

        //    // Act
        //    var response = await client.GetAsync("/api/car/List");

        //    response.EnsureSuccessStatusCode();

        //    // Assert
        //    var responseString = await response.Content?.ReadAsStringAsync();
        //    List<Car> resultCar = (List<Car>)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(List<Car>));
        //    Assert.NotNull(resultCar);
        //}
    }
}
