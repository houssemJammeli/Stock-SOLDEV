using GestionStock.Controllers;
using GestionStock.DTOs.PanierDTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class PanierControllerTests
    {
        private void ResetPanier()
        {
            // On vide le panier statique avant chaque test
            PanierController.GetPanier().Lignes.Clear();
        }

        // ✅ GET panier vide
        [Fact]
        public async Task Get_ReturnsEmptyPanier_WhenNoProduct()
        {
            ResetPanier();
            var controller = new PanierController();

            var result = await controller.Get();

            var panier = Assert.IsType<PanierDTO>(result.Value);
            Assert.Empty(panier.Lignes);
        }

        // ✅ Ajouter produit
        [Fact]
        public async Task AjouterProduitAuPanier_AddsNewProduct()
        {
            ResetPanier();
            var controller = new PanierController();

            var dto = new LignePanierDTO
            {
                ProduitId = 1,
                Quantite = 2
            };

            var result = await controller.AjouterProduitAuPanier(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var panier = Assert.IsType<PanierDTO>(ok.Value);

            Assert.Single(panier.Lignes);
            Assert.Equal(2, panier.Lignes.First().Quantite);
        }

        // ✅ Ajouter même produit → augmente quantité
        [Fact]
        public async Task AjouterProduitAuPanier_IncrementsQuantity_WhenProductExists()
        {
            ResetPanier();
            var controller = new PanierController();

            await controller.AjouterProduitAuPanier(new LignePanierDTO
            {
                ProduitId = 1,
                Quantite = 2
            });

            await controller.AjouterProduitAuPanier(new LignePanierDTO
            {
                ProduitId = 1,
                Quantite = 3
            });

            var panier = PanierController.GetPanier();

            Assert.Single(panier.Lignes);
            Assert.Equal(5, panier.Lignes.First().Quantite);
        }

        // ✅ Supprimer produit
        [Fact]
        public async Task SupprimerProduitAuPanier_RemovesProduct()
        {
            ResetPanier();
            var controller = new PanierController();

            await controller.AjouterProduitAuPanier(new LignePanierDTO
            {
                ProduitId = 1,
                Quantite = 2
            });

            var result = await controller.SupprimerProduitAuPanier(new LignePanierDTO
            {
                ProduitId = 1
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var panier = Assert.IsType<PanierDTO>(ok.Value);

            Assert.Empty(panier.Lignes);
        }

        // ✅ Vider panier
        [Fact]
        public void ViderPanier_ClearsAllProducts()
        {
            ResetPanier();
            var controller = new PanierController();

            controller.AjouterProduitAuPanier(new LignePanierDTO
            {
                ProduitId = 1,
                Quantite = 2
            }).Wait();

            var result = controller.ViderPanier();

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(PanierController.GetPanier().Lignes);
        }
    }
}
