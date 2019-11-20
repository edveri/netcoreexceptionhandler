using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApiWithExceptionHandler.Exceptions;
using WebApiWithExceptionHandler.Exceptions.Abstract;
using WebApiWithExceptionHandler.Validation;

namespace WebApiWithExceptionHandler
{
    public class Startup
    {
        private IDictionary<int, ClientErrorData> _clientErrorMapping;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
            {
                _clientErrorMapping = apiBehaviorOptions.ClientErrorMapping;
                apiBehaviorOptions.InvalidModelStateResponseFactory =
                    arg => new ValidationProblemDetailsResult(apiBehaviorOptions.ClientErrorMapping[400].Link);
            });
            services.AddTransient<IExceptionHandlerHelper, ExceptionHandlerHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IExceptionHandlerHelper exceptionHandlerHelper, ILogger<Startup> logger)
        {
            app.UseExceptionHandler(applicationBuilder =>
            {
                
                applicationBuilder.Run(  async context => { await exceptionHandlerHelper.HandleExceptionAsync(context, _clientErrorMapping, logger); });
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}