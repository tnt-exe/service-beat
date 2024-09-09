using Hangfire;
using Hangfire.Redis.StackExchange;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

var apiUrl = builder.Configuration["Api"];
var hangfireTitle = builder.Configuration["Hangfire:Title"];
var hangfireUser = builder.Configuration["Hangfire:User"];
var hangfirePass = builder.Configuration["Hangfire:Pass"];
var hangfireRedis = builder.Configuration["Hangfire:Redis"];

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHangfire(config =>
{
    config.UseRedisStorage(hangfireRedis);
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
