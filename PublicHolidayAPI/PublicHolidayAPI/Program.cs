using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PublicHolidayAPI.Controllers;
using PublicHolidayAPI.Database;
using PublicHolidayAPI.Interfaces;
using PublicHolidayAPI.Services;

namespace PublicHolidayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublicHolidayAPI", Version = "v1" });
            });

            builder.Services.AddDbContext<HolidayDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHttpClient<IKayaposoftApiService,KayaposoftApiService>();
            builder.Services.AddScoped<HolidayController>();

            var app = builder.Build();
            app.UseCors("AllowAllOrigins");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
