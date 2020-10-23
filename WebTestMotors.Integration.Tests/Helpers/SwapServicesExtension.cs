namespace WebTestMotors.Integration.Tests.Helpers
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class SwapServicesExtension
    {
        public static void SwapService<TIService, TService>(this IServiceCollection services, TService newService, ServiceLifetime serviceLifetime)
        {
            var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(TIService) && d.Lifetime == ServiceLifetime.Scoped);

            services.Remove(descriptor);

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.AddSingleton(typeof(TIService), services => newService);
                        break;
                    }

                case ServiceLifetime.Scoped:
                    {
                        services.AddScoped(typeof(TIService), services => newService);
                        break;
                    }

                case ServiceLifetime.Transient:
                    {
                        services.AddTransient(typeof(TIService), services => newService);
                        break;
                    }

                default:
                    break;
            }
        }

        public static void SwapService<TService>(this IServiceCollection services, TService newService, ServiceLifetime serviceLifetime)
        {
            var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(TService) && d.Lifetime == ServiceLifetime.Scoped);

            services.Remove(descriptor);

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.AddSingleton(typeof(TService), services => newService);
                        break;
                    }

                case ServiceLifetime.Scoped:
                    {
                        services.AddScoped(typeof(TService), services => newService);
                        break;
                    }

                case ServiceLifetime.Transient:
                    {
                        services.AddTransient(typeof(TService), services => newService);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
