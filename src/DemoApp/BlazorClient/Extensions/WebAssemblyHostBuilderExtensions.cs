using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace BlazorApp.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder UseStartup<T>(this WebAssemblyHostBuilder builder) where T : class, new()
        {
            var startup = new T();
            var configureServices = startup.GetType().GetMethod("ConfigureServices");
            configureServices.Invoke(startup, new object[] { builder.Services });
            return builder;
        }

        public static IConfigurationBuilder AddHttpJsonStream(this IConfigurationBuilder builder, Stream stream)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            return builder.Add<HttpResponseStreamConfigurationSource>(source =>
            {
                source.Stream = stream;
            });
        }

        public static WebAssemblyHostBuilder ConfigureAppConfiguration(this WebAssemblyHostBuilder builder, Action<WebAssemblyHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                configureDelegate.Invoke(new WebAssemblyHostBuilderContext(builder, scope.ServiceProvider.GetRequiredService<HttpClient>()), builder.Configuration);
            }
            return builder;
        }
    }

    public class WebAssemblyHostBuilderContext
    {
        public WebAssemblyHostBuilderContext(WebAssemblyHostBuilder builder, HttpClient http)
        {
            ConfigurationBuilder = builder.Configuration;
            RootComponents = builder.RootComponents;
            Services = builder.Services;
            Http = http;
        }

        public IConfigurationBuilder ConfigurationBuilder { get; }
        public RootComponentMappingCollection RootComponents { get; }
        public IServiceCollection Services { get; }
        public HttpClient Http { get; }
    }

    public class HttpResponseStreamConfigurationSource : StreamConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new HttpResponseStreamConfigurationProvider(this);
        }
    }

    public class HttpResponseStreamConfigurationProvider : StreamConfigurationProvider
    {
        public HttpResponseStreamConfigurationProvider(StreamConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            //没进来
            JsonConfigurationFileParser.Parse(stream);
        }
    }

    internal class JsonConfigurationFileParser
    {
        private JsonConfigurationFileParser() { }

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        public static IDictionary<string, string> Parse(Stream input)
            => new JsonConfigurationFileParser().ParseStream(input);

        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();

            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(input))
            using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    //throw new FormatException(Resources.FormatError_UnsupportedJSONToken(doc.RootElement.ValueKind));
                    throw new FormatException($"Unsupported JSONToken of {doc.RootElement.ValueKind}.");
                }
                VisitElement(doc.RootElement);
            }

            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            foreach (var property in element.EnumerateObject())
            {
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitElement(value);
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var arrayElement in value.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitValue(arrayElement);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    var key = _currentPath;
                    if (_data.ContainsKey(key))
                    {
                        //throw new FormatException(Resources.FormatError_KeyIsDuplicated(key));
                        throw new FormatException($"Key {key} is duplicated.");
                    }
                    _data[key] = value.ToString();
                    break;

                default:
                    //throw new FormatException(Resources.FormatError_UnsupportedJSONToken(value.ValueKind));
                    throw new FormatException($"Unsupported JSONToken of {value.ValueKind}.");
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
