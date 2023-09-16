using AutoMapper;
using CompagnyTools.AutoMapper;
using CompagnyTools.Interface;
using CompagnyTools.Services;
using DAL.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AuthSamples.ClaimsTransformer;

namespace CompagnyTools
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
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDbContext<EFCoreContext>();
            services.AddDbContext<Access>(
                options => options.UseNpgsql(Configuration["ConnectionStrings:ConnectionString"])
            );

            services.AddScoped<IOffice, OfficeService>();
            services.AddScoped<IAccount, AccountService>();
            services.AddScoped<ILogin, LoginService>();
            services.AddSession();
            services.AddControllersWithViews();

            MapperConfiguration mappingConfig = new(mc =>
            {
                mc.AddProfile(new Profiles());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Sets the default scheme to cookies
            .AddCookie(options => options.LoginPath = "/Login");

            // claims transformation is run after every Authenticate call
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseSession();

            // Must add this to resolve an issue when insert datetime fields
            // See  https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
