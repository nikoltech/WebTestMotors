using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTestMotors.DataAccess;
using WebTestMotors.DataAccess.Entities;
using WebTestMotors.DataAccess.MongoDb;
using Xunit;

namespace WebTestMotors.Integration.Tests.Scenarios
{
    // TODO: Refactoring with setup & launch WebApplicationFactory
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
        public async Task AddCarTestTEMPLATEAsync()
        {
            // Arange
            Mock<IBsonSerializer<Car>> mockCarDocumentSerializer = new Mock<IBsonSerializer<Car>>();

            Mock<IMongoIndexManager<Car>> mockCarIndexManager = new Mock<IMongoIndexManager<Car>>();


            Mock<IMongoCollection<Car>> mockCarMongoCollection = new Mock<IMongoCollection<Car>>();
            mockCarMongoCollection.SetupAllProperties();
            IMongoCollection<Car> carMongoCollection = mockCarMongoCollection.Object;

            //List<Car> carList = new List<Car>();
            //carMongoCollection.InsertMany(carList);

            var collectionMock = Mock.Of<IMongoCollection<Car>>();
            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(collectionMock).Verifiable();

            ///

            //var collectionMock = Mock.Of<IMongoCollection<Car>>();
            var dbMock = new Mock<IMongoDatabase>();

            dbMock
                .Setup(_ => _.GetCollection<Car>("Cars", It.IsAny<MongoCollectionSettings>()))
                .Returns(collectionMock);

            ///

            //// IAsyncCursor

            Car newCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Mock<IMongoCollection<Car>> _mockCollection = new Mock<IMongoCollection<Car>>();
            _mockCollection.SetupAllProperties();
            //_mockCollection.Object.InsertOne(newCar);
            Mock<IDataContext> _mockContext = new Mock<IDataContext>();
            List<Car> _carList = new List<Car>();
            _carList.Add(newCar);


            //Mock MoveNextAsync

            Mock<IAsyncCursor<Car>> _carCursor = new Mock<IAsyncCursor<Car>>();
            _carCursor.Setup(_ => _.Current).Returns(_carList);
            _carCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _carCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                 .Returns(Task.FromResult(false));

            //Mock FindAsync
        //IFindFluent<TDocument, TDocument> Find<TDocument>(this IMongoCollection<TDocument> collection, Expression<Func<TDocument, bool>> filter, FindOptions options = null);

            Mock<IFindFluent<Car, Car>> mockFindFluentCar = new Mock<IFindFluent<Car, Car>>();
            mockFindFluentCar.Setup(f => f.ToCursor(It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            // Error can't use in extension
            //_mockCollection.Setup(op => op.Find<Car>(It.IsAny<Expression<Func<Car, bool>>>(),
            //    It.IsAny<FindOptions>()))
            //    .Returns(mockFindFluentCar.Object);


            // AddCursor
            _mockCollection.Setup(op => op.FindSync<Car>(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_carCursor.Object);

            //Mock GetCollection
            Mock<IDataContext> mockIDataContext2 = new Mock<IDataContext>();
            //_mockContext.Setup(c => c.GetCollection<Car>(typeof(Car).Name)).Returns(_mockCollection.Object);
            mockIDataContext2.Setup(c => c.Cars).Returns(_mockCollection.Object).Verifiable();


            var dbMock2 = new Mock<IMongoDatabase>();

            dbMock2
                .Setup(_ => _.GetCollection<Car>("Cars", It.IsAny<MongoCollectionSettings>()))
                .Returns(_mockCollection.Object);
            ////


            #region IFakeMgCollection

            //IOptions<MongoDbSettings> _mongoSettings;
            ////CarController _queryController;
            //Mock<IFakeMongoCollection> _fakeMongoCollection;
            //Mock<IMongoDatabase> _fakeMongoDatabase;
            //Mock<IDataContext> _fakeMongoContext;
            //Mock<IFindFluent<BsonDocument, BsonDocument>> _fakeCollectionResult;


            //_fakeMongoCollection = new Mock<IFakeMongoCollection>();
            //_fakeCollectionResult = new Mock<IFindFluent<BsonDocument, BsonDocument>>(
            //_fakeMongoDatabase = new Mock<IMongoDatabase>();
            //_fakeMongoDatabase
            //    .Setup(_ => _.GetCollection<BsonDocument>("Test", It.IsAny<MongoCollectionSettings>()))
            //    .Returns(_fakeMongoCollection.Object);

            //_fakeMongoContext = new Mock<IDataContext>();

            //var dbMock2 = new Mock<IMongoDatabase>();

            //dbMock2
            //    .Setup(_ => _.GetCollection<Car>("Cars", It.IsAny<MongoCollectionSettings>()))
            //    .Returns(collectionMock);


            #endregion





            var client = _factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services => 
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(IDataContext));

                        services.Remove(descriptor);

                        services.AddScoped(typeof(IDataContext), services => mockIDataContext2.Object);

                        //var descriptor = services.SingleOrDefault(
                        //    d => d.ServiceType ==
                        //        typeof(IMongoDatabase));

                        //services.Remove(descriptor);

                        //services.AddScoped(typeof(IMongoDatabase), services => dbMock.Object);
                    }))
                .CreateClient();

            // Act
            //Car newCar = new Car
            //{
            //    Name = "SomeCar2",
            //    Description = "SomeDesc"
            //};
            var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(newCar);

            StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
            //var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);
            //var response = await client.GetAsync($"/api/car/{newCar.Id}");
            var response = await client.GetAsync($"/api/car/list");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();
            //Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            //Assert.Equal(newCar.Name, resultCar.Name);
            //Assert.Equal(newCar.Description, resultCar.Description);

            List<Car> resultCar = (List<Car>)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(List<Car>));
            Assert.Equal(newCar.Name, resultCar.FirstOrDefault()?.Name);
            Assert.Equal(newCar.Description, resultCar.FirstOrDefault()?.Description);
        }

        [Fact]
        public async Task AddCarTestAsync()
        {
            // Arange
            Car addCar = new Car
            {
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Mock<IDataContext> _mockContext = new Mock<IDataContext>();

            Mock<IMongoCollection<Car>> _mockCollection = new Mock<IMongoCollection<Car>>();
            _mockCollection.SetupAllProperties();

            // Storage
            List<Car> _carList = new List<Car>();

            // Mock
            Mock<IAsyncCursor<Car>> _carCursor = new Mock<IAsyncCursor<Car>>();
            _carCursor.Setup(_ => _.Current).Returns(_carList);
            _carCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _carCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            // AddCursor
            _mockCollection.Setup(op => op.FindSync<Car>(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_carCursor.Object);

            _mockCollection.Setup(op => op.InsertOne(It.IsAny<Car>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).Callback(() => _carList.Add(addCar));

            _mockCollection.Setup(op => op.InsertOneAsync(It.IsAny<Car>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).Callback(() => _carList.Add(addCar));

            // Mock GetCollection
            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(_mockCollection.Object).Verifiable();

            var client = this._factory
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
            var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(addCar);

            StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Single(_carList);
            Assert.Equal(addCar.Name, resultCar?.Name);
            Assert.Equal(addCar.Description, resultCar?.Description);
        }

        [Fact]
        public async Task PartialUpdateCarTestAsync()
        {
            // Arange
            Car existCar = new Car
            {
                Id = "dskgbjsdjgk",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Car updateCarInfo = new Car
            {
                Id = existCar.Id,
                Name = "ChangedCar",
            };

            Mock<IDataContext> _mockContext = new Mock<IDataContext>();

            Mock<IMongoCollection<Car>> _mockCollection = new Mock<IMongoCollection<Car>>();
            _mockCollection.SetupAllProperties();

            // Storage
            List<Car> _carList = new List<Car>();
            _carList.Add(existCar);

            // Mock
            Mock<IAsyncCursor<Car>> _carCursor = new Mock<IAsyncCursor<Car>>();
            _carCursor.Setup(_ => _.Current).Returns(_carList);
            _carCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _carCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            // AddCursor
            _mockCollection.Setup(op => op.FindSync<Car>(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_carCursor.Object);

            //_mockCollection.Setup(op => op.InsertOne(It.IsAny<Car>(),
            //    It.IsAny<InsertOneOptions>(),
            //    It.IsAny<CancellationToken>()));//.Callback(() => _carList.Add(addCar));

            //_mockCollection.Setup(op => op.InsertOneAsync(It.IsAny<Car>(),
            //    It.IsAny<InsertOneOptions>(),
            //    It.IsAny<CancellationToken>()));//.Callback(() => _carList.Add(addCar));

            // ReplaceOne
            Mock<ReplaceOneResult> mockReplaceOneResult = new Mock<ReplaceOneResult>();
            mockReplaceOneResult.Setup(r => r.IsAcknowledged).Returns(true);

            _mockCollection.Setup(op => op.ReplaceOne(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<Car>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(mockReplaceOneResult.Object);
            //.Callback(() => _carList.Add(existCar));

            _mockCollection.Setup(op => op.ReplaceOneAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<Car>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReplaceOneResult.Object);

            // Mock GetCollection
            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(_mockCollection.Object).Verifiable();

            var client = this._factory
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
            var carContent = Newtonsoft.Json.JsonConvert.SerializeObject(updateCarInfo);

            StringContent stringContent = new StringContent(carContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/car/CreateUpdate", stringContent);

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Single(_carList);
            Assert.Equal(updateCarInfo.Name, resultCar?.Name);
            Assert.Equal(existCar.Description, resultCar?.Description);
        }

        [Fact]
        public async Task GetCarListTestAsync()
        {
            // Arange
            Car existCar = new Car
            {
                Id = "45dsfdg",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Mock<IDataContext> _mockContext = new Mock<IDataContext>();

            Mock<IMongoCollection<Car>> _mockCollection = new Mock<IMongoCollection<Car>>();
            _mockCollection.SetupAllProperties();

            // Storage
            List<Car> _carList = new List<Car>();
            _carList.Add(existCar);

            // Mock
            Mock<IAsyncCursor<Car>> _carCursor = new Mock<IAsyncCursor<Car>>();
            _carCursor.Setup(_ => _.Current).Returns(_carList);
            _carCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _carCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            // AddCursor
            _mockCollection.Setup(op => op.FindSync<Car>(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_carCursor.Object);

            // Mock GetCollection
            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(_mockCollection.Object).Verifiable();

            var client = this._factory
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
            var response = await client.GetAsync($"/api/car/list");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            List<Car> resultCar = (List<Car>)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(List<Car>));
            Assert.Single(resultCar);
            Assert.Equal(existCar.Name, resultCar.FirstOrDefault()?.Name);
            Assert.Equal(existCar.Description, resultCar.FirstOrDefault()?.Description);
        }


        [Fact]
        public async Task GetCarByIdTestAsync()
        {
            // Arange
            Car newCar = new Car
            {
                Id = "dsgkhbdk",
                Name = "SomeCar2",
                Description = "SomeDesc"
            };

            Mock<IDataContext> _mockContext = new Mock<IDataContext>();

            Mock<IMongoCollection<Car>> _mockCollection = new Mock<IMongoCollection<Car>>();
            _mockCollection.SetupAllProperties();

            // Storage
            List<Car> _carList = new List<Car>();
            _carList.Add(newCar);

            // Mock
            Mock<IAsyncCursor<Car>> _carCursor = new Mock<IAsyncCursor<Car>>();
            _carCursor.Setup(_ => _.Current).Returns(_carList);
            _carCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _carCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            // AddCursor
            _mockCollection.Setup(op => op.FindSync<Car>(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).Returns(_carCursor.Object);

            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<Car>>(),
                It.IsAny<FindOptions<Car, Car>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_carCursor.Object);

            // Mock GetCollection
            Mock<IDataContext> mockIDataContext = new Mock<IDataContext>();
            mockIDataContext.Setup(c => c.Cars).Returns(_mockCollection.Object).Verifiable();

            var client = this._factory
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
            var response = await client.GetAsync($"/api/car/{newCar.Id}");

            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content?.ReadAsStringAsync();

            Car resultCar = (Car)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(Car));
            Assert.Equal(newCar.Name, resultCar?.Name);
            Assert.Equal(newCar.Description, resultCar?.Description);
        }
    }
}
