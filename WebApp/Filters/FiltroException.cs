using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters
{
    public class FiltroException: ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroException> logger;

        public FiltroException( ILogger<FiltroException> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }

    }
}
