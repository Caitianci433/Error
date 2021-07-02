using Microsoft.AspNetCore.Builder;

namespace Presentation.Configure
{
    /// <summary>
    /// MyConfigurationExtension
    /// </summary>
    public static class MyConfigurationExtension
    {
        /// <summary>
        /// OpenApi
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMySwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Presentation v1"));
            return app;
        }

        /// <summary>
        /// 配置起始页
        /// </summary>
        /// <param name="app"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIndex(this IApplicationBuilder app,string path)
        {
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Add(path);
            app.UseDefaultFiles(defaultFilesOptions);
            return app;
        }
    }
}
