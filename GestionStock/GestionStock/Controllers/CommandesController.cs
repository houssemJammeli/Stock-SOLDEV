using GestionStock.Context;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CommandesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommandesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("panier/valider")]
        public async Task<IActionResult> ValiderPanier([FromBody] int clientId)
        {
            var panier = await _context.Commandes
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Produit)
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.DateVente == null);

            if (panier == null)
                return NotFound("Panier introuvable");

            // Vérifie si le panier contient des produits
            if (panier.LignesCommande == null || !panier.LignesCommande.Any())
                return BadRequest("Le panier est vide");

            // Mettre à jour DateVente
            panier.DateVente = DateTime.Now;

            // Facultatif : Mettre à jour le stock des produits
            foreach (var ligne in panier.LignesCommande)
            {
                var produit = await _context.Produits.FindAsync(ligne.ProduitId);
                if (produit != null)
                {
                    produit.QuantiteEnStock -= ligne.Quantite;
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Commande validée avec succès !");
        }




        // GET: api/Commandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Produit)
                .ToListAsync();
        }

        // GET: api/Commandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(lc => lc.Produit)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return NotFound();

            return commande;
        }

        // POST: api/Commandes
        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommande), new { id = commande.Id }, commande);
        }

        // PUT: api/Commandes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommande(int id, Commande commande)
        {
            if (id != commande.Id)
                return BadRequest();

            _context.Entry(commande).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommandeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Commandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            _context.Commandes.Remove(commande);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommandeExists(int id)
        {
            return _context.Commandes.Any(e => e.Id == id);
        }
    }
}
