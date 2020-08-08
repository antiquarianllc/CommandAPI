using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandAPI
{
    using CommandAPI.Models;
    using Npgsql;

    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new NpgsqlConnectionStringBuilder();
            builder.ConnectionString = Configuration.GetConnectionString("PostgreSqlConnection");
            builder.Username = Configuration["UserID"];
            builder.Password = Configuration["Password"];

            services.AddDbContext<CommandContext>(opt => opt.UseNpgsql(builder.ConnectionString));

            // Define service hook to use of Azure Active Directory for authentication
            services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
                .AddJwtBearer( opt =>
                {
                    opt.Audience = Configuration[ "ResoureId" ];
                    opt.Authority = $"{Configuration[ "InstanceId" ]}{Configuration[ "TenentId" ]}";
                });
            
            services.AddControllers( );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CommandContext context )
        {

            context.Database.Migrate( );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Add authentication and authorization to pipeline.
            app.UseAuthentication( );
            app.UseAuthorization( );

            app.UseEndpoints(endpoints =>
            {
                /*
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                */
                endpoints.MapControllers( );
            });
        }
    }
}
