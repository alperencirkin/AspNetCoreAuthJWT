using AspNetCoreAuthJWT.Helpers;
using AspNetCoreAuthJWT.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreAuthJWT
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<Appsettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<Appsettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                //Gelen isteklerin sadece HTTPS yani SSL sertifikasý olanlarý kabul etmesi(varsayýlan true)
                x.RequireHttpsMetadata = false;
                //Eðer token onaylanmýþ ise sunucu tarafýnda kayýt edilir.
                x.SaveToken = true;
                //Token içinde neleri kontrol edeceðimizin ayarlarý.
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //Token 3.kýsým(imza) kontrolü
                    ValidateIssuerSigningKey = true,
                    //Neyle kontrol etmesi gerektigi
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //Bu iki ayar ise "aud" ve "iss" claimlerini kontrol edelim mi diye soruyor
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //DI için IUserService arayüzünü çaðýrdýðým zaman UserService sýnýfýný getirmesini söylüyorum.
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
