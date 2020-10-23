namespace WebTestMotors.DataAccess
{
    using WebTestMotors.DataAccess.Entities;
    using WebTestMotors.DataAccess.Repositories;

    public interface IDataContext
    {
        public IRepository<Car> Cars { get; }
    }
}
