namespace WebApp.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; }

        private int recordsPagina = 10;

        private readonly int cantidadMaxPorPagina = 50;

        public int RecordsPagina
        {
            get
            {
                return recordsPagina;
            }

            set
            {
                recordsPagina = (value > cantidadMaxPorPagina) ? cantidadMaxPorPagina : value;
            }
        }
    }
}
