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
    public class FournisseursController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FournisseursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Fournisseurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fournisseur>>> GetFournisseurs()
        {
            return await _context.Fournisseurs
                .Include(f => f.Produits)
                .ToListAsync();
        }

        // GET: api/Fournisseurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fournisseur>> GetFournisseur(int id)
        {
            var fournisseur = await _context.Fournisseurs
                .Include(f => f.Produits)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fournisseur == null)
                return NotFound();

            return fournisseur;
        }

        // POST: api/Fournisseurs
        [HttpPost]
        public async Task<ActionResult<Fournisseur>> PostFournisseur(Fournisseur fournisseur)
        {
            _context.Fournisseurs.Add(fournisseur);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFournisseur), new { id = fournisseur.Id }, fournisseur);
        }

        // PUT: api/Fournisseurs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFournisseur(int id, Fournisseur fournisseur)
        {
            if (id != fournisseur.Id)
                return BadRequest();

            _context.Entry(fournisseur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FournisseurExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Fournisseurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFournisseur(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null)
                return NotFound();

            _context.Fournisseurs.Remove(fournisseur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FournisseurExists(int id)
        {
            return _context.Fournisseurs.Any(f => f.Id == id);
        }
    }
}
