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
    public class AdminDashboardControllerTests
    {
        private ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Utilisateur CreateClient()
        {
            return new Utilisateur
            {
                Nom = "Client",
                Email = "client@test.com",
                Password = "hashed",
                Telephone = "12345678",
                Adresse = "Tunis",
                Role = Role.Client
            };
        }

        private Fournisseur CreateFournisseur()
        {
            return new Fournisseur
            {
                Nom = "Fournisseur Test",
                Email = "fournisseur@test.com",
                Telephone = "11111111",
                Adresse = "Sfax"
            };
        }

        private Produit CreateProduit(int fournisseurId)
        {
            return new Produit
            {
                Nom = "Produit Test",
                Description = "Produit pour test",
                QuantiteEnStock = 100,
                PrixUnitaire = 50,
                FournisseurId = fournisseurId
            };
        }

        // ✅ TEST STATS
        [Fact]
        public async Task GetStats_ReturnsStats()
        {
            var context = GetDb();

            var fournisseur = CreateFournisseur();
            context.Fournisseurs.Add(fournisseur);
            await context.SaveChangesAsync();

            var produit = CreateProduit(fournisseur.Id);
            context.Produits.Add(produit);

            var client = CreateClient();
            context.Utilisateurs.Add(client);

            await context.SaveChangesAsync();

            var commande = new Commande
            {
                ClientId = client.Id,
                DateVente = DateTime.Now,
                Total = 100,
                TypeLivraison = TypeLivraison.Standard,
                EtatCommande = EtatCommande.Preparation,
                DateLivraisonPrevue = DateTime.Now.AddDays(3)
            };

            context.Commandes.Add(commande);
            await context.SaveChangesAsync();

            context.LignesCommande.Add(new LigneCommande
            {
                CommandeId = commande.Id,
                ProduitId = produit.Id,
                Quantite = 2
            });

            await context.SaveChangesAsync();

            var controller = new AdminDashboardController(context);

            var result = await controller.GetStats();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        // ✅ TEST VENTES RECENTES
        [Fact]
        public async Task GetVentesRecentes_ReturnsList()
        {
            var context = GetDb();

            var fournisseur = CreateFournisseur();
            context.Fournisseurs.Add(fournisseur);
            await context.SaveChangesAsync();

            var produit = CreateProduit(fournisseur.Id);
            context.Produits.Add(produit);

            var client = CreateClient();
            context.Utilisateurs.Add(client);

            await context.SaveChangesAsync();

            var commande = new Commande
            {
                ClientId = client.Id,
                DateVente = DateTime.Now,
                Total = 200,
                TypeLivraison = TypeLivraison.Standard,
                EtatCommande = EtatCommande.Preparation,
                DateLivraisonPrevue = DateTime.Now.AddDays(2)
            };

            context.Commandes.Add(commande);
            await context.SaveChangesAsync();

            context.LignesCommande.Add(new LigneCommande
            {
                CommandeId = commande.Id,
                ProduitId = produit.Id,
                Quantite = 1
            });

            await context.SaveChangesAsync();

            var controller = new AdminDashboardController(context);

            var result = await controller.GetVentesRecentes();

            var ok = Assert.IsType<OkObjectResult>(result);
            var ventes = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);

            Assert.NotEmpty(ventes);
        }

        // ✅ TEST TOP PRODUITS
        [Fact]
        public async Task GetTopProduits_ReturnsTopProducts()
        {
            var context = GetDb();

            var fournisseur = CreateFournisseur();
            context.Fournisseurs.Add(fournisseur);
            await context.SaveChangesAsync();

            var produit = CreateProduit(fournisseur.Id);
            context.Produits.Add(produit);
            await context.SaveChangesAsync();

            context.LignesCommande.Add(new LigneCommande
            {
                ProduitId = produit.Id,
                Quantite = 5
            });

            await context.SaveChangesAsync();

            var controller = new AdminDashboardController(context);

            var result = await controller.GetTopProduits();

            var ok = Assert.IsType<OkObjectResult>(result);
            var produits = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);

            Assert.NotEmpty(produits);
        }
    }
}
