using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs.IARecommandationDTOs;
using GestionStock.Models;
using GestionStock.Services.IA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class IAControllerTests
    {
        private ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Fournisseur CreateFournisseur()
        {
            return new Fournisseur
            {
                Nom = "Fournisseur",
                Email = "f@test.com",
                Telephone = "11111111",
                Adresse = "Tunis"
            };
        }

        private Produit CreateProduit(int fournisseurId)
        {
            return new Produit
            {
                Nom = "Produit Test",
                Description = "Produit IA",
                QuantiteEnStock = 10,
                PrixUnitaire = 20,
                ImageUrl = "test.jpg",
                FournisseurId = fournisseurId
            };
        }

        // ✅ IA retourne des produits
        [Fact]
        public async Task Recommandation_ReturnsProducts_WhenIAResponseContainsIds()
        {
            var context = GetDb();

            var fournisseur = CreateFournisseur();
            context.Fournisseurs.Add(fournisseur);
            await context.SaveChangesAsync();

            var produit = CreateProduit(fournisseur.Id);
            context.Produits.Add(produit);
            await context.SaveChangesAsync();

            var iaResponse = new IARecommandationResponseDto
            {
                Imc = 22.5,
                Produits = new List<int> { produit.Id }
            };

            var mockIA = new Mock<IAService>(new HttpClient());
            mockIA.Setup(s => s.RecommanderAsync(It.IsAny<IARecommandationRequestDto>()))
                .ReturnsAsync(iaResponse);

            var controller = new IAController(mockIA.Object, context);

            var dto = new IARecommandationRequestDto
            {
                Poids = 70,
                Taille = 1.75,
                Objectif = 0,
                Niveau = 1
            };

            var result = await controller.Recommandation(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        // ✅ IA retourne liste vide
        [Fact]
        public async Task Recommandation_ReturnsEmptyProducts_WhenIAResponseIsEmpty()
        {
            var context = GetDb();

            var iaResponse = new IARecommandationResponseDto
            {
                Imc = 21,
                Produits = new List<int>()
            };

            var mockIA = new Mock<IAService>(new HttpClient());
            mockIA.Setup(s => s.RecommanderAsync(It.IsAny<IARecommandationRequestDto>()))
                .ReturnsAsync(iaResponse);

            var controller = new IAController(mockIA.Object, context);

            var dto = new IARecommandationRequestDto
            {
                Poids = 65,
                Taille = 1.70,
                Objectif = 1,
                Niveau = 0
            };

            var result = await controller.Recommandation(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        // ❌ IA lance une exception
        [Fact]
        public async Task Recommandation_Returns500_WhenServiceThrowsException()
        {
            var context = GetDb();

            var mockIA = new Mock<IAService>(new HttpClient());
            mockIA.Setup(s => s.RecommanderAsync(It.IsAny<IARecommandationRequestDto>()))
                .ThrowsAsync(new Exception("IA error"));

            var controller = new IAController(mockIA.Object, context);

            var dto = new IARecommandationRequestDto
            {
                Poids = 80,
                Taille = 1.80,
                Objectif = 2,
                Niveau = 2
            };

            var result = await controller.Recommandation(dto);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}
