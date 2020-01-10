using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.RabbitMQ;
using RabbitMQ.Client;

namespace NotificationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Being called by runtime
        // Used to add services to the controller
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // configuration located in appsettings.json
            AppDb db = new AppDb(Configuration["ConnectionStrings:DefaultConnection"]);
            services.AddTransient<AppDb>(_ => db);

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "rabbitmq"
                };

                return new RabbitMQPersistentConnection(factory, db);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseRabbitListener();
        }
    }

    // Nested class for initiallizing RabbitMQ on startup
     public static class ApplicationBuilderExtentions
    {
        public static IRabbitMQPersistentConnection Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<IRabbitMQPersistentConnection>();
            var life = app.ApplicationServices.GetService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);

            life.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted()
        {
            Listener.CreateConsumerChannel("event.add");
            Listener.CreateConsumerChannel("event.update");
        }

        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}
