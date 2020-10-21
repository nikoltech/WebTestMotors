namespace WebTestMotors.Controllers
{
    using DataAccess.Entities;
    using DataAccess.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly IRepository repository;

        public CarController(IRepository repo)
        {
            this.repository = repo;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return this.Ok("Started!");
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetListAsync()
        {
            try
            {
                List<Car> list = await this.repository.GetCarListAsync().ConfigureAwait(false);

                return this.Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarAsync(string id)
        {
            try
            {
                Car car = await this.repository.GetCarAsync(id).ConfigureAwait(false);

                return this.Ok(car);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateUpdate")]
        public async Task<IActionResult> CreateUpdateCarAsync([FromBody]Car car)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                Car result = null;
                if (string.IsNullOrEmpty(car.Id))
                {
                    result = await this.repository.CreateCarAsync(car).ConfigureAwait(false);
                }
                else
                {
                    Car existCar = await this.repository.GetCarAsync(car.Id).ConfigureAwait(false);

                    JsonConvert.PopulateObject(JsonConvert.SerializeObject(car), existCar);

                    result = await this.repository.UpdateCarAsync(car).ConfigureAwait(false);
                }

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveCarAsync(string id)
        {
            try
            {
                bool result = await this.repository.RemoveCarAsync(id).ConfigureAwait(false);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
