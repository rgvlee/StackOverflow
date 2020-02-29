using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Question59561669 {
    public static class Extensions
    {
        public static void AddCustomersConfiguration(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            logger.LogDebug($"Entering {nameof(AddCustomersConfiguration)} method.");
            var customersSection = configuration.GetSection("Customers");
            if (!customersSection.Exists())
            {
                logger.LogError($"There is no Customers section in configuration.");
                return;
            }
        }
    }
}