using System.Net;
using System.Text.Json;

namespace QLBH.Web.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred at {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var isApi = context.Request.Path.StartsWithSegments("/api")
                || context.Request.Headers.Accept.Any(h => h?.Contains("application/json") == true);

            if (!isApi && context.Request.Method == "GET")
            {
                var message = GetFriendlyMessage(exception);
                context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(message)}");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = _env.IsDevelopment() ? exception.Message : GetFriendlyMessage(exception),
                Detail = _env.IsDevelopment() ? exception.StackTrace : null,
                Path = context.Request.Path
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static string GetFriendlyMessage(Exception exception)
        {
            if (exception is Microsoft.Data.SqlClient.SqlException
                || exception.InnerException is Microsoft.Data.SqlClient.SqlException)
            {
                return "Không kết nối được SQL Server. Vui lòng bật SQL Server (SQLEXPRESS hoặc KHAICUTE) và chạy Setup-Database.ps1 trong thư mục dự án.";
            }

            return exception.Message;
        }
    }
}
