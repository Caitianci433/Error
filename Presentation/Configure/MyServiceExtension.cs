using Infrastructure.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Configure
{
    /// <summary>
    /// MyServiceExtension
    /// </summary>
    public static class MyServiceExtension
    {
        /// <summary>
        /// AddMySwagger
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMySwagger(this IServiceCollection services) 
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Presentation", Version = "v1" });
                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                c.IncludeXmlComments(xmlPath, true);
            });
            return services;
        }

        /// <summary>
        /// redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMyRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
            return services;
        }

        /// <summary>
        /// mysql
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMyMysql(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnection = configuration.GetConnectionString("MySql");
            services.AddDbContext<MainDBContext>(options => options.UseMySQL(sqlConnection));
            return services;
        }

        /// <summary>
        /// Repository
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            var types = Assembly.Load("Infrastructure").GetTypes();
            var iRepositories = types.Where(o => o.GetInterface("IRepository") != null && !o.IsClass).ToList<Type>();

            foreach (var iRepository in iRepositories)
            {
                var repository = types.Where(o => o.GetInterface(iRepository.Name) != null).SingleOrDefault();
                services.AddScoped(iRepository, repository);
            }
            return services;
        }

        /// <summary>
        /// PieplineBehavior
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPieplineBehavior(this IServiceCollection services)
        {
            var types = Assembly.Load("UseCase").GetTypes();

            var type = typeof(UseCase.Behavior.AbstractPipelineBehavior<,>);
            var pieplineBehaviors = types.Where(o => o.BaseType?.Name== type.Name).ToList<Type>();

            foreach (var pieplineBehavior in pieplineBehaviors)
            {
                services.AddScoped(typeof(IPipelineBehavior<,>), pieplineBehavior);
            }
            

            return services;
        }

        /// <summary>
        /// Cors
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCORS(this IServiceCollection services)
        {
            //配置跨域请求
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            return services;
        }

        /// <summary>
        /// JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //JWT
            var jwtConfig = configuration.GetSection("Jwt");
            //生成密钥
            var symmetricKeyAsBase64 = jwtConfig.GetValue<string>("Secret");
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            //认证参数
            services.AddAuthentication("Bearer")
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,//是否验证签名,不验证的画可以篡改数据，不安全
                        IssuerSigningKey = signingKey,//解密的密钥
                        ValidateIssuer = true,//是否验证发行人，就是验证载荷中的Iss是否对应ValidIssuer参数
                        ValidIssuer = jwtConfig.GetValue<string>("Iss"),//发行人
                        ValidateAudience = true,//是否验证订阅人，就是验证载荷中的Aud是否对应ValidAudience参数
                        ValidAudience = jwtConfig.GetValue<string>("Aud"),//订阅人
                        ValidateLifetime = true,//是否验证过期时间，过期了就拒绝访问
                        ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲，默认好像是7分钟，你可以直接设置为0
                        RequireExpirationTime = true,
                    };
                });

            return services;
        }

    }
}
