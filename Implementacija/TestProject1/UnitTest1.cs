﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Implementacija.Controllers;
using Implementacija.Data;
using Implementacija.Models;
using Implementacija.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Index_ReturnsViewWithModel()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "a@gmail.com";
            korisnik.UserName = "username";
            korisnik.Id = "1";
            _dbContext.Add(korisnik);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
   {
        new Claim(ClaimTypes.NameIdentifier, korisnik.Id),
        new Claim(ClaimTypes.Name, korisnik.UserName),
        new Claim(ClaimTypes.Email, korisnik.Email),
   }, "mock"));
            Poruka poruka = new Poruka();
            poruka.primalacId = "1";
            poruka.Id = 18984;
            poruka.sadrzaj = "1";
            poruka.primalac = (ObicniKorisnik)korisnik;
            _dbContext.Add(poruka);

            await _dbContext.SaveChangesAsync();
            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            var modelReservations = (List<Poruka>)viewResult.Model;
            int expectedCount = _dbContext.Poruke.Count();
            Assert.AreEqual(expectedCount, modelReservations.Count);


        }
        [TestMethod]
        public async Task Details_ReturnsViewWithModel()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            var exp = 2;
            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "a@gmail.com";
            korisnik.UserName = "username";
            korisnik.Id = "2";
            _dbContext.Add(korisnik);
            Poruka poruka = new Poruka();
            poruka.primalacId = "1";
            poruka.Id = 2;
            poruka.sadrzaj = "1";
            poruka.primalac = (ObicniKorisnik)korisnik;
            _dbContext.Add(poruka);
            await _dbContext.SaveChangesAsync();
            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);
            var result = await controller.Details(exp);
            Assert.IsNotNull(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as Poruka;
            Assert.IsNotNull(model);
            Assert.AreEqual(exp, model.Id);

        }
        [TestMethod]
        public async Task Details_ReturnsViewWithModelError()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);
            var result = await controller.Details(null);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Details_NoReservationWithId_ReturnsNotFound()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "6";
            _dbContext.Add(korisnik);
            Poruka poruka = new Poruka();
            poruka.primalacId = "6";
            poruka.Id = 5;
            poruka.sadrzaj = "10";
            poruka.primalac = (ObicniKorisnik)korisnik;
            _dbContext.Add(poruka);
            await _dbContext.SaveChangesAsync();
            var expectedId = 100;
            // Arrange
            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);
            // Act
            var result = await _controller.Details(expectedId);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public Task Create_NoReservationWithId_ReturnsNotFound()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            // Add any necessary setup for your _dbContext here

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = controller.Create();
            var viewResult = result as ViewResult;
            // Assert
            var model = viewResult.Model as PorukaController;

            Assert.IsNotNull(viewResult.ViewData["primalacId"]);
            return Task.CompletedTask;
        }
        [TestMethod]
        public async Task Create1_NoReservationWithId_ReturnsNotFound()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            ObicniKorisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "75@gmail.com";
            korisnik.UserName = "user123";
            korisnik.Id = "17";
            // _dbContext.ObicniKorisnici.Add(((ObicniKorisnik)korisnik)); await _dbContext.SaveChangesAsync();
            Poruka poruka = new Poruka();
            poruka.primalacId = "17";
            poruka.Id = 4452;
            poruka.sadrzaj = "10";
            poruka.primalac = korisnik;
            //   _dbContext.Poruke.Add(poruka);
            //  await _dbContext.SaveChangesAsync();
            // var userManagerMock = new Mock<UserManager<ObicniKorisnik>>(
            // new Mock<IUserStore<ObicniKorisnik>>(korisnik).Object,
            // null, null, null, null, null, null, null, null);

            // var controller = new PorukaController(_dbContext,userManager: null, porukaManagerMock.Object);



            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            var result = await controller.Create(poruka);
            Assert.IsNotNull(result);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

        }
        [TestMethod]
        public async Task IzvodjacIndexTest()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            Korisnik korisnik = new Izvodjac();
            korisnik.Email = "775@gmail.com";
            korisnik.UserName = "user123";
            korisnik.Id = "1000";
            _dbContext.Add(korisnik); await _dbContext.SaveChangesAsync();


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            int expectedCount = _dbContext.Izvodjaci.Count();


            var izvodjaci = result.Model as List<Izvodjac>;
            Assert.AreEqual(expectedCount, izvodjaci.Count);

        }
        [TestMethod]
        public async Task IzvodjacDetailsTest()
        {
            // Postavite vaš lažni kontekst i dodajte izvođača
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            Korisnik izvodjac = new Izvodjac();
            izvodjac.Email = "77115@gmail.com";
            izvodjac.UserName = "user1123";
            izvodjac.Id = "10000";
            _dbContext.Add(izvodjac);
            await _dbContext.SaveChangesAsync();

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Details metodu sa ID izvođača
            var result = await controller.Details(izvodjac.Id) as ViewResult;


            Assert.IsNotNull(result);

            if (result.Model == null)
            {
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }
            else
            {
                Assert.IsInstanceOfType(result.Model, typeof(Izvodjac));


                var detaljiIzvodjaca = result.Model as Izvodjac;
                Assert.AreEqual(izvodjac.Id, detaljiIzvodjaca.Id);
                Assert.AreEqual(izvodjac.Email, detaljiIzvodjaca.Email);

            }
        }
        [TestMethod]
        public async Task DetailsIzvodjac_ReturnsViewWithModelError()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            var controller = new IzvodjacController(_dbContext);
            var result = await controller.Details(null);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public void IzvodjacCreateTest()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Create metodu
            var result = controller.Create() as ViewResult;

            // Provera da li je rezultat tipa ViewResult
            Assert.IsNotNull(result);


        }
        [TestMethod]
        public async Task IzvodjacCreate1Test()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            //var fakeIzvodjac = new Izvodjac();
            Izvodjac izvodjac = new Izvodjac();
            izvodjac.Email = "77115@gmail.com";
            izvodjac.UserName = "user1123";
            izvodjac.Id = "101000";

            var controller = new IzvodjacController(_dbContext);

            var result = await controller.Create(izvodjac);
            Assert.IsNotNull(result);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);



        }
        [TestMethod]
        public async Task IzvodjacCreate_InvalidModel_ReturnsView()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);

            var invalidIzvodjac = new Izvodjac();

            controller.ModelState.AddModelError("SomeKey", "InvalidModelState");
            var result = await controller.Create(invalidIzvodjac);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(invalidIzvodjac, viewResult.Model);
        }
        [TestMethod]
        public async Task Edit_WithNullId_ReturnsNotFound()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Edit(null);


            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_WithNonExistingId_ReturnsNotFound()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Edit("nonExistingId");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_WithExistingId_ReturnsViewWithIzvodjac()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var existingIzvodjac = new Izvodjac { Id = "existingId78", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Edit("existingId78") as ViewResult;


            Assert.IsNotNull(result);

            Assert.IsInstanceOfType(result.Model, typeof(Izvodjac));


        }
        [TestMethod]
        public async Task Edit_IdsMismatch_ReturnsNotFound()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            var controller = new IzvodjacController(_dbContext);

            var result = await controller.Edit("id1", new Izvodjac { Id = "id2" });

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_ValidModel_ReturnsRedirectToAction()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            var existingIzvodjac = new Izvodjac { Id = "existingId10", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Edit("existingId10", existingIzvodjac);


            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));


            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        [TestMethod]
        public async Task Edit_InvalidModel_ReturnsView()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Edit("id", new Izvodjac { Id = "id" });


            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edi2t_InvalidModel_ReturnsView()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);

            var existingIzvodjac = new Izvodjac { Id = "existingId1", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();


            controller.ModelState.AddModelError("SomeKey", "InvalidModelState");


            var result = await controller.Edit("existingId1", existingIzvodjac);


            Assert.IsInstanceOfType(result, typeof(ViewResult));


            var viewResult = (ViewResult)result;
            Assert.AreEqual(existingIzvodjac, viewResult.Model);
        }
        [TestMethod]
        public async Task Edit_InvalidModelState_ReturnsViewWithIzvodjac()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Dodajte izvođača u bazu podataka
            var existingIzvodjac = new Izvodjac { Id = "existingd", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();

            // Simulirajte neispravan model dodavanjem grešaka u ModelState
            controller.ModelState.AddModelError("SomeKey", "InvalidModelState");

            // Pozovite Edit metodu sa neispravnim modelom
            var result = await controller.Edit("existingd", existingIzvodjac);

            // Provera da li je rezultat tipa ViewResult
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Provera da li se vratio isti View sa neispravnim izvođačem
            var viewResult = (ViewResult)result;
            Assert.AreEqual(existingIzvodjac, viewResult.Model);
        }
        [TestMethod]
        public async Task Delete_WithNullId_ReturnsNotFound()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Delete metodu sa null id
            var result = await controller.Delete(null);

            // Provera da li je rezultat tipa NotFoundResult
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_WithNonExistingId_ReturnsNotFound()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Delete metodu sa nevalidnim id
            var result = await controller.Delete("nonExistingId");

            // Provera da li je rezultat tipa NotFoundResult
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_WithExistingId_ReturnsViewWithIzvodjac()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Dodajte izvođača u bazu podataka
            var existingIzvodjac = new Izvodjac { Id = "existingId", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Delete metodu sa postojećim id
            var result = await controller.Delete("existingId") as ViewResult;

            // Provera da li je rezultat tipa ViewResult
            Assert.IsNotNull(result);

            // Provera da li se vratio izvođač kao model
            Assert.IsInstanceOfType(result.Model, typeof(Izvodjac));

        }
        [TestMethod]
        public async Task DeleteConfirmed_WithExistingId_RemovesIzvodjacAndRedirectsToIndex()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Dodajte izvođača u bazu podataka
            var existingIzvodjac = new Izvodjac { Id = "existingI2d", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite DeleteConfirmed metodu sa postojećim id
            var result = await controller.DeleteConfirmed("existingI2d");

            // Provera da li je rezultat tipa RedirectToActionResult
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Provera da li se vratio na Index akciju
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            // Provera da li je izvođač uspešno uklonjen iz baze
            var deletedIzvodjac = await _dbContext.Izvodjaci.FindAsync("existingI2d");
            Assert.IsNull(deletedIzvodjac);
        }
        [TestMethod]
        public async Task Delete_WithValidId_ReturnsViewWithIzvodjac()
        {
            // Postavite vaš lažni kontekst
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            // Dodajte izvođača u bazu podataka
            var existingIzvodjac = new Izvodjac { Id = "existingId1111", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();

            // Kreirajte kontroler sa lažnim kontekstom
            var controller = new IzvodjacController(_dbContext);

            // Pozovite Delete metodu sa postojećim id
            var result = await controller.Delete("existingId1111") as ViewResult;

            // Provera da li je rezultat tipa ViewResult
            Assert.IsNotNull(result);

            // Provera da li se vratio izvođač kao model
            Assert.IsInstanceOfType(result.Model, typeof(Izvodjac));


        }
        [TestMethod]
        public async Task Details_WithValidId_ReturnsViewWithIzvodjac()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            var existingIzvodjac = new Izvodjac { Id = "existingId11", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();


            var controller = new IzvodjacController(_dbContext);

            var result = await controller.Details("existingId11") as ViewResult;

            // Provera da li je rezultat tipa ViewResult
            Assert.IsNotNull(result);

            // Provera da li se vratio izvođač kao model
            Assert.IsInstanceOfType(result.Model, typeof(Izvodjac));


        }
        [TestMethod]
        public async Task Details_WithNullIzvodjac_ReturnsNotFound()
        {
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);


            var controller = new IzvodjacController(_dbContext);


            var result = await controller.Details("nonExistingId");


            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_WithConcurrencyException_ReturnsNotFound()
        {

            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);

            var existingIzvodjac = new Izvodjac { Id = "exiwstingId", UserName = "user123", Email = "user@example.com" };
            _dbContext.Izvodjaci.Add(existingIzvodjac);
            await _dbContext.SaveChangesAsync();


            var controller = new IzvodjacController(_dbContext);


            _dbContext.SaveChanges(); // Ovo će izazvati DbUpdateConcurrencyException jer se isti zapis izmenjuje bez sinhronizacije

            var updatedIzvodjac = new Izvodjac { Id = "exiwstingId", UserName = "updatedUser", Email = "updated@example.com" };


            var result = await controller.Edit("existingId", updatedIzvodjac) as NotFoundResult;

            // Provera da li je rezultat tipa NotFoundResult
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task Details_ReturnsViewResult_WithValidId()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            // Create a test user
            var korisnik = new ObicniKorisnik
            {
                Email = "b@gmail.com",
                UserName = "username1",
                Id = "64"
            };

            _dbContext.Add(korisnik);

            // Create a test poruka
            var poruka = new Poruka
            {
                primalacId = "6",
                Id = 787,
                sadrzaj = "10",
                primalac = korisnik
            };

            _dbContext.Add(poruka);

            await _dbContext.SaveChangesAsync();

            var expectedId = poruka.Id;

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.Edit(expectedId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            Assert.IsInstanceOfType(result.Model, typeof(Poruka));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            // Use an invalid ID that doesn't exist in the test data
            var invalidId = 100;

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.Edit(invalidId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WithInvalidIdNull()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.Edit(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task Delete_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            // Use an invalid ID that doesn't exist in the test data
            var invalidId = 1002;

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.Delete(invalidId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WithInvalidIdNull()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.Delete(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task Delete_ReturnsViewWithPoruka_WhenIdIsValid()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act

            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "6111";
            _dbContext.Add(korisnik);
            Poruka poruka = new Poruka();
            poruka.primalacId = "6111";
            poruka.Id = 5555;
            poruka.sadrzaj = "10";
            poruka.primalac = (ObicniKorisnik)korisnik;
            _dbContext.Add(poruka);
            await _dbContext.SaveChangesAsync();
            var result = await _controller.Delete(5555) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Poruka));
        }
        [TestMethod]
        public async Task DeleteConfirmed_DeletesPorukaAndRedirectsToIndex()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();


            var validId = 51211;
            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "61111";
            _dbContext.Add(korisnik);
            Poruka poruka = new Poruka();
            poruka.primalacId = "61111";
            poruka.Id = validId;
            poruka.sadrzaj = "10";
            poruka.primalac = (ObicniKorisnik)korisnik;
            _dbContext.Add(poruka);
            await _dbContext.SaveChangesAsync();


            var _controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await _controller.DeleteConfirmed(validId) as RedirectToActionResult;

            // AssertS
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            // Verify that the Poruka was deleted
            Assert.IsFalse(_dbContext.Poruke.Any(e => e.Id == validId));
        }

        [TestMethod]
        public async Task EditPoruka_DeletesPorukaAndRedirectsToIndex()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();


            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            var result = await controller.Edit(1, new Poruka { sadrzaj="djasdj" });

            // Provera da li je rezultat tipa NotFoundResult
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }
        [TestMethod]
        public async Task EditPoruka2_DeletesPorukaAndRedirectsToIndex()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            var result = await controller.Edit(2, new Poruka { Id = 1, sadrzaj = "vlaal", primalac = null, primalacId = "11" });

            // Provera da li je rezultat tipa NotFoundResult
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }
        [TestMethod]
        public async Task EditPoruka_ValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            ObicniKorisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "682328";

            var validId = 1665454;
            var poruka = new Poruka { Id = validId, sadrzaj = "djansdjn", primalac = korisnik, primalacId = "682328" };
            _dbContext.Poruke.Add(poruka);
            await _dbContext.SaveChangesAsync();

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Call Edit method with valid model
            var result = await controller.Edit(validId, poruka) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task EditPoruka_InvalidModel_ReturnsView()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);
            ObicniKorisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b88@gmail.com";
            korisnik.UserName = "username81";
            korisnik.Id = "68232888";
            var poruka = new Poruka { Id = 45432, sadrzaj = "djansdjn", primalac = korisnik, primalacId = "68232888" };
            _dbContext.Add(poruka);
            await _dbContext.SaveChangesAsync();

            // Call Edit method with invalid model
            var result = await controller.Edit(45432, new Poruka { sadrzaj="dahbshdb" }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task EditPoruka_InvalidModel_ReturnsViewWithPoruka()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "688";
            _dbContext.ObicniKorisnici.Add((ObicniKorisnik)korisnik);
            // Assuming you have a valid Poruka ID in your test data
            var validId = 555555;
            var poruka = new Poruka { Id = validId, sadrzaj = "dada", primalac = (ObicniKorisnik)korisnik, primalacId = "688" };
            _dbContext.Poruke.Add(poruka);
            await _dbContext.SaveChangesAsync();

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Set ModelState to simulate invalid model
            controller.ModelState.AddModelError("SomeKey", "InvalidModelState");

            // Call Edit method with invalid model
            var result = await controller.Edit(validId, poruka) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Poruka));
        }
        [TestMethod]
        public async Task EditPoruka_CorrectIdAndValidModel_ReturnsRedirectToAction()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            Korisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b@gmail.com";
            korisnik.UserName = "username1";
            korisnik.Id = "6888";
            _dbContext.ObicniKorisnici.Add((ObicniKorisnik)korisnik);
            //var porukaManagerMock = new Mock<IPorukaManager>();
            var validId = 1212;
            var poruka = new Poruka { Id = validId, sadrzaj = "adhsdas", primalac = (ObicniKorisnik)korisnik, primalacId = "6888" };
            _dbContext.Poruke.Add(poruka);
            await _dbContext.SaveChangesAsync();

            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Call Edit method with correct ID and valid model
            var result = await controller.Edit(validId, poruka) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task Edit_ReturnsNotFound_WithNullId()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await controller.Edit(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WithInvalidId()
        {
            // Arrange
            var myFakeContext = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("VvsDb");
            var _dbContext = new ApplicationDbContext(myFakeContext.Options);
            var porukaManagerMock = new Mock<IPorukaManager>();
            var controller = new PorukaController(_dbContext, userManager: null, porukaManagerMock.Object);

            // Use an invalid ID that doesn't exist in the test data
            var invalidId = 100;

            // Act
            var result = await controller.Edit(invalidId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            var context = new ApplicationDbContext(dbContextOptions);
            ObicniKorisnik korisnik = new ObicniKorisnik();
            korisnik.Email = "b2@gmail.com";
            korisnik.UserName = "u2sername1";
            korisnik.Id = "68828";
            var porukaManagerMock = new Mock<IPorukaManager>();

            var controller = new PorukaController(context, userManager: null, porukaManagerMock.Object);

            // Act
            var result = await controller.Create(new Poruka { Id = 1651651651, sadrzaj = "Test", primalacId = "68828" }) as ViewResult;

            // Assert
            Assert.IsNull(result);
            // Assert.AreEqual("Index", result.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            var context = new ApplicationDbContext(dbContextOptions);

            //  var userManager = Mock.Of<UserManager<IdentityUser>>();
            var porukaManagerMock = new Mock<IPorukaManager>();

            var controller = new PorukaController(context, userManager: null, porukaManagerMock.Object);
            controller.ModelState.AddModelError("sadrzaj", "Required"); // Simulating model validation error

            // Act
            var result = await controller.Create(new Poruka { Id = 13115121, sadrzaj = null, primalacId = "6888" }) as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            // Provjera da li je primalacId postavljen u TempData
            Assert.IsNotNull(controller.TempData);
            Assert.AreEqual("6888", controller.TempData["primalacId"]);

            // Provjera da li je primalacId postavljen u ViewData
            Assert.IsNotNull(controller.ViewData);
            Assert.AreEqual("6888", controller.ViewData["primalacId"]);
        }

    }
}
