using Implementacija.Controllers;
using Implementacija.Data;
using Implementacija.Models;
using Implementacija.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testovi
{
    [TestClass]
    public class IznajmljivacControllerTests
    {
        private ApplicationDbContext _dbContext;
        private Iznajmljivac iznajmljivac;
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            iznajmljivac = new Iznajmljivac { Id = "3", UserName = "Iznajmljivac3", Email = "user3@example.com" };
            _dbContext.Iznajmljivaci.AddRange(
                new Iznajmljivac { Id = "1", UserName = "Iznajmljivac1", Email = "user1@example.com" },
                new Iznajmljivac { Id = "2", UserName = "Iznajmljivac2", Email = "user2@example.com" },
                iznajmljivac
            );
            _dbContext.SaveChanges();
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithListOfIznajmljivaci()
        {
            // Arrange
            var controller = new IznajmljivacController(_dbContext);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as List<Iznajmljivac>;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);
        }

        [TestMethod]
        public async Task Details_WithValidId_ReturnsView()
        {
            var controller = new IznajmljivacController(_dbContext);
            var result = await controller.Details("1") as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as Iznajmljivac;
            Assert.AreEqual(model.UserName, "Iznajmljivac1");
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            var controller = new IznajmljivacController(_dbContext);

            var result1 = await controller.Details("5") as NotFoundResult;
            var result2 = await controller.Details(null) as NotFoundResult;    

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.AreEqual(404, result1.StatusCode);
            Assert.AreEqual(404, result2.StatusCode);
        }
        [TestMethod]
        public async Task Create_ValidModelState_RedirectsToIndex()
        {
            var iznajmljivacTemp = new Iznajmljivac { Id = "4", UserName = "Iznajmljivac4", Email = "user4@example.com" };
            var controller = new IznajmljivacController(_dbContext);

            var result = await controller.Create(iznajmljivacTemp) as RedirectToActionResult;

            Assert.IsNotNull(result);
            var check = _dbContext.Iznajmljivaci.Any(x => x.Id == "4");
            Assert.IsTrue(check);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task Edit_ValidId_ReturnsModel()
        {
            var controller = new IznajmljivacController (_dbContext);
            var result = await controller.Edit("1") as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as Iznajmljivac;
            Assert.AreEqual("1", model.Id);
        }
        [TestMethod]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var controller = new IznajmljivacController(_dbContext);

            var result1 = await controller.Edit("5") as NotFoundResult;
            var result2 = await controller.Edit(null) as NotFoundResult;
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.AreEqual(404, result1.StatusCode);
            Assert.AreEqual(404, result2.StatusCode);
        }
        [TestMethod]
        public async Task DeleteConfirmed_ValidId_DeletesIznajmljivac()
        {
            var controller = new IznajmljivacController(_dbContext);

            var result = await controller.DeleteConfirmed("1") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var deletedIznajmljivac = _dbContext.Iznajmljivaci.Find("1");

            Assert.IsNull(deletedIznajmljivac);
        }
        [TestMethod]
        public async Task Delete_WithValidId_ReturnsView()
        {
            var controller = new IznajmljivacController(_dbContext);
            var result = await controller.Delete("1") as ViewResult;
            Assert.IsNotNull(result);
            var model = result.Model as Iznajmljivac;  
            Assert.IsNotNull(model);
            Assert.AreEqual("Iznajmljivac1", model.UserName);
        }
        [TestMethod]
        public async Task Delete_WithInValidId_ReturnsNotFoundResult()
        {
            var controller = new IznajmljivacController(_dbContext);
            var result1 = await controller.Delete("999") as NotFoundResult;
            var result2 = await controller.Delete(null) as NotFoundResult;
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result1, typeof(NotFoundResult));
            Assert.IsInstanceOfType(result2, typeof(NotFoundResult));
        }
        [TestMethod]
        public void Create_ReturnsViewResult()
        {
            var controller = new IznajmljivacController(_dbContext);

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task IznajmljivacExists_WithValidId_ReturnsTrue()
        {
            await _dbContext.SaveChangesAsync();
            var controller = new IznajmljivacController(_dbContext);

            var methodInfo = typeof(IznajmljivacController).GetMethod("IznajmljivacExists", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "Method not found");

            var result = methodInfo.Invoke(controller, new object[] { "1" }) as bool?;

            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public async Task Edit_ValidModelState_RedirectsToIndex()
        {
            await _dbContext.SaveChangesAsync();
            var controller = new IznajmljivacController(_dbContext);

            // Update
            iznajmljivac.UserName = "IznajmljivacPromijenjen";

            // Act
            var result = await controller.Edit(iznajmljivac.Id, iznajmljivac) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var updatedIznajmljivac = _dbContext.Iznajmljivaci.FirstOrDefault(r => r.Id == iznajmljivac.Id);
            Assert.IsNotNull(updatedIznajmljivac);
            Assert.AreEqual(iznajmljivac.UserName, updatedIznajmljivac.UserName);
        }
        [TestMethod]
        public async Task Edit_MismatchedIds_ReturnsNotFoundResult()
        {
            await _dbContext.SaveChangesAsync();
            var controller = new IznajmljivacController(_dbContext);
            // Update
            iznajmljivac.UserName = "IznajmljivacPromijenjen";
            // Act
            var result = await controller.Edit(iznajmljivac.Id+1, iznajmljivac) as NotFoundResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
        // This test verifies that the catch block handles DbUpdateConcurrencyException appropriately
       

    }
}
