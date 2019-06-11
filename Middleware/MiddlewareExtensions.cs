using Microsoft.AspNetCore.Builder;
namespace Telegram.Bot.Examples.DotNetCoreWebHook.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
