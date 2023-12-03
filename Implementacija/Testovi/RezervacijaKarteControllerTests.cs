using Implementacija.Controllers;
using Implementacija.Data;
using Implementacija.Models;
using Implementacija.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testovi
{
    [TestClass]
    public class RezervacijaKarteControllerTests
    {
        private ApplicationDbContext _context;
        private IRezervacijaManager rezervacijaManager;
        private Izvodjac izvodjac;
        private Koncert koncert;
        private RezervacijaKarte rezervacijaKarte;
        private RezervacijaKarte rezervacijaKarteForCreate;
        private Rezervacija rezervacija;
        private ObicniKorisnik obicniKorisnik;
        private Iznajmljivac iznajmljivac;
        private Dvorana dvorana;
        private RezervacijaDvorane rezervacijaDvorane;
        [TestInitialize]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            var httpContextAccessor = new HttpContextAccessor();
            var porukaManager = new PorukaManager(_context, httpContextAccessor);
            rezervacijaManager = new RezervacijaManager(_context, porukaManager);
            izvodjac = new Izvodjac
            {
                Id = "12345",
                UserName = "NoviIzvodjac",
                Email = "noviizvodjac@example.com"
            };
            koncert = new Koncert
            {
                Id = 1,
                naziv = "noviKoncert",
                zanr = Zanr.HIPHOP,
                datum = DateTime.Now,
                izvodjacId = "12345"
            };
            rezervacija = new Rezervacija
            {
                cijena = 0,
                potvrda = true,
                Id = 1
            };
            obicniKorisnik = new ObicniKorisnik
            {
                Id = "23456",
                UserName = "NoviObicniKorisnik",
                Email = "noviobicnikorisnik@example.com"
            };
            rezervacijaKarte = new RezervacijaKarte
            {
                Id = 1,
                rezervacijaId = 1,
                obicniKorisnikId = "23456",
                koncertId = 1,
                tipMjesta = TipMjesta.PARTER
            };
            rezervacijaKarteForCreate = new RezervacijaKarte
            {
                Id = 2,
                rezervacijaId = 1,
                obicniKorisnikId = "23456",
                koncertId = 1,
                tipMjesta = TipMjesta.PARTER
            };
            iznajmljivac = new Iznajmljivac
            {
                Id = "34567",
                UserName = "NoviIznajmljivac",
                Email = "noviiznajmljivac@example.com"
            };
            dvorana = new Dvorana
            {
                Id = 1,
                nazivDvorane = "Naziv nove dvorane",
                adresaDvorane = "Adresa nove dvorane",
                brojSjedista = 100, // Postavite željeni broj sjedala
                iznajmljivacId = "34567" // Postavite ID iznajmljivača, ovisno o vašoj implementaciji
            };
            rezervacijaDvorane = new RezervacijaDvorane
            {
                Id = 1,
                rezervacijaId = 1,
                izvodjacId = "12345",
                dvoranaId = 1
            };
            _context.AddRange(izvodjac, koncert, rezervacija, obicniKorisnik, rezervacijaKarte, iznajmljivac, dvorana, rezervacijaDvorane);
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithModel()
        {
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);
            // Act
            var result = await controller.Index();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            var modelReservations = (List<RezervacijaKarte>)viewResult.Model;
            Assert.AreEqual(1, modelReservations.Count);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Details(5); // Ne postoji rezervacija karte sa id-em 5

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenIdExists()
        {
            await _context.SaveChangesAsync();
            // Arrange

            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Details(rezervacijaKarte.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(RezervacijaKarte));
            Assert.AreEqual(rezervacijaKarte.Id, ((RezervacijaKarte)viewResult.Model).Id);
        }

        [TestMethod]
        public async Task Reserve_ReturnsNotFound_WhenIdIsNull()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Reserve(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Reserve_ReturnsNotFound_WhenKoncertDoesNotExist()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Reserve(5); // Ne postoji koncert sa id-em 5

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Reserve_ReturnsViewResult_WhenIdExists()
        {
            // Arrange
            await _context.SaveChangesAsync();

            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Reserve(koncert.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(RezervacijaKarte));
            Assert.AreEqual(koncert.Id, ((RezervacijaKarte)viewResult.Model).koncert.Id);
        }
        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Create(rezervacijaKarteForCreate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {

            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);
            controller.ModelState.AddModelError("FieldName", "Error Message");

            // Act
            var result = await controller.Create(rezervacijaKarteForCreate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [Ignore]
        [TestMethod]
        public async Task CreateReserve_ReturnsRedirectToActionResult_WhenReservationIsSuccessful()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.CreateReserve(rezervacijaKarteForCreate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
        }

        [TestMethod]
        public async Task Edit_ReturnsViewWithModel_WhenIdIsNotNull()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(rezervacijaKarte.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            var model = (RezervacijaKarte)viewResult.Model;
            Assert.AreEqual(rezervacijaKarte.Id, model.Id);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenReservationNotFound()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(1111);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditPost_ReturnsRedirectToIndex_WhenSuccessful()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(rezervacijaKarte.Id, rezervacijaKarte);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public async Task EditPost_ReturnsNotFound_WhenInvalidId()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(1111, rezervacijaKarte);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [Ignore]
        [TestMethod]
        public async Task EditPost_ReturnsNotFound_WhenConcurrencyException()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var novaRezervacijaKarte = new RezervacijaKarte
            {
                Id = 5,
                rezervacijaId = 1,
                obicniKorisnikId = "23456",
                koncertId = 1,
                tipMjesta = TipMjesta.PARTER
            };
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Edit(5, novaRezervacijaKarte);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task EditPost_ReturnsViewWithModel_WhenInvalidModel()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);
            controller.ModelState.AddModelError("field", "Error message"); // Dodajte pogrešku u ModelState

            // Act
            var result = await controller.Edit(rezervacijaKarte.Id, rezervacijaKarte);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(rezervacijaKarte, viewResult.Model);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Delete(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenReservationNotFound()
        {
            await _context.SaveChangesAsync();
            // Arrange
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Delete(1111); // Pretpostavljamo da nema rezervacije s ID-om 1111

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewResult_WhenReservationExists()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.Delete(rezervacijaKarte.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(RezervacijaKarte));
            Assert.AreEqual(rezervacijaKarte.Id, ((RezervacijaKarte)viewResult.Model).Id);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsRedirectToIndex_WhenReservationExists()
        {
            // Arrange
            await _context.SaveChangesAsync();
            var controller = new RezervacijaKarteController(_context, rezervacijaManager);

            // Act
            var result = await controller.DeleteConfirmed(rezervacijaKarte.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        [TestCleanup]
        public void Cleanup()
        {
            var izvodjacToDelete = _context.Izvodjaci.FirstOrDefault(i => i.Id == "12345");
            if (izvodjacToDelete != null)
            {
                _context.Izvodjaci.Remove(izvodjacToDelete);
            }
            var koncertToDelete = _context.Koncerti.FirstOrDefault(k => k.Id == 1);
            if (koncertToDelete != null)
            {
                _context.Koncerti.Remove(koncertToDelete);
            }
            var rezervacijaToDelete = _context.Rezervacija.FirstOrDefault(k => k.Id == 1);
            if (rezervacijaToDelete != null)
            {
                _context.Rezervacija.Remove(rezervacijaToDelete);
            }
            var obicniKorisnikToDelete = _context.ObicniKorisnici.FirstOrDefault(k => k.Id == "23456");
            if (obicniKorisnikToDelete != null)
            {
                _context.ObicniKorisnici.Remove(obicniKorisnikToDelete);
            }
            var rezervacijaKarteToDelete = _context.RezervacijaKarata.FirstOrDefault(k => k.Id == 1);
            if (rezervacijaKarteToDelete != null)
            {
                _context.RezervacijaKarata.Remove(rezervacijaKarteToDelete);
            }
            var rezervacijaKarteForCreateToDelete = _context.RezervacijaKarata.FirstOrDefault(k => k.Id == 2);
            if (rezervacijaKarteForCreateToDelete != null)
            {
                _context.RezervacijaKarata.Remove(rezervacijaKarteForCreateToDelete);
            }
            var iznajmljivacToDelete = _context.Iznajmljivaci.FirstOrDefault(k => k.Id == "34567");
            if (iznajmljivacToDelete != null)
            {
                _context.Iznajmljivaci.Remove(iznajmljivacToDelete);
            }
            var dvoranaToDelete = _context.Dvorane.FirstOrDefault(k => k.Id == 1);
            if (dvoranaToDelete != null)
            {
                _context.Dvorane.Remove(dvoranaToDelete);
            }
            var rezervacijaDvoraneToDelete = _context.RezervacijaDvorana.FirstOrDefault(k => k.Id == 1);
            if (rezervacijaDvoraneToDelete != null)
            {
                _context.RezervacijaDvorana.Remove(rezervacijaDvoraneToDelete);
            }

            _context.SaveChanges();
        }
    }
}
