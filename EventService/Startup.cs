using EventService.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventService
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
            services.AddControllers();

            services.AddTransient<AppDb>(s => new AppDb(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "rabbitmq"
                };
                
                AppDb db = new AppDb(Configuration["ConnectionStrings:DefaultConnection"]);
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

    public static class ApplicationBuilderExtentions
    {
        public static IRabbitMQPersistentConnection Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<IRabbitMQPersistentConnection>();
            var life = app.ApplicationServices.GetService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            life.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted()
        {
            Listener.CreateConsumerChannel();
            Listener.CreateRPCChannel();
        }

        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}
