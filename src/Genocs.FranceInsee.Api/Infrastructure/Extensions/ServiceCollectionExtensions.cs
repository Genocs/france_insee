using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Polly;
using Refit;
using System.Net.Http.Headers;
using UTU.Platform.API;
using Genocs.FranceInsee.Config;
using Genocs.FranceInsee.Infrastructure.ApiClients;

namespace Genocs.FranceInsee.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                      .RequireAuthenticatedUser()
                      .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
           .AddNewtonsoftJson(options =>
           {
               //options.SerializerSettings.Converters.Add(new StringEnumConverter());
               //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
               options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
               //options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
           });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddCustomRouteConstraint();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

        public static IServiceCollection AddCustomApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration.GetSection("ApplicationsInsights:InstrumentationKey").Value);
            return services;
        }

        public static IServiceCollection AddAPIClients(this IServiceCollection services, IConfiguration configuration)
        {
            var apiClientSetting = new APIClientSetting();
            configuration.GetSection("APIClientSetting").Bind(apiClientSetting);
            services.AddSingleton(apiClientSetting);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(30);
            var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());

            services.AddRefitClient<ISireneApiClient>()
                      .AddCustomHttpClientBuilder(apiClientSetting, timeoutPolicy);

            return services;
        }

        public static IHttpClientBuilder AddCustomHttpClientBuilder(this IHttpClientBuilder builder,
                        APIClientSetting settings, IAsyncPolicy<HttpResponseMessage> timeoutPolicy)
        {

            builder.ConfigureHttpClient((sp, c) =>
            {
                c.BaseAddress = new Uri(settings.SireneApiUrl);

                c.DefaultRequestHeaders.Add("Cookie", settings.Cookie);
                c.DefaultRequestHeaders.Add("Authorization", settings.Authorization); 
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(6, TimeSpan.FromSeconds(5)))
            .AddPolicyHandler(request =>
                {
                    if (request.Method == HttpMethod.Get)
                        return timeoutPolicy;
                    return Policy.NoOpAsync<HttpResponseMessage>();
                });

            return builder;
        }
    }
}
