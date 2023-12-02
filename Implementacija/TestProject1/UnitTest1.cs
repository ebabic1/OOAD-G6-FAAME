using Implementacija.Controllers;
using Implementacija.Data;
using Implementacija.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Testovi
{
    [TestClass]
    public class RegistracijaKoncertaControllerTests
    {
        private ApplicationDbContext _context;

        [TestInitialize]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            var izvodjac = new Izvodjac { Id = "12345", UserName = "NoviIzvodjac", Email = "noviizvodjac@example.com" };
            await _context.Izvodjaci.AddAsync(izvodjac);
            await _context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Register_ValidKoncert_RedirectsToConfirmation()
        {

            var controller = new RegistracijaKoncertaController(_context);

            var result = await controller.Register(new Koncert { Id = 1, naziv = "noviKoncert", zanr = Zanr.HIPHOP, datum = DateTime.Now, izvodjacId = "12345" }) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Confirmation", result.ActionName);
        }

        [TestMethod]
        public async Task Register_ValidKoncert_SavesToDatabase()
        {

            var controller = new RegistracijaKoncertaController(_context);

            await controller.Register(new Koncert { Id = 1, naziv = "noviKoncert", zanr = Zanr.ROCK, datum = DateTime.Now, izvodjacId = "12345" });

            var savedKoncert = _context.Koncerti.FirstOrDefault(k => k.Id == 1);
            Assert.IsNotNull(savedKoncert);
            Assert.AreEqual(Zanr.ROCK, savedKoncert.zanr);
        }

        [TestMethod]
        public void Confirmation_ReturnsView()
        {
            var controller = new RegistracijaKoncertaController(_context);

            var result = controller.Confirmation() as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Register_ReturnsView()
        {

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
