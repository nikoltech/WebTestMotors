namespace WebTestMotors.DataAccess.Repositories
{
    using WebTestMotors.DataAccess.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task<List<Car>> GetCarListAsync();

        Task<Car> GetCarAsync(string id);

        Task<Car> CreateCarAsync(Car entity);

        Task<Car> UpdateCarAsync(Car entity);

        Task<bool> RemoveCarAsync(string id);
    }
}
