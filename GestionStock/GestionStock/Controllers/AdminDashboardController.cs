using GestionStock.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var commandes = await _context.Commandes.CountAsync();
            var ventes = await _context.LignesCommande.SumAsync(l => l.Quantite);
            var profit = await _context.LignesCommande.SumAsync(l => l.Produit.PrixUnitaire * l.Quantite); // à adapter
            var revenu = profit; // si pas de taxe/marge définie

            return Ok(new
            {
                commandes,
                ventes,
                profit,
                revenu
            });
        }

        [HttpGet("ventes-recentes")]
        public async Task<IActionResult> GetVentesRecentes()
        {
            var ventes = await _context.Commandes
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Produit)
                .OrderByDescending(c => c.DateVente)
                .Take(10)
                .Select(c => new
                {
                    date = c.DateVente,
                    client = c.Client.Nom,
                    produit = string.Join(" - ", c.LignesCommande.Select(x => x.Produit.Nom)),
                    prix = c.Total,
                }).ToListAsync();

            return Ok(ventes);
        }

        [HttpGet("top-produits")]
        public async Task<IActionResult> GetTopProduits()
        {
            var topProduits = await _context.LignesCommande
                .GroupBy(l => l.Produit.Nom)
                .Select(g => new
                {
                    nom = g.Key,
                    quantiteVendue = g.Sum(x => x.Quantite)
                })
                .OrderByDescending(x => x.quantiteVendue)
                .Take(10)
                .ToListAsync();

            return Ok(topProduits);
        }
    }
}
