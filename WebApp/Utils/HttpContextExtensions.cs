using Microsoft.EntityFrameworkCore;

namespace WebApp.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionCabecera<T>(
            this HttpContext httpContext, IQueryable<T> quearyable
            )
        {
            if (httpContext == null )
            {
                throw new ArgumentNullException( nameof(httpContext) );
            }

            double cantidad = await quearyable.CountAsync();

            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());

        }
    }
}
