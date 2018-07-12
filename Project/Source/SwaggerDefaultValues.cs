using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Api213
{
    /// <summary>
    /// SwaggerDefaultValues
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https: //github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https: //github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = context.ApiDescription.ParameterDescriptions.First(p => string.Equals(p.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase));
                var routeInfo = description.RouteInfo;

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (routeInfo == null)
                {
                    continue;
                }

                if (parameter.Default == null)
                {
                    parameter.Default = routeInfo.DefaultValue;
                }

                parameter.Required |= !routeInfo.IsOptional;
            }
        }

        /// <summary>
        /// CreateInfoForApiVersion
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info
            {
                Title = $" API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A sample application   API versioning.",
                Contact = new Contact { Name = "Supervielle SA", Email = "xxxx@somewhere.com" },
                TermsOfService = "Private"

                // License = new License() { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            if (description.IsDeprecated) info.Description += " This API version has been deprecated.";

            return info;
        }
   
        /// <summary>
        /// AddSwaggerGen
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerGenforService(IServiceCollection services) => services.AddSwaggerGen(
            options =>
            {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments
                if (XmlCommentsFilePath() != null)
                 options.IncludeXmlComments(XmlCommentsFilePath(), true);

                options.DescribeAllEnumsAsStrings();
                options.DescribeAllParametersInCamelCase();
                options.CustomSchemaIds((type) => type.FullName);
            });

        /// <summary>
        /// using Swashbuckle.AspNetCore.SwaggerUI
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        public static void SwaggerOptionUi(IApiVersionDescriptionProvider provider, SwaggerUIOptions options)
        {
            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }

            options.RoutePrefix = string.Empty; // "/swagger";
        }

        /// <summary>
        /// Set the comments path for the swagger json and ui.
        /// </summary>
        /// <returns></returns>
        private static string XmlCommentsFilePath()
        {
            string fileName;
            var basePath = AppContext.BaseDirectory;
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            try
            {
                fileName = System.IO.Path.GetFileName(assemblyName + ".xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            if (fileName == null || fileName.StartsWith("testhost"))
                return null;

            return System.IO.Path.Combine(basePath, fileName);
        }
    }
}