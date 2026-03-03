using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs.CommandeDTO;
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
    public class CommandesControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // ✅ CREER COMMANDE SUCCESS
        [Fact]
        public async Task CreerCommande_ReturnsOk_WhenClientExists()
        {
            var context = GetInMemoryDbContext();

            var client = new Utilisateur
            {
                Nom = "Client",
                Email = "client@test.com",
                Password = "hashed",
                Role = Role.Client
            };

            context.Utilisateurs.Add(client);
            await context.SaveChangesAsync();

            var controller = new CommandesController(context);

            var dto = new CommandeCreateDto
            {
                ClientId = client.Id,
                Total = 100,
                TypeLivraison = TypeLivraison.Standard,
                Lignes = new List<LigneCommandeDto>
                {
                    new LigneCommandeDto
                    {
                        ProduitId = 1,
                        Quantite = 2
                    }
                }
            };

            var result = await controller.CreerCommande(dto);

            Assert.IsType<OkObjectResult>(result);
            Assert.Single(context.Commandes);
        }

        // ❌ CREER COMMANDE CLIENT NOT FOUND
        [Fact]
        public async Task CreerCommande_ReturnsBadRequest_WhenClientNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = new CommandesController(context);

            var dto = new CommandeCreateDto
            {
                ClientId = 999,
                Total = 50,
                TypeLivraison = TypeLivraison.Standard,
                Lignes = new List<LigneCommandeDto>()
            };

            var result = await controller.CreerCommande(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ✅ GET COMMANDES CLIENT + CALCUL ETAT
        [Fact]
        public async Task GetCommandesClient_UpdatesEtatCommande()
        {
            var context = GetInMemoryDbContext();

            var client = new Utilisateur
            {
                Nom = "Client",
                Email = "client@test.com",
                Password = "hashed",
                Role = Role.Client
            };

            context.Utilisateurs.Add(client);
            await context.SaveChangesAsync();

            var commande = new Commande
            {
                ClientId = client.Id,
                DateVente = DateTime.Now.AddDays(-8), // ancienne commande
                Total = 200,
                TypeLivraison = TypeLivraison.Standard,
                EtatCommande = EtatCommande.Preparation
            };

            context.Commandes.Add(commande);
            await context.SaveChangesAsync();

            var controller = new CommandesController(context);

            var result = await controller.GetCommandesClient(client.Id);

            var commandes = Assert.IsType<List<Commande>>(result.Value);

            Assert.Equal(EtatCommande.Livree, commandes.First().EtatCommande);
        }

        // ✅ GET TOUTES LES COMMANDES (ADMIN)
        [Fact]
        public async Task GetToutesLesCommandes_ReturnsCommandeDtoList()
        {
            var context = GetInMemoryDbContext();

            var client = new Utilisateur
            {
                Nom = "Client",
                Email = "client@test.com",
                Password = "hashed",
                Role = Role.Client
            };

            context.Utilisateurs.Add(client);
            await context.SaveChangesAsync();

            var commande = new Commande
            {
                ClientId = client.Id,
                DateVente = DateTime.Now,
                Total = 300,
                TypeLivraison = TypeLivraison.Standard,
                EtatCommande = EtatCommande.Preparation
            };

            context.Commandes.Add(commande);
            await context.SaveChangesAsync();

            var controller = new CommandesController(context);

            var result = await controller.GetToutesLesCommandes();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<CommandeDto>>(okResult.Value);

            Assert.Single(list);
            Assert.Equal("Client", list.First().NomClient);
        }
    }
}
