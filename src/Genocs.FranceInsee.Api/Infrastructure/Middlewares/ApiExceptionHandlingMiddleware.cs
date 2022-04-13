//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using UTU.Platform.API.Exceptions;
//using UTU.Platform.Model.Api;

//namespace UTU.CashBack.API.Infrastructure.Middlewares
//{
//    public static class MiddlewareExtensions
//    {        

//        public static IApplicationBuilder UseApiExceptionHandlingTest(this IApplicationBuilder app)
//                => app.UseMiddleware<ApiExceptionHandlingMiddleware>();
//    }

//    public class ApiExceptionHandlingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

//        public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                await HandleExceptionAsync(context, ex);
//            }
//        }

//        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
//        {
//            ResponseStatus responseStatus = new ResponseStatus();
//            //_logger.LogError(ex, $"An unhandled exception has occurred, {ex.Message}");
//            if (ex is ValidationException)
//            {
//                List<string> errors = ((ValidationException)ex).Errors;
//                foreach (var error in errors)
//                {
//                    responseStatus.Errors.Add(JsonConvert.DeserializeObject<ResponseError>(error));
//                }
//                responseStatus.ErrorCode = responseStatus.Errors.FirstOrDefault().ErrorCode;
//                responseStatus.Message = responseStatus.Errors.FirstOrDefault().Message;
//                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            }
//            else if (ex is NotFoundException)
//            {
//                responseStatus.ErrorCode = ex.GetType().Name;
//                responseStatus.Message = ex.Message;
//                responseStatus.StackTrace = !string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : string.Empty; //ex.ToBetterString();
//                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
//            }
//            else if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidOperationException)
//            {
//                responseStatus.ErrorCode = ex.GetType().Name;
//                responseStatus.Message = ex.Message;
//                //responseStatus.Errors(new ResponseError {  ErrorCode = ex.GetType().Name, FieldName = ex.p })
//                responseStatus.StackTrace = !string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : string.Empty; //ex.ToBetterString();
//                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            }
//            else
//            {
//                responseStatus.ErrorCode = ex.GetType().Name;
//                responseStatus.Message = ex.Message;
//                responseStatus.StackTrace = !string.IsNullOrEmpty(ex.StackTrace) ? ex.StackTrace : string.Empty; //ex.ToBetterString();
//                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//            }

//            context.Response.ContentType = "application/json";
//            var result = JsonConvert.SerializeObject(new { ResponseStatus = responseStatus }, Formatting.Indented);
//            await context.Response.WriteAsync(result);
//        }         
//    }
//}
