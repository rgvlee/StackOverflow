using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Question59741796 {
    //public class Helper : IHelpers
    //{
    //    private IConfiguration configuration;
    //    private readonly ILogger<Helper> logger;
    //    public Helper(IConfiguration _config, ILogger<Helper> _logger)
    //    {
    //        configuration = _config;
    //        logger = _logger;
    //    }
    //    public Tuple<string, string> SpotifyClientInformation()
    //    {
    //        Tuple<string, string> tuple = null;
    //        try
    //        {
    //            if (configuration != null)
    //            {
    //                string clientID = configuration["SpotifySecrets:clientID"];
    //                //Todo move secretID to more secure location
    //                string secretID = configuration["SpotifySecrets:secretID"];
    //                tuple = new Tuple<string, string>(clientID, secretID);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogCritical("Configuration DI not set up correctly", ex);
    //        }

    //        return tuple;
    //    }
    //}

    public class Helper : IHelpers
    {
        private IConfiguration configuration;
        private readonly ILogger<Helper> logger;
        public Helper(IConfiguration _config, ILogger<Helper> _logger)
        {
            configuration = _config;
            logger = _logger;

            if (configuration == null)
            {
                logger.LogCritical("Configuration DI not set up correctly");
            }
        }
        public Tuple<string, string> SpotifyClientInformation()
        {
            if (configuration == null)
            {
                return null;
            }

            string clientID = configuration["SpotifySecrets:clientID"];
            //Todo move secretID to more secure location
            string secretID = configuration["SpotifySecrets:secretID"];
            return new Tuple<string, string>(clientID, secretID);
        }
    }
}