using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WdtAsrA1.Controller;
using WdtAsrA1.Utils;

namespace WdtAsrA1
{
    /// <summary>
    ///  static only. no instance possible
    /// </summary>
    public static class Program
    {
        private static readonly Lazy<IConfiguration> _configuration;
        private static IConfiguration Configuration => _configuration.Value;
        private static readonly Lazy<string> _connectionString;
        internal static string ConnectionString => _connectionString.Value;

        /// <summary>
        /// amount of lines for pagination
        /// </summary>
        private static readonly Lazy<int> _fetchLines;
        public static int FetchLines => _fetchLines.Value;
        
        /// <summary>
        /// pre set slot duration
        /// </summary>
        private static readonly Lazy<int> _slotDuration;
        public static int SlotDuration => _slotDuration.Value;
        
        /// <summary>
        /// room can be booked specified times per day
        /// </summary>
        private static readonly Lazy<int> _dailyStaffBookings;
        public static int DailyStaffBookings => _dailyStaffBookings.Value;
        
        /// <summary>
        /// staff can have specified slots per day
        /// </summary>
        private static readonly Lazy<int> _dailyRoomBookings;
        public static int DailyRoomBookings => _dailyRoomBookings.Value;
        
        /// <summary>
        /// School open Hours
        /// </summary>
        private static readonly Lazy<int> _workingHoursStart;
        public static int WorkingHoursStart => _workingHoursStart.Value;
        private static readonly Lazy<int> _workingHoursEnd;
        public static int WorkingHoursEnd => _workingHoursEnd.Value;
        
        
        /// <summary>
        /// per specs login not required
        /// </summary>
        public static bool PrototypeMode { get; } = true;

        static Program()
        {
            // fetch current environment
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            _fetchLines = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("FetchLines"));
            
            _slotDuration = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("SlotDuration"));
            
            _dailyRoomBookings = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("DailyRoomBookings"));
            _dailyStaffBookings = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("DailyStaffBookings"));
            _workingHoursStart = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("WorkingHoursStart"));
            
            _workingHoursEnd = new Lazy<int>(() =>
                Configuration.GetSection("GenericSettings").GetValue<int>("WorkingHoursEnd"));

            _configuration = new Lazy<IConfiguration>(() =>
            {
                //Determines the working environment as IHostingEnvironment is unavailable in a console app

                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                // if development environment
                if (environmentName == "Development")
                {
                    //only add secrets in development
                    builder
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets<AsrDb>();
                }

                return builder.Build();
            });
            _connectionString = new Lazy<string>(() =>
            {
                string connectionString;
                // if development environment
                if (environmentName == "Development")
                {
                    var secrets = Configuration.GetSection(nameof(AsrDb)).Get<AsrDb>();
                    var sqlString = new SqlConnectionStringBuilder(Configuration.GetConnectionString("AsrDevDB"))
                    {
                        UserID = secrets.Uid,
                        Password = secrets.Password
                    };
                    connectionString = sqlString.ConnectionString;
                }
                else
                {
                    connectionString = Configuration.GetConnectionString("AsrDB");
                }

                return connectionString;
            });
        }


        static void Main(string[] args)
        {
            Console.Clear();
            BaseController login = new MainMenuController();
            login.Start();
        }
    }
}