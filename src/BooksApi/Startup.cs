using EntityFrameworkCore.UnitOfWork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BooksApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region Mongo

            services.Configure<Models.Mongo.BookstoreDatabaseSettings>(Configuration.GetSection(nameof(Models.Mongo.BookstoreDatabaseSettings)));
            services.AddSingleton<Models.Mongo.IBookstoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<Models.Mongo.BookstoreDatabaseSettings>>().Value);
            services.AddScoped<Services.Mongo.IBookService, Services.Mongo.BookService>();

            #endregion Mongo

            #region Postgres

            var postgresConnectionString = Configuration.GetConnectionString("PostgreSQLEFCoreDataAccess");

            services.AddDbContext<Models.Postgres.Context>(options =>
            {
                options.UseNpgsql(postgresConnectionString, NpgsqlOptions =>
                {
                    var assembly = typeof(Models.Postgres.Context).Assembly;
                    var assemblyName = assembly.GetName();

                    NpgsqlOptions.MigrationsAssembly(assemblyName.Name);
                });

                options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            });

            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

            services.AddScoped<DbContext, Models.Postgres.Context>();
            services.AddUnitOfWork<Models.Postgres.Context>();
            services.AddScoped<Services.Postgres.IBookService, Services.Postgres.BookService>();

            #endregion Postgres

            // #region MySQL

            // var mySqlConnectionString = Configuration.GetConnectionString("MySQLEFCoreDataAccess");

            // services.AddDbContext<Models.MySQL.Context>(options =>
            // {
            //     options.UseMySql(mySqlConnectionString, mySqlOptions =>
            //     {
            //         var assembly = typeof(Models.MySQL.Context).Assembly;
            //         var assemblyName = assembly.GetName();

            //         mySqlOptions.MigrationsAssembly(assemblyName.Name);
            //     });

            //     options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            // });

            // services.AddScoped<DbContext, Models.MySQL.Context>();
            // services.AddUnitOfWork<Models.MySQL.Context>();
            // services.AddScoped<Services.MySQL.IBookService, Services.MySQL.BookService>();

            // #endregion MySQL

            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                // PostgreSQL
                var context = serviceScope.ServiceProvider.GetService<Models.Postgres.Context>();

                // MySQL
                // var context = serviceScope.ServiceProvider.GetService<Models.MySQL.Context>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
