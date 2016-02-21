using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using Thinktecture.IdentityModel.Owin;

namespace WebApplication.Infrastructure
{
    public class InfrastructureStartup
    {
        public void Configuration(IAppBuilder builder)
        {
            var config = new HttpConfiguration();
            config.SuppressDefaultHostAuthentication();
            config.MapHttpAttributeRoutes();
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("1.0", "api");
                c.IncludeXmlComments(GetPath());
            })
            .EnableSwaggerUi(c => c.DisableValidator());

            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            config.Formatters.JsonFormatter.SerializerSettings = jsonSettings;

            builder.UseWebApi(config);
            builder.Use(typeof (AuthenticationMiddleware));
            var settings = new JsonSerializerSettings
            {
            };

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                FileSystem = new PhysicalFileSystem(GetWwwRoot())
            };
            builder.UseFileServer(options);
            
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));
            GlobalHost.HubPipeline.RequireAuthentication();
            GlobalHost.Configuration.DefaultMessageBufferSize = 10;
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(21);
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(7);
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(20);

            builder.MapSignalR();

            var listener = (HttpListener)builder.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        private static string GetPath()
        {
            var path = Assembly.GetExecutingAssembly().CodeBase;
            var ext = Path.GetExtension(path);
            return path.Replace(ext, ".xml");
        }

        private static string GetWwwRoot()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            path = Path.Combine(Path.GetDirectoryName(path), "..\\..\\wwwroot");
            return path;
        }
    }
}
