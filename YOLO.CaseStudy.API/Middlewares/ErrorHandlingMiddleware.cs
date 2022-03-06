using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using YOLO.CaseStudy.Entities;

namespace YOLO.CaseStudy.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var data = JsonSerializer.Serialize(new ErrorResult(ex.Message));

                await context.Response.WriteAsync(data);
            }
        }
    }
}
