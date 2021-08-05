namespace WebTestMotors.DataAccess.Repositories
{
    using WebTestMotors.DataAccess.Entities;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    public class GlobalRepository : IGlobalRepository
    {
        private readonly IDataContext context;

        public GlobalRepository(IDataContext context)
        {
            this.context = context;
        }

        // TODO: page, count
        public async Task<List<Car>> GetCarListAsync()
        {
            List<Car> carList = await this.context.Cars.AsQueryable().ToListAsync();
            //List<Car> carList = await this.context.Cars.FindAsync(c => true);

            return carList;
        }

        public async Task<Car> GetCarAsync(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            Car car = (await this.context.Cars.FindAsync(c => c.Id.Equals(id))).FirstOrDefault();

            if (car == null)
            {
                throw new Exception($"Car with id {id} not found.");
            }

            return car;
        }

        public async Task<Car> CreateCarAsync(Car entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            Car existCar = null;
            if (!string.IsNullOrEmpty(entity.Id))
            {
                existCar = (await this.context.Cars.FindAsync(c => c.Id.Equals(entity.Id))).FirstOrDefault();
            }
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

            Car existCar = (await this.context.Cars.FindAsync(c => c.Id.Equals(entity.Id))).FirstOrDefault();
            if (existCar == null)
            {
                throw new Exception($"Car with id {entity.Id} does not exists.");
            }

            //ReplaceOneResult result = await this.context.Cars.ReplaceOneAsync(c => c.Id == entity.Id, entity);
            Car result = await this.context.Cars.FindOneAndReplaceAsync(c => c.Id == entity.Id, entity);

            return entity;
        }

        public async Task<bool> RemoveCarAsync(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            Car car = (await this.context.Cars.FindAsync(c => c.Id.Equals(id))).FirstOrDefault();
            if (car == null)
            {
                throw new Exception($"Car with id {id} does not exists.");
            }

            DeleteResult result = await this.context.Cars.DeleteOneAsync(c => c.Id == id);

            return result.DeletedCount == 1;
        }
    }
}
