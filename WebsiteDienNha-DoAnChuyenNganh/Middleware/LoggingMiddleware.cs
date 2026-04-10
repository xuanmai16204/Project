namespace WebsiteDienNha_DoAnChuyenNganh.Middleware
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public LoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			// Minimal logging stub
			await _next(context);
		}
	}
}


