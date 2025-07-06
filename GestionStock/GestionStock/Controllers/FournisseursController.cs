using GestionStock.Context;
using GestionStock.DTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Controllers
{
    [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FournisseurDto>>> GetFournisseurs()
        {
            var fournisseurs = await _context.Fournisseurs
                .Select(f => new FournisseurDto
                {
                    Id = f.Id,
                    Nom = f.Nom,
                    Email = f.Email,
                    Telephone = f.Telephone,
                    Adresse = f.Adresse
                })
                .ToListAsync();

            return Ok(fournisseurs);
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
        public async Task<ActionResult<FournisseurDto>> CreateFournisseur(FournisseurCreateDto dto)
        {
            var fournisseur = new Fournisseur
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Adresse = dto.Adresse
            };

            _context.Fournisseurs.Add(fournisseur);
            await _context.SaveChangesAsync();

            var result = new FournisseurDto
            {
                Id = fournisseur.Id,
                Nom = fournisseur.Nom,
                Email = fournisseur.Email,
                Telephone = fournisseur.Telephone,
                Adresse = fournisseur.Adresse
            };

            return CreatedAtAction(nameof(GetFournisseurs), new { id = fournisseur.Id }, result);
        }

        // PUT: api/Fournisseurs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFournisseur(int id, FournisseurUpdateDto dto)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur is null) return NotFound();

            fournisseur.Nom = dto.Nom;
            fournisseur.Email = dto.Email;
            fournisseur.Telephone = dto.Telephone;
            fournisseur.Adresse = dto.Adresse;

            await _context.SaveChangesAsync();
            return NoContent();                      // 204 si tout va bien
        }

        // DELETE: api/Fournisseurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFournisseur(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur is null)
                return NotFound();

            _context.Fournisseurs.Remove(fournisseur);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 = suppression OK
        }

        private bool FournisseurExists(int id)
        {
            return _context.Fournisseurs.Any(f => f.Id == id);
        }
    }
}
