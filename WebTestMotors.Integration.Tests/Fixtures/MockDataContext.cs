namespace WebTestMotors.Integration.Tests.Fixtures
{
    using MongoDB.Driver;
    using Moq;
    using WebTestMotors.DataAccess;
    using WebTestMotors.DataAccess.Entities;

    public class MockDataContext : IDataContext
    {
        public IMongoCollection<Car> Cars => new Mock<IMongoCollection<Car>>().Object;
    }
}
