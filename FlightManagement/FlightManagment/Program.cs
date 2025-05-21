using System.Net;
using System.Text.Json;
using FlightManagement.Common.Constant;
using FlightManagement.Services.ApiServices;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container  
ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware  
ConfigureMiddleware(app);

// Map endpoints  
app.MapControllers();

app.Run();

// Method to configure services  
void ConfigureServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddScoped<IFlightDataService, FlightDataService>();
    services.AddScoped<IFlightScheduleCheckerService, FlightScheduleCheckerService>();
    services.AddControllers();

    // Add Authentication   todo: if needed

    // Add Authorization    todo: if needed
}

// Method to configure middleware  
void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var errorResponse = new
                {
                    Message = AppConstants.Message.UnexpectedError,
                    Detail = exceptionHandlerPathFeature?.Error.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            });
        });
    }
}
