//using Microsoft.Extensions.Options;
//using MongoDB.Driver;
//using EADProject.Models;
//using EADProject.Services;

//namespace EAD
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Configure MongoDB settings
//            builder.Services.Configure<MongoDBSettings>(
//                builder.Configuration.GetSection("MongoDBSettings"));

//            // Register MongoClient as a singleton
//            builder.Services.AddSingleton<IMongoClient>(s =>
//                new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));

//            // Register UserService or other services
//            builder.Services.AddScoped<UserService>();
//            builder.Services.AddScoped<ProductService>();
//            builder.Services.AddScoped<OrderService>();

//            // Add services to the container.
//            builder.Services.AddControllers();
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                //app.UseSwagger();
//                //app.UseSwaggerUI();
//            }

//            //app.UseHttpsRedirection();
//            app.UseAuthorization();
//            app.MapControllers();

//            app.Run();
//        }
//    }
//}
using EADProject.Models;
using EADProject.Services;
using MongoDB.Driver;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure MongoDB settings
        builder.Services.Configure<MongoDBSettings>(
            builder.Configuration.GetSection("MongoDBSettings"));

        // Register MongoClient as a singleton
        builder.Services.AddSingleton<IMongoClient>(s =>
            new MongoClient(builder.Configuration.GetValue<string>("MongoDBSettings:ConnectionString")));

        // Register services
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<CategoryService>();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        var app = builder.Build();

        // Use CORS
        app.UseCors("AllowAllOrigins");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseSwagger();
            //app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
