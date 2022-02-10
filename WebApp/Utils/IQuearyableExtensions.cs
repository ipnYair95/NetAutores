using WebApp.DTOs;

namespace WebApp.Utils
{
    public static class IQuearyableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPagina)
                .Take(paginacionDTO.RecordsPagina);
        }
    }
}
