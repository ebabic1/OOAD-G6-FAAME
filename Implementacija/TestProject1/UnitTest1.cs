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
    public class RegistracijaKoncertaControllerTests
    {
        private ApplicationDbContext _context;
        private IRezervacijaManager rezervacijaManager;
        private Izvodjac izvodjac;
        private Koncert koncert;
        private RezervacijaKarte rezervacijaKarte;
        private Rezervacija rezervacija;
        private ObicniKorisnik obicniKorisnik;
        private Koncert koncertForRegister;
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
            koncertForRegister = new Koncert
            {
                Id = 2,
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
                obicniKorisnikId = "1",
                koncertId = 1,
                tipMjesta = TipMjesta.PARTER
            };
            _context.AddRange(izvodjac, koncert, rezervacija, obicniKorisnik, rezervacijaKarte);
        }

        [TestMethod]
        public async Task Register_ValidKoncert_RedirectsToConfirmation()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            var result = await controller.Register(koncertForRegister) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Confirmation", result.ActionName);
        }

        [TestMethod]
        public async Task Register_ValidKoncert_SavesToDatabase()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            await controller.Register(koncertForRegister);

            var savedKoncert = _context.Koncerti.FirstOrDefault(k => k.Id == 1);
            Assert.IsNotNull(savedKoncert);
            Assert.AreEqual(Zanr.HIPHOP, savedKoncert.zanr);
        }

        [TestMethod]
        public async Task Confirmation_ReturnsView()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            var result = controller.Confirmation() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public async Task Register_ReturnsView()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            var result = controller.Register() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
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
            var result = await controller.Details(5); 

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
            var result = await controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(RezervacijaKarte));
            Assert.AreEqual(rezervacijaKarte.Id, ((RezervacijaKarte)viewResult.Model).Id);
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
            var koncertForRegisterToDelete = _context.Koncerti.FirstOrDefault(k => k.Id == 2);
            if (koncertForRegisterToDelete != null)
            {
                _context.Koncerti.Remove(koncertForRegisterToDelete);
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

            _context.SaveChanges();
        }
    }
}
