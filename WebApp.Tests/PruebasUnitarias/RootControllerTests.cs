using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApp.Controllers.v1;
using WebApp.Tests.Mocks;

namespace WebApp.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
 

        [TestMethod]
        public async Task siUsuarioEsAdmin_Obtenemos4Links()
        {
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();

            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            //ejecucion

            var resultado = await rootController.Get();

            //verificacion

            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task siUsuarioNoEsAdmin_Obtenemos2Links_UsandoMoq()
        {
            //preparacion

            var mockAuthorizationService = new Mock<IAuthorizationService>();

            mockAuthorizationService.Setup( x => 
                x.AuthorizeAsync( 
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x =>
               x.AuthorizeAsync(
                   It.IsAny<ClaimsPrincipal>(),
                   It.IsAny<object>(),
                   It.IsAny<string>()
           )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup( x =>
                x.Link(
                    It.IsAny<string>(),
                    It.IsAny<object>()
            )).Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockURLHelper.Object;


            //ejecucion

            var resultado = await rootController.Get();

            //verificacion

            Assert.AreEqual(2, resultado.Value.Count());
        }


    }
}
