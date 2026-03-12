namespace ISpanShop.MVC.Middleware
{
    /// <summary>
    /// 全域例外處理 Middleware —— 捕捉未處理例外，回傳統一 JSON 格式錯誤訊息
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate                      _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment                  _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next        = next;
            _logger      = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未預期的伺服器錯誤 [{Method}] {Path}",
                    context.Request.Method, context.Request.Path);

                // 開發環境：直接拋出例外，讓 ASP.NET Core 顯示詳細錯誤頁面（黃頁）
                if (_environment.IsDevelopment())
                {
                    throw;
                }

                // 正式環境：回傳 JSON 格式的友善錯誤訊息
                context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "伺服器發生未預期的錯誤，請稍後再試。"
                });
            }
        }
    }
}
