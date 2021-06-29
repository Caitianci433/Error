using Infrastructure.DB;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            //redis
            var redisConnection = Configuration.GetConnectionString("MysqlConnection");
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
            //DB
            var sqlConnection = Configuration.GetConnectionString("MysqlConnection");
            services.AddDbContext<MainDBContext>(options => options.UseMySQL(sqlConnection));
            //Repositoryע��
            var types = Assembly.Load("Infrastructure").GetTypes();
            var iRepositories = types.Where(o => o.GetInterface("IRepository") != null && !o.IsClass).ToList<Type>();

            foreach (var iRepository in iRepositories)
            {
                var repository = types.Where(o => o.GetInterface(iRepository.Name) != null).SingleOrDefault();
                services.AddScoped(iRepository, repository);
            }

            services.AddControllers();
            //����UseCase
            services.AddMediatR(Assembly.Load("UseCase"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Presentation", Version = "v1" });
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                c.IncludeXmlComments(xmlPath, true);
            });
            //���ÿ�������
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));
            //JWT
            var jwtConfig = Configuration.GetSection("Jwt");
            //������Կ
            var symmetricKeyAsBase64 = jwtConfig.GetValue<string>("Secret");
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            //��֤����
            services.AddAuthentication("Bearer")
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,//�Ƿ���֤ǩ��,����֤�Ļ����Դ۸����ݣ�����ȫ
                        IssuerSigningKey = signingKey,//���ܵ���Կ
                        ValidateIssuer = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Iss�Ƿ��ӦValidIssuer����
                        ValidIssuer = jwtConfig.GetValue<string>("Iss"),//������
                        ValidateAudience = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Aud�Ƿ��ӦValidAudience����
                        ValidAudience = jwtConfig.GetValue<string>("Aud"),//������
                        ValidateLifetime = true,//�Ƿ���֤����ʱ�䣬�����˾;ܾ�����
                        ClockSkew = TimeSpan.Zero,//����ǻ������ʱ�䣬Ҳ����˵����ʹ���������˹���ʱ�䣬����ҲҪ���ǽ�ȥ������ʱ��+���壬Ĭ�Ϻ�����7���ӣ������ֱ������Ϊ0
                        RequireExpirationTime = true,
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {            
                app.UseDeveloperExceptionPage();
            }
            //OpenApi
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Presentation v1"));
            //������������
            app.UseCors("CorsPolicy");
            //http�ض���
            //app.UseHttpsRedirection();

            //������ʼҳ
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            //��̬�ļ�
            app.UseStaticFiles();
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
