using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs.ProduitDTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GestionStock.Tests
{
    public class ProduitsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Produit CreateProduit(ApplicationDbContext context)
        {
            var produit = new Produit
            {
                Nom = "Produit Test",
                Description = "Desc",
                QuantiteEnStock = 10,
                PrixUnitaire = 20,
                Categorie = Models.EnumCategorieProduit.CategorieProduit.Informatique,
                FournisseurId = 1,
                ImageUrl = "images/test.jpg"
            };
            context.Produits.Add(produit);
            context.SaveChanges();
            return produit;
        }

        [Fact]
        public async Task GetProduits_ReturnsList()
        {
            var context = GetInMemoryDbContext();
            CreateProduit(context);

            var controller = new ProduitsController(context);
            var result = await controller.GetProduits();

            var list = Assert.IsType<List<ProduitDto>>(result.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetProduit_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var result = await controller.GetProduit(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduit_ReturnsCreated()
        {
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var produit = new Produit
            {
                Nom = "Produit 1",
                Description = "Desc",
                QuantiteEnStock = 5,
                PrixUnitaire = 15,
                Categorie = Models.EnumCategorieProduit.CategorieProduit.Sport,
                ImageUrl = "images/default.jpg",
                FournisseurId = 1
            };

            var result = await controller.PostProduit(produit);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<Produit>(created.Value);
            Assert.Equal(produit.Nom, value.Nom);
        }

        [Fact]
        public async Task AjouterProduitAvecImage_ReturnsBadRequest_WhenNoImage()
        {
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var dto = new ProduitUploadDto
            {
                Nom = "Produit sans image",
                Description = "Desc",
                QuantiteEnStock = 1,
                PrixUnitaire = 10,
                Categorie = Models.EnumCategorieProduit.CategorieProduit.Maison,
                FournisseurId = 1
                // Image = null
            };

            var result = await controller.AjouterProduitAvecImage(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AjouterProduitAvecImage_ReturnsOk_WhenImageProvided()
        {
            Directory.CreateDirectory("wwwroot/images"); // ← important

            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var content = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(content);
            var file = new FormFile(stream, 0, stream.Length, "file", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var dto = new ProduitUploadDto
            {
                Nom = "Produit avec image",
                Description = "Desc",
                QuantiteEnStock = 1,
                PrixUnitaire = 10,
                Categorie = Models.EnumCategorieProduit.CategorieProduit.Maison,
                FournisseurId = 1,
                Image = file
            };

            var result = await controller.AjouterProduitAvecImage(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var produit = Assert.IsType<Produit>(okResult.Value);
            Assert.Equal("Produit avec image", produit.Nom);
        }

        [Fact]
        public async Task DeleteProduit_ReturnsNotFound_WhenProduitDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var controller = new ProduitsController(context);

            var result = await controller.DeleteProduit(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduit_ReturnsOk_WhenProduitExists()
        {
            var context = GetInMemoryDbContext();
            var produit = CreateProduit(context);

            var controller = new ProduitsController(context);
            var result = await controller.DeleteProduit(produit.Id);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Produit supprimé avec succès !", ok.Value);
        }
    }
}
