using System.Transactions;
using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Data;
using UserAuthAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
var hangconn = builder.Configuration.GetConnectionString("HangfireString");
builder.Services.AddDbContext<UserAuthContext>(option => option.UseMySql(conn, ServerVersion.AutoDetect(conn)));
builder.Services.AddScoped<UserAuthService>();
builder.Services.AddScoped<BackgroundJobClient>();

builder.Services.AddHangfire(
    config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(hangconn,
    new MySqlStorageOptions
    {
        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        QueuePollInterval = TimeSpan.FromSeconds(15),
        JobExpirationCheckInterval = TimeSpan.FromHours(1),
        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        PrepareSchemaIfNecessary = true,
        DashboardJobListLimit = 50000,
        TransactionTimeout = TimeSpan.FromMinutes(1),
        TablesPrefix = "Hangfire"
    })));


builder.Services.AddHangfireServer();
builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => @"UserAuth API. Navigate to /swagger to open the Swagger test UI.");
app.MapHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();