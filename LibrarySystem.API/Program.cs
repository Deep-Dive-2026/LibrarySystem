using LibrarySystem.API.Extensions;
using LibrarySystem.Application.Commands.Books.CreateBook;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Application.Services;
using LibrarySystem.Infrastructure.Persistence;
using LibrarySystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Library")));

builder.Services.AddSwaggerConfiguration();
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(
        typeof(CreateBookCommand).Assembly));




builder.Services.AddSignalR();



builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
        });
});



//IMemory Cache
builder.Services.AddMemoryCache();


//Redis Cache
builder.Services.AddStackExchangeRedisCache(
    options =>
    {
        //options.Configuration = "localhost:6379";
        options.Configuration = "127.0.0.1:6379";
        options.InstanceName = "library:";
    });




// Hybrid Cache
builder.Services.AddHybridCache();


//Output Cache
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Books", policy =>
        policy.Expire(TimeSpan.FromSeconds(60))
              .Tag("books"));
});



// Response Cache
builder.Services.AddResponseCaching();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    try
    {
        await db.Database.MigrateAsync();
        await DataSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Log.Error(ex,
            "Could not migrate/seed the database. " +
            "Is the SQL Server container running? (docker start library-sql)");
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add Output Caching pipline
app.UseOutputCache();

// Add Response Caching pipline
app.UseResponseCaching();
app.MapHub<LibrarySystem.API.Hubs.ChatHub>("message");

app.MapControllers();

app.Run();
