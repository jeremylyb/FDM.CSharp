
using AcmeCorp.Shopper.CartsRestAPI.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace AcmeCorp.Shopper.CartsRestAPI
{

    /*
        Step1 - Download all the packages
        Step2 - Configure appsettings.json
              "ConnectionStrings": {
                 "CRUDConnection": "Data Source=DESKTOP-DQUSH87; Initial Catalog=CartsAcme; Integrated Security=sspi; TrustServerCertificate=True;"
              }
        step3 - build context using scaffolding method by using package manager console
                : Scaffold-DbContext 'Data Source=DESKTOP-DQUSH87; Initial Catalog=CartsAcme; Integrated Security=sspi; TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
                The classes based on the database construct will be created in our models folder
        Step4: Configure ASP.NET Core such that it allow context to be passed through dependency injection
                This is done through:
                    1. remove no arg constructor in context class
                    2. Comment out the optionsBuilder.useSqlServer  => we will be using appsettings.json instead
                        it will be based on the ConnectionString JSON defined
                    3. Configure our builder by AddDbContext to service
   
   
 */
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AcmeCorp.Shopper.CartsRestAPI.Models.CartsAcmeContext>(
            options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CRUDConnectionCart"));
            });

            builder.Services.AddDbContext<AcmeCorp.Shopper.CartsRestAPI.Models.ProductsAcmeContext>(
            options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CRUDConnectionProduct"));
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // To handle cyclical referencing due to bidirectional relationship between Cart and CartItems
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                //options.JsonSerializerOptions.MaxDepth = 32;
                // Other serialization options...

                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        
            });

            var app = builder.Build();

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
