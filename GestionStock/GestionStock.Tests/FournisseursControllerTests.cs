using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs;
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
    public class FournisseursControllerTests
    {
        // Crée un DbContext InMemory isolé pour chaque test
        private ApplicationDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private FournisseurCreateDto CreateDto() => new FournisseurCreateDto
        {
            Nom = "Fournisseur Test",
            Email = "f@test.com",
            Telephone = "12345678",
            Adresse = "Tunis"
        };

        private FournisseurUpdateDto UpdateDto() => new FournisseurUpdateDto
        {
            Nom = "Fournisseur Modifié",
            Email = "f2@test.com",
            Telephone = "87654321",
            Adresse = "Sfax"
        };

        // ✅ GET ALL
        [Fact]
        public async Task GetFournisseurs_ReturnsList()
        {
            var context = GetDb();
            context.Fournisseurs.Add(new Fournisseur { Nom = "F1", Email = "f1@test.com", Telephone = "111", Adresse = "Tunis" });
            context.Fournisseurs.Add(new Fournisseur { Nom = "F2", Email = "f2@test.com", Telephone = "222", Adresse = "Sfax" });
            await context.SaveChangesAsync();

            var controller = new FournisseursController(context);
            var result = await controller.GetFournisseurs();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<FournisseurDto>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        // ✅ GET BY ID
        [Fact]
        public async Task GetFournisseur_ReturnsFournisseur_WhenExists()
        {
            var context = GetDb();
            var f = new Fournisseur { Nom = "F1", Email = "f1@test.com", Telephone = "111", Adresse = "Tunis" };
            context.Fournisseurs.Add(f);
            await context.SaveChangesAsync();

            var controller = new FournisseursController(context);
            var result = await controller.GetFournisseur(f.Id);

            var fournisseur = Assert.IsType<Fournisseur>(result.Value);
            Assert.Equal(f.Nom, fournisseur.Nom);
        }

        [Fact]
        public async Task GetFournisseur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new FournisseursController(context);

            var result = await controller.GetFournisseur(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ✅ CREATE
        [Fact]
        public async Task CreateFournisseur_ReturnsCreated()
        {
            var context = GetDb();
            var controller = new FournisseursController(context);

            var dto = CreateDto();
            var result = await controller.CreateFournisseur(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<FournisseurDto>(created.Value);
            Assert.Equal(dto.Nom, value.Nom);
            Assert.Single(context.Fournisseurs);
        }

        // ✅ UPDATE
        [Fact]
        public async Task UpdateFournisseur_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var f = new Fournisseur { Nom = "F1", Email = "f1@test.com", Telephone = "111", Adresse = "Tunis" };
            context.Fournisseurs.Add(f);
            await context.SaveChangesAsync();

            var controller = new FournisseursController(context);
            var dto = UpdateDto();

            var result = await controller.UpdateFournisseur(f.Id, dto);
            Assert.IsType<NoContentResult>(result);

            var updated = await context.Fournisseurs.FindAsync(f.Id);
            Assert.Equal(dto.Nom, updated.Nom);
        }

        [Fact]
        public async Task UpdateFournisseur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new FournisseursController(context);

            var result = await controller.UpdateFournisseur(999, UpdateDto());
            Assert.IsType<NotFoundResult>(result);
        }

        // ✅ DELETE
        [Fact]
        public async Task DeleteFournisseur_ReturnsNoContent_WhenExists()
        {
            var context = GetDb();
            var f = new Fournisseur { Nom = "F1", Email = "f1@test.com", Telephone = "111", Adresse = "Tunis" };
            context.Fournisseurs.Add(f);
            await context.SaveChangesAsync();

            var controller = new FournisseursController(context);
            var result = await controller.DeleteFournisseur(f.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Fournisseurs);
        }

        [Fact]
        public async Task DeleteFournisseur_ReturnsNotFound_WhenDoesNotExist()
        {
            var context = GetDb();
            var controller = new FournisseursController(context);

            var result = await controller.DeleteFournisseur(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
