using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Context
{
    public partial class Access : EFCoreContext
    {
        private readonly DbContextOptionsBuilder OptionBuilder;
        public Access(DbContextOptions<EFCoreContext> options)
        : base(options)
            {
                OptionBuilder = new DbContextOptionsBuilder();
            }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string projectDir = Directory.GetCurrentDirectory();
                string configPath = Path.Combine(projectDir, "appsettings.json");
                string configPathDev = Path.Combine(projectDir, "appsettings.Development.json");

                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(configPath, optional: false, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();

                string connectionString = configuration.GetSection("ConnectionStrings:ConnectionString").Value;

                if (string.IsNullOrEmpty(connectionString))
                {
                    string msg = "Error found. Check settings file";
                    Console.WriteLine(msg);
                    throw new System.Exception(msg);
                }
                else
                {
                    optionsBuilder.UseNpgsql(
                        connectionString,
                        options => options.EnableRetryOnFailure()
                    );
                }
            }
        }
    }
}
