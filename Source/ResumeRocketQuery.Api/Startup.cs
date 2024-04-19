using ResumeRocketQuery.Api.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ResumeRocketQuery.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            (new ResumeRocketQueryServiceCollection()).ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ResumeRocketQuery/swagger.json", "ResumeRocketQuery");
                c.RoutePrefix = string.Empty;
                
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
