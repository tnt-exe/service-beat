using Hangfire;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

var apiUrl = builder.Configuration["Api"];
var hangfireTitle = builder.Configuration["Hangfire:Title"];
var hangfireUser = builder.Configuration["Hangfire:User"];
var hangfirePass = builder.Configuration["Hangfire:Pass"];
var dbStore = builder.Configuration.GetConnectionString("DB_STORE");

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(dbStore),
        new PostgreSqlStorageOptions
        {
            JobExpirationCheckInterval = TimeSpan.FromDays(7)
        });
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DarkModeEnabled = true,
    DashboardTitle = hangfireTitle,
    DisplayStorageConnectionString = false,
    Authorization = 
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = hangfireUser,
            Pass = hangfirePass
        }
    ]
});

RecurringJob.AddOrUpdate(
    "Run every 10 minutes",
    () => new HttpClient().GetAsync(apiUrl),
    "*/10 * * * *"
);

app.Run();
