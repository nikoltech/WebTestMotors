namespace WebTestMotors.DataAccess.Repositories
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Repository : IRepository
    {
        private readonly DataContext context;

        public Repository(DataContext context)
        {
            this.context = context;
        }

        // TODO: page, count
        public async Task<List<Car>> GetCarListAsync()
        {
            List<Car> carList = await this.context.Cars.Find(c => true).ToListAsync();

            return carList;
        }

        public async Task<Car> GetCarAsync(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            Car car = await this.context.Cars.Find<Car>(c => c.Id.Equals(id)).FirstOrDefaultAsync();

            if (car == null)
            {
                throw new Exception($"Car with id {id} not found.");
            }

            return car;
        }

        public async Task<Car> CreateCarAsync(Car entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            Car existCar = await this.context.Cars.Find<Car>(c => c.Id.Equals(entity.Id)).FirstOrDefaultAsync();
            if (existCar != null)
            {
                throw new Exception($"Car with id {entity.Id} already exists.");
            }

            entity.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            await this.context.Cars.InsertOneAsync(entity);

            return entity;
        }

        public async Task<Car> UpdateCarAsync(Car entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            Car existCar = await this.context.Cars.Find(c => c.Id.Equals(entity.Id)).FirstOrDefaultAsync();
            if (existCar == null)
            {
                throw new Exception($"Car with id {entity.Id} does not exists.");
            }

            ReplaceOneResult result = await this.context.Cars.ReplaceOneAsync(c => c.Id == entity.Id, entity);

            if (!result.IsAcknowledged)
            {
                throw new Exception("Something got wrong in updating record!");
            }

            return entity;
        }

        public async Task<bool> RemoveCarAsync(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            Car car = await this.context.Cars.Find(c => c.Id.Equals(id)).FirstOrDefaultAsync();
            if (car == null)
            {
                throw new Exception($"Car with id {id} does not exists.");
            }

            DeleteResult result = await this.context.Cars.DeleteOneAsync(c => c.Id == id);

            return result.DeletedCount == 1;
        }
    }
}
