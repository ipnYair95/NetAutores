using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApp.Validations;

namespace WebApp.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            // preparacion

            var primeraLetraMayuscula = new PrimeraMayusculaAttribute();

            var valor = "felipe";

            var valContext = new ValidationContext( new { 
                Nombre = valor
            });

            //ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            // verificacion

            Assert.AreEqual("La primer letra debe ser mayuscula", resultado.ErrorMessage );

        }


        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            // preparacion

            var primeraLetraMayuscula = new PrimeraMayusculaAttribute();

            string valor = null;

            var valContext = new ValidationContext(new
            {
                Nombre = valor
            });

            //ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            // verificacion

            Assert.IsNull(resultado);

        }

        [TestMethod]
        public void ValorConPrimeraMayuscula_NoDevuelveError()
        {
            // preparacion

            var primeraLetraMayuscula = new PrimeraMayusculaAttribute();

            string valor = "Felipe";

            var valContext = new ValidationContext(new
            {
                Nombre = valor
            });

            //ejecucion

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            // verificacion

            Assert.IsNull(resultado);

        }
    }
}