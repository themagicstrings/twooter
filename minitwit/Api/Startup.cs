using System.Threading;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Models;
using Prometheus;

namespace Api
{
  public class Startup
    {

        public static readonly Gauge Uptime = Metrics.CreateGauge("Minitwit_uptime", "Current uptime for the system");
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            new Thread(() => {
                while(true) {
                    Uptime.Inc();
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbpassword = System.Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbip = System.Environment.GetEnvironmentVariable("DB_IP");
            var connectionString = $"Server={dbip};Database=Minitwit;Trusted_Connection=True;Integrated Security=false;User Id=SA;Password={dbpassword}";
            services.AddDbContext<MinitwitContext>(o => o.UseInMemoryDatabase("da"));
            //services.AddDbContext<MinitwitContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<IMinitwitContext, MinitwitContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddControllers( o => o.SuppressAsyncSuffixInActionNames = false);
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "session";
                options.Cookie.IsEssential = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<MinitwitContext>();
                //context.Database.Migrate();
            }

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}
