using APRP.Services.AuthorityAPI.Persistence.DbContexts;
using APRP.Services.AuthorityAPI.Persistence.Repositories;
using APRP.Services.AuthorityAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

//set logging DI
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Serilog configuration
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

// Add Framework Provided services to the container.
#region Framework Provided Services
var services = builder.Services;

//set CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
        b.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});

#region Database
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));
services.AddHttpContextAccessor();
#endregion


services.AddSingleton<IUriService>(o =>
{
    var accessor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});

services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
#endregion

// Add Custom Provided services to the container.
#region Custom Services
//Custom Repositories
services.AddScoped<IAuthorityRepository, AuthorityRepository>();

//Custom services
services.AddScoped<IUnitOfWork, UnitOfWork>();
//services.AddScoped<IUriService, UriService>();
services.AddScoped<IAuthorityService, AuthorityService>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // allows any client from anywhere to access

app.UseAuthorization();

app.MapControllers();

app.Run();
