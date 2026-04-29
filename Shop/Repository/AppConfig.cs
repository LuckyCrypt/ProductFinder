namespace CustomDeloAPI.Repository
{
    public static class AppConfig
    {
        private static readonly IConfiguration _config;

        static AppConfig()
        {
            // Этот код выполнится один раз при первом обращении к классу
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Удобные свойства для доступа к данным
        public static string VisaName => _config["NameVisa"];
        public static string PortName => _config["NameProtocol"];
       
    }
}
