namespace WebTestMotors
{
    using DataAccess;
    using DataAccess.MongoDb;
    using DataAccess.Repositories;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // requires using Microsoft.Extensions.Options
            services.Configure<MongoDbSettings>(
                Configuration.GetSection(nameof(MongoDbSettings)));

            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            //services.AddScoped<IMongoDatabase>(sp => {
            //    var dbParams = sp.GetRequiredService<IMongoDbSettings>();
            //    var client = new MongoClient(dbParams.ConnectionString);
            //    return client.GetDatabase(dbParams.DatabaseName);
            //});

            // Resolve dependencies
            services.AddScoped<IMongoDatabaseFactory, MongoDatabaseFactory>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<IGlobalRepository, GlobalRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Car}/{action=Index}/{id?}");
            });
        }
    }
}
