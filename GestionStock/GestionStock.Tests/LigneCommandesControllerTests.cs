using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.Models;
using GestionStock.Models.EnumsCommande;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class LigneCommandesControllerTests
    {
        // Création d'un DbContext InMemory unique
        private ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        // Création d'un produit valide
        private Produit CreateProduit(ApplicationDbContext context)
        {
            var produit = new Produit
            {
                Nom = "Produit Test",
                Description = "Description test",
                QuantiteEnStock = 10,
                PrixUnitaire = 20,
                ImageUrl = "test.jpg",
                FournisseurId = 1
            };
            context.Produits.Add(produit);
            context.SaveChanges();
            return produit;
        }

        // Création d'une commande valide
        private Commande CreateCommande(ApplicationDbContext context)
        {
            var commande = new Commande
            {
                ClientId = 1,
                DateVente = DateTime.UtcNow,
                DateLivraisonPrevue = DateTime.UtcNow.AddDays(3),
                Total = 100,
                TypeLivraison = TypeLivraison.Standard,
                EtatCommande = EtatCommande.EnCoursLivraison
            };
            context.Commandes.Add(commande);
            context.SaveChanges();
            return commande;
        }

        // Création d'une ligne de commande
        private LigneCommande CreateLigne(ApplicationDbContext context)
        {
            var produit = CreateProduit(context);
            var commande = CreateCommande(context);

            var ligne = new LigneCommande
            {
                ProduitId = produit.Id,
                CommandeId = commande.Id,
                Quantite = 2
            };
            context.LignesCommande.Add(ligne);
            context.SaveChanges();
            return ligne;
        }

        // ✅ GET ALL
        [Fact]
        public async Task GetLignesCommande_ReturnsList()
        {
            var context = GetDb();
            CreateLigne(context);
            CreateLigne(context);

            var controller = new LigneCommandesController(context);
            var result = await controller.GetLignesCommande();

            var list = Assert.IsType<List<LigneCommande>>(result.Value);
            Assert.Equal(2, list.Count);
        }

        // ✅ GET BY ID
        [Fact]
        public async Task GetLigneCommande_ReturnsLigne_WhenExists()
        {
            var context = GetDb();
            var ligne = CreateLigne(context);

            var controller = new LigneCommandesController(context);
            var result = await controller.GetLigneCommande(ligne.Id);

            var returned = Assert.IsType<LigneCommande>(result.Value);
            Assert.Equal(ligne.Quantite, returned.Quantite);
            Assert.NotNull(returned.Produit);
            Assert.NotNull(returned.Commande);
        }

        [Fact]
        public async Task GetLigneCommande_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new LigneCommandesController(context);

            var result = await controller.GetLigneCommande(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ✅ POST
        [Fact]
        public async Task PostLigneCommande_ReturnsCreated()
        {
            var context = GetDb();
            var produit = CreateProduit(context);
            var commande = CreateCommande(context);

            var controller = new LigneCommandesController(context);
            var ligne = new LigneCommande
            {
                ProduitId = produit.Id,
                CommandeId = commande.Id,
                Quantite = 5
            };

            var result = await controller.PostLigneCommande(ligne);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<LigneCommande>(created.Value);
            Assert.Equal(ligne.Quantite, value.Quantite);
        }

        // ✅ PUT
        [Fact]
        public async Task PutLigneCommande_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var ligne = CreateLigne(context);

            var controller = new LigneCommandesController(context);
            ligne.Quantite = 10;

            var result = await controller.PutLigneCommande(ligne.Id, ligne);
            Assert.IsType<NoContentResult>(result);

            var dbLigne = await context.LignesCommande.FindAsync(ligne.Id);
            Assert.Equal(10, dbLigne.Quantite);
        }

        [Fact]
        public async Task PutLigneCommande_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = GetDb();
            var ligne = CreateLigne(context);

            var controller = new LigneCommandesController(context);
            var result = await controller.PutLigneCommande(ligne.Id + 1, ligne);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutLigneCommande_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new LigneCommandesController(context);
            var ligne = new LigneCommande { Id = 999, Quantite = 1, ProduitId = 1, CommandeId = 1 };

            var result = await controller.PutLigneCommande(999, ligne);
            Assert.IsType<NotFoundResult>(result);
        }

        // ✅ DELETE
        [Fact]
        public async Task DeleteLigneCommande_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var ligne = CreateLigne(context);

            var controller = new LigneCommandesController(context);
            var result = await controller.DeleteLigneCommande(ligne.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.LignesCommande);
        }

        [Fact]
        public async Task DeleteLigneCommande_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new LigneCommandesController(context);

            var result = await controller.DeleteLigneCommande(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
