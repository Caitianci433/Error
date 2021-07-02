using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Configure;
using System.Reflection;

namespace Presentation
{
#pragma warning disable 1591
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
            //Redis
            services.AddMyRedis(Configuration);
            //Mysql
            services.AddMyMysql(Configuration);
            //Repository injection
            services.AddRepository();
            //������
            services.AddControllers();
            //����UseCase
            services.AddMediatR(Assembly.Load("UseCase"));
            //Swagger
            services.AddMySwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //OpenApi
            app.UseMySwagger();
            //������ʼҳ
            app.UseIndex("index.html");
            //��̬�ļ�
            app.UseStaticFiles();
            //����·��
            app.UseRouting();
            //��֤����Ȩ
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
