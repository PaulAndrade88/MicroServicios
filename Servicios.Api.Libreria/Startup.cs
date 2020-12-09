using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Servicios.Api.Libreria.Core;
using Servicios.Api.Libreria.Core.ContextMongoDB;
using Servicios.Api.Libreria.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.Api.Libreria
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoSettings>(
                options =>
                {
                    options.ConnectionString = Configuration.GetSection("MongoDb:ConnectionString").Value;
                    options.Database = Configuration.GetSection("MongoDb:Database").Value;
                });

            //Si existe activamente una instancia sigue trabajando con ella si no es asi la crea.
            services.AddSingleton<MongoSettings>(); 

            //Dependency Inyection - AddTrasient
            // Cuando queremos que nuestra clase inyectada genere instancias por cada metodo individual que se va ejecutando
            // En otras palabras, creando nuevas instancias cada vez que un cliente necesita ejecutar un API
            // periodos cortos de instancia por transaccion, es lo necesario para trabajar con mongo db.
            services.AddTransient<IAutorContext, AutorContext>();
            services.AddTransient<IAutorRepository, AutorRepository>();

            //AddScoped se inicializa en el request y se destruye en el response.
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Servicios.Api.Libreria", Version = "v1" });
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsRule", rule =>
                {
                    //rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://mylocalpage.com"); 

                    //El * nos permite hacer nuestra app accesible/publica
                    rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsRule");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
