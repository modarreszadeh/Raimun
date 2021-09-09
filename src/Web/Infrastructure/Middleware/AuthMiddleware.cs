using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Web.Infrastructure.Api;

namespace Web.Infrastructure.Middleware
{
    public static class SampleMiddlewareExtensions
    {
        // using in startup class in Sample.Api in configure method
        public static void UseAuthMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthMiddleware>();
        }
    }

    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate request)
        {
            _next = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await WriteToResponse(ApiResultStatusCode.Unauthorized,
                        HttpStatusCode.Unauthorized, httpContext, "عدم دسترسی");
                }

                if (httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    await WriteToResponse(ApiResultStatusCode.ServerError,
                        HttpStatusCode.InternalServerError, httpContext, "خطایی در سرور رخ داده");
                }
            }
        }

        private async Task WriteToResponse(ApiResultStatusCode statusCode, HttpStatusCode httpStatusCode,
            HttpContext httpContext, string message)
        {
            var result = new ApiResult(statusCode, null, message);
            var json = JsonConvert.SerializeObject(result);
            httpContext.Response.StatusCode = (int)httpStatusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
    }
}