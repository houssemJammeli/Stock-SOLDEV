using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class UtilisateursControllerTests
    {
        private ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private Utilisateur CreateUtilisateur() => new Utilisateur
        {
            Nom = "Ahmed",
            Email = "ahmed@test.com",
            Password = "123456",
            Telephone = "11111111",
            Adresse = "Tunis",
            Role = Role.Client
        };

        private Utilisateur UpdateUtilisateur() => new Utilisateur
        {
            Nom = "Houssem",
            Email = "houssem@test.com",
            Password = "654321",
            Telephone = "22222222",
            Adresse = "Sfax",
            Role = Role.Admin
        };

        // ✅ GET ALL
        [Fact]
        public async Task GetUtilisateurs_ReturnsList()
        {
            var context = GetDb();
            context.Utilisateurs.Add(CreateUtilisateur());
            context.Utilisateurs.Add(CreateUtilisateur());
            await context.SaveChangesAsync();

            var controller = new UtilisateursController(context);
            var result = await controller.GetUtilisateurs();

            var list = Assert.IsType<List<Utilisateur>>(result.Value);
            Assert.Equal(2, list.Count);
        }

        // ✅ GET BY ID
        [Fact]
        public async Task GetUtilisateur_ReturnsUtilisateur_WhenExists()
        {
            var context = GetDb();
            var user = CreateUtilisateur();
            context.Utilisateurs.Add(user);
            await context.SaveChangesAsync();

            var controller = new UtilisateursController(context);
            var result = await controller.GetUtilisateur(user.Id);

            var utilisateur = Assert.IsType<Utilisateur>(result.Value);
            Assert.Equal(user.Nom, utilisateur.Nom);
        }

        [Fact]
        public async Task GetUtilisateur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new UtilisateursController(context);

            var result = await controller.GetUtilisateur(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ✅ POST
        [Fact]
        public async Task PostUtilisateur_ReturnsCreated()
        {
            var context = GetDb();
            var controller = new UtilisateursController(context);

            var utilisateur = CreateUtilisateur();
            var result = await controller.PostUtilisateur(utilisateur);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<Utilisateur>(created.Value);
            Assert.Equal(utilisateur.Nom, value.Nom);
            Assert.Single(context.Utilisateurs);
        }

        // ✅ PUT
        [Fact]
        public async Task PutUtilisateur_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var user = CreateUtilisateur();
            context.Utilisateurs.Add(user);
            await context.SaveChangesAsync();

            var controller = new UtilisateursController(context);
            var updatedUser = UpdateUtilisateur();
            updatedUser.Id = user.Id;

            var result = await controller.PutUtilisateur(user.Id, updatedUser);
            Assert.IsType<NoContentResult>(result);

            var dbUser = await context.Utilisateurs.FindAsync(user.Id);
            Assert.Equal(updatedUser.Nom, dbUser.Nom);
            Assert.Equal(updatedUser.Role, dbUser.Role);
        }

        [Fact]
        public async Task PutUtilisateur_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = GetDb();
            var controller = new UtilisateursController(context);

            var utilisateur = CreateUtilisateur();
            var result = await controller.PutUtilisateur(999, utilisateur);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutUtilisateur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new UtilisateursController(context);

            var utilisateur = CreateUtilisateur();
            utilisateur.Id = 999;
            var result = await controller.PutUtilisateur(999, utilisateur);
            Assert.IsType<NotFoundResult>(result);
        }

        // ✅ DELETE
        [Fact]
        public async Task DeleteUtilisateur_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var user = CreateUtilisateur();
            context.Utilisateurs.Add(user);
            await context.SaveChangesAsync();

            var controller = new UtilisateursController(context);
            var result = await controller.DeleteUtilisateur(user.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Utilisateurs);
        }

        [Fact]
        public async Task DeleteUtilisateur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new UtilisateursController(context);

            var result = await controller.DeleteUtilisateur(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
