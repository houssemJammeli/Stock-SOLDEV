using GestionStock.Context;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LigneCommandesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LigneCommandesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LigneCommandes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LigneCommande>>> GetLignesCommande()
        {
            return await _context.LignesCommande
                .Include(lc => lc.Produit)
                .Include(lc => lc.Commande)
                .ToListAsync();
        }

        // GET: api/LigneCommandes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LigneCommande>> GetLigneCommande(int id)
        {
            var ligne = await _context.LignesCommande
                .Include(lc => lc.Produit)
                .Include(lc => lc.Commande)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (ligne == null)
                return NotFound();

            return ligne;
        }

        // POST: api/LigneCommandes
        [HttpPost]
        public async Task<ActionResult<LigneCommande>> PostLigneCommande(LigneCommande ligne)
        {
            _context.LignesCommande.Add(ligne);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLigneCommande), new { id = ligne.Id }, ligne);
        }

        // PUT: api/LigneCommandes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLigneCommande(int id, LigneCommande ligne)
        {
            if (id != ligne.Id)
                return BadRequest();

            _context.Entry(ligne).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LigneCommandeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/LigneCommandes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLigneCommande(int id)
        {
            var ligne = await _context.LignesCommande.FindAsync(id);
            if (ligne == null)
                return NotFound();

            _context.LignesCommande.Remove(ligne);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LigneCommandeExists(int id)
        {
            return _context.LignesCommande.Any(l => l.Id == id);
        }
    }
}
