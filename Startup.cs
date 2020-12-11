using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pixel.Core.Auth;
using Microsoft.Extensions.Hosting;
 
namespace Pixel.IRIS5.API.Mobile
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
            //services.AddTransient<IApplicationEventWorkflowHelper, ApplicationEventWorkflowHelper>();
           // services.AddTransient<IWorkflowDBService, WorkflowDBService>();
           // services.AddTransient<IEmailProvider, EmailProvider>();
            //services.AddTransient<ILanguagePickerManager, LanguagePickerManager>();
           // services.AddTransient<INotificationManager, NotificationManager>(); // for push notification
            services.AddScoped<IAuthenticationProcess, AuthenticationProcess>();
            services.AddSingleton(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                var keyByteArray = Encoding.ASCII.GetBytes(Configuration["SigningKey"]);
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyByteArray),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var cors_array = Configuration["ConnectionStrings:SpecificCorsOrigins"].Split(",");

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins(cors_array)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
               );
            });

            //adding cors globally using MVC filter
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //TODO: options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
                //}).AddJsonOptions(o =>
                //{
                //    // ADDED BY TONY ON 30-04-2019 to solve the issue of date value sent to API -1 day
                //    //o.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
                //})
                //.AddJsonOptions(options =>
                //{
                //    // ADDEAD BY TONY ON 25-05-2019 to solve the issue of get date from API including time (hours)
                //    //TODO: options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
            }).AddNewtonsoftJson();

            services.AddControllers().AddNewtonsoftJson(o =>
            {
                //To solve the issue of date value sent to API -1 day
                o.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;

                //To solve the issue of get date from API including time(hours)
                o.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
            });

            services.AddControllers().AddNewtonsoftJson();

            // using newtonsoft intead of System.Text.Json
            services.AddSignalR().AddNewtonsoftJsonProtocol();

            services.AddControllers();

            Pixel.IRIS5.Shared.Helper.SetGlobalVariables(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // to be added in order
            //app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
               // endpoints.MapHub<NotificationHub>("/notifications");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "api/{controller}/{action}");
            });
        }
    }
}
