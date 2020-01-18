using TSensor.Web.Models.Services.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using TSensor.Web.Models.Broadcast;
using TSensor.Web.Models.Repository;

namespace TSensor.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = new PathString("/login");
                    opts.AccessDeniedPath = new PathString("/youshallnotpass");
                    //opts.EventsType = typeof(UpdateAuthenticationEvents);
                });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //services.AddControllersWithViews();

            services.AddSignalR();

            services.AddSingleton<AuthService>();

            var connectionString = Configuration.GetConnectionString("oltp");
            services.AddScoped<IRepository, Repository>(p => new Repository(connectionString));
            services.AddScoped<IUserRepository, UserRepository>(p => new UserRepository(connectionString));
            services.AddSingleton<IBroadcastRepository, BroadcastRepository>(p => new BroadcastRepository(connectionString));

            services.AddHostedService<BroadcastService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BroadcastHub>("broadcast");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Default}/{id?}");
            });

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/npm",
                EnableDirectoryBrowsing = false
            });
        }
    }
}
