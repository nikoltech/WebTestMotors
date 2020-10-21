namespace WebTestMotors.DataAccess
{
    using MongoDB.Driver;
    using WebTestMotors.DataAccess.Entities;

    public interface IDataContext
    {
        IMongoCollection<Car> Cars { get; }
    }
}
