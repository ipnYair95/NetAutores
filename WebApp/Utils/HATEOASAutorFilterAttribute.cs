
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DTOs;
using WebApp.Services;

namespace WebApp.Utils
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlace generadorEnlace;

        public HATEOASAutorFilterAttribute(GeneradorEnlace generadorEnlace)
        {
            this.generadorEnlace = generadorEnlace;
        }

        public override async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next
            )
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            //var modelo = resultado.Value as AutorDTO ?? throw new ArgumentNullException("Se esperaba una instancia de AutorDTO");

            var autorDTO = resultado.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO>
                    ?? throw new ArgumentNullException("Se esperaba una instancia de AutorDTO");

                autoresDTO.ForEach(async autor => await generadorEnlace.GenerarEnlaces(autor));
                resultado.Value = autoresDTO;
            }
            else
            {

                await generadorEnlace.GenerarEnlaces(autorDTO);

            }


            await next();

        }
    }
}
