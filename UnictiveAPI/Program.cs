using HelperClasses;
using Repository;
using RepositoryInterface;
using Service;
using ServiceInterface;
using System.Data.Common;
using Utility;
using UtilityInterface;

namespace UnictiveAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Setup token security key for JWT
            ApplicationSettings.TokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;

            //Setup connection for database
            ApplicationSettings.connectionString = builder.Configuration.GetConnectionString("connstring");

            //Setup dependency injetion
            builder.Services.AddScoped<IHobbyRepository, HobbyRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPhoneValidation, PhoneValidation>();


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}