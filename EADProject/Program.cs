/***************************************************************
 * File Name: Program.cs
 * Description: Entry point for the EAD project. This file configures
 *              the services, middleware, and MongoDB connection settings 
 *              required for the application to run.
 * Date Created: September 29, 2024
 * Notes: The application uses MongoDB for data storage and configures
 *        necessary services like CORS and Swagger for development.
 ***************************************************************/
using System.Text;
using System.Text.Json.Serialization; 
using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure MongoDB settings from the appsettings.json configuration section.
        builder.Services.Configure<MongoDBSettings>(
            builder.Configuration.GetSection("MongoDBSettings"));

        // Register MongoClient as a singleton to manage the MongoDB connection.
        builder.Services.AddSingleton<IMongoClient>(s =>
            new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));

        // Register application services like UserService, ProductService, OrderService, and CategoryService.
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<CategoryService>();
        builder.Services.AddScoped<RatingService>();
        builder.Services.AddScoped<VendorNotificationService>();
        builder.Services.AddScoped<AdminNotificationService>();
        builder.Services.AddScoped<CustomerNotificationService>();
        builder.Services.AddScoped<JwtTokenHelper>();

        // Add controllers to handle HTTP requests and responses.
        //builder.Services.AddControllers();

        // Add controllers with JSON options
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            // Add enum converter to serialize enums as strings
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer(); // Add API explorer for endpoint discovery.
        builder.Services.AddSwaggerGen(); // Add Swagger for API documentation (if in development).

        // Add CORS policy to allow cross-origin requests from any origin.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        

        //  JWT authentication
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var key = Encoding.ASCII.GetBytes(secretKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
               
            };
        });

        var app = builder.Build();

        // Apply CORS policy to allow requests from all origins.
        app.UseCors("AllowAllOrigins");

        app.UseAuthentication();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Enable Swagger UI only in development for API testing and documentation.
            //app.UseSwagger();
            //app.UseSwaggerUI();
        }

        // Use authorization middleware to protect API endpoints.
        app.UseAuthorization();

        // Map controllers to handle API routes.
        app.MapControllers();

        // Run the application.
        app.Run();
    }
}
