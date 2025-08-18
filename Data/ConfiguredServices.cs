using bmhAPI.Services;

namespace bmhAPI
{
    public static class ConfiguredServices
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<DBExecutor>();
            services.AddScoped<DatabaseService>();
            services.AddScoped<DBNonQueryExecutor>();
        }
    }
}
