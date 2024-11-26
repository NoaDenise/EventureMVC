namespace EventureMVC.Middleware
{
    // Middleware/RoleBasedAccessMiddleware.cs
    public class RoleBasedAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
           
            var userRole = context.Session.GetString("UserRole");
            Console.WriteLine($"User role from session: {userRole}");

            if (context.Session != null)
            {
                var path = context.Request.Path;

                if (path.StartsWithSegments("/logout") || path.StartsWithSegments("/user/logout") || path.StartsWithSegments("/admin/logout"))
                {
                    await _next(context);
                    return;
                }

                if (path.StartsWithSegments("/admin") && userRole != "admin")
                {
                    context.Response.Redirect("/Explore");
                    return;
                }

                if (path.StartsWithSegments("/user") && !path.StartsWithSegments("/user/logout") && userRole == "admin")
                {
                    context.Response.Redirect("/Admin");
                    return;
                }
            }

            await _next(context);
        }
    }

}
