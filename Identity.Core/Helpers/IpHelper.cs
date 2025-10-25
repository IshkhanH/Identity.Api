using Microsoft.AspNetCore.Http;

namespace Identity.Core.Helpers
{
    public static class IpHelper
    {
        public static string GetClientIpAddress(HttpContext context)
        {
            
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                return ips[0].Trim();
            }
            
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }
          
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}