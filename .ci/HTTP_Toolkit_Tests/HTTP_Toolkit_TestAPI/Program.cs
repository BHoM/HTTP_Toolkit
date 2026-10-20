using BHoM.ZeroCode.Extensions.Formatters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Add(new BHoMInputFormatter());
    options.OutputFormatters.Add(new BHoMOutputFormatter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
