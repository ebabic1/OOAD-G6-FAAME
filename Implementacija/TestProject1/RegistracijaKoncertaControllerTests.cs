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
        private Izvodjac izvodjac;
        private Koncert koncert;
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
            _context.Add(izvodjac);
        }

        [TestMethod]
        public async Task Register_ValidKoncert_RedirectsToConfirmation()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            var result = await controller.Register(koncert) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Confirmation", result.ActionName);
        }

        [TestMethod]
        public async Task Register_ValidKoncert_SavesToDatabase()
        {
            await _context.SaveChangesAsync();
            var controller = new RegistracijaKoncertaController(_context);

            await controller.Register(koncert);

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
            _context.SaveChanges();
        }
    }
}
