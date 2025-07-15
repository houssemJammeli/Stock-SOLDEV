using GestionStock.Context;
using GestionStock.DTOs.ProduitDTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProduitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Produits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduits()
        {
            return await _context.Produits
                .Include(p => p.Fournisseur)
                .Select(p => new ProduitDto()
                {
                    Id = p.Id,
                    Nom = p.Nom,
                    Description = p.Description,
                    PrixUnitaire = p.PrixUnitaire,
                    QuantiteEnStock = p.QuantiteEnStock,
                    Categorie = p.Categorie,
                    FournisseurId = p.FournisseurId,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();
        }

        // GET: api/Produits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produit>> GetProduit(int id)
        {
            var produit = await _context.Produits
                .Include(p => p.Fournisseur)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produit == null)
                return NotFound();

            return produit;
        }

        // POST: api/Produits
        [HttpPost]
        public async Task<ActionResult<Produit>> PostProduit(Produit produit)
        {
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduit), new { id = produit.Id }, produit);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> AjouterProduitAvecImage([FromForm] ProduitUploadDto produitDto)
        {
            if (produitDto.Image == null || produitDto.Image.Length == 0)
                return BadRequest(new { message = "Aucune image reçue par le serveur." });


            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(produitDto.Image.FileName);
            var imagePath = Path.Combine("wwwroot/images", imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await produitDto.Image.CopyToAsync(stream);
            }

            var produit = new Produit
            {
                Nom = produitDto.Nom,
                Description = produitDto.Description,
                QuantiteEnStock = produitDto.QuantiteEnStock,
                PrixUnitaire = produitDto.PrixUnitaire,
                Categorie = produitDto.Categorie,
                FournisseurId = produitDto.FournisseurId,
                ImageUrl = $"images/{imageName}"
            };

            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            return Ok(produit);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduitAvecImage(int id, [FromForm] ProduitUploadDto dto)
        {
            try
            {
                var produit = await _context.Produits.FindAsync(id);
                if (produit == null)
                    return NotFound();

                produit.Nom = dto.Nom;
                produit.Description = dto.Description;
                produit.QuantiteEnStock = dto.QuantiteEnStock;
                produit.PrixUnitaire = dto.PrixUnitaire;
                produit.Categorie = dto.Categorie;
                produit.FournisseurId = dto.FournisseurId;

                if (dto.Image != null && dto.Image.Length > 0)
                {
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                    var imagePath = Path.Combine("wwwroot/images", imageName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await dto.Image.CopyToAsync(stream);
                    }

                    produit.ImageUrl = $"images/{imageName}";
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Produit mis à jour avec succès !" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erreur serveur : {ex.Message}" });
            }
        }

        /*
        // PUT: api/Produits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduit(int id, Produit produit)
        {
            if (id != produit.Id)
                return BadRequest();

            _context.Entry(produit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProduitExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        */

        // DELETE: api/Produits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduit(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
                return NotFound();

            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();

            return Ok("Produit supprimé avec succès !");
        }

        private bool ProduitExists(int id)
        {
            return _context.Produits.Any(e => e.Id == id);
        }
    }
}
