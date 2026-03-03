using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs.ProduitDTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class ProduitsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetProduits_ReturnsEmptyList_WhenNoProduit()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            // Act
            var result = await controller.GetProduits();

            // Assert
            var produits = Assert.IsType<List<ProduitDto>>(result.Value);
            Assert.Empty(produits);
        }

        

        [Fact]
        public async Task GetProduit_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            // Act
            var result = await controller.GetProduit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteProduit_ReturnsNotFound_WhenProduitDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var result = await controller.DeleteProduit(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
