using GestionStock.Context;
using GestionStock.DTOs.CommandeDTO;
using GestionStock.Models;
using GestionStock.Models.EnumsCommande;
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

        [HttpPost]
        public async Task<IActionResult> CreerCommande([FromBody] CommandeCreateDto dto)
        {
            if (!_context.Utilisateurs.Any(u => u.Id == dto.ClientId))
                return BadRequest("Client non trouvé");

            var dateVente = DateTime.Now;

            int joursDelai = dto.TypeLivraison switch
            {
                TypeLivraison.Standard => 7,
                TypeLivraison.Express => 3,
                _ => 7
            };

            var commande = new Commande
            {
                ClientId = dto.ClientId,
                DateVente = dateVente,
                Total = dto.Total,
                TypeLivraison = dto.TypeLivraison,
                EtatCommande = EtatCommande.Preparation, // état initial
                DateLivraisonPrevue = dateVente.AddDays(joursDelai),
                LignesCommande = dto.Lignes.Select(l => new LigneCommande
                {
                    ProduitId = l.ProduitId,
                    Quantite = l.Quantite
                }).ToList()
            };

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return Ok(new { commande.Id });
        }

        private EtatCommande CalculerEtatCommande(Commande commande)
        {
            var joursDepuisVente = (DateTime.Now - commande.DateVente)?.TotalDays ?? 0;

            if (commande.TypeLivraison == TypeLivraison.Standard)
            {
                if (joursDepuisVente < 3)
                    return EtatCommande.Preparation;
                else if (joursDepuisVente < 6)
                    return EtatCommande.EnCoursLivraison;
                else
                    return EtatCommande.Livree;
            }
            else if (commande.TypeLivraison == TypeLivraison.Express)
            {
                if (joursDepuisVente < 1)
                    return EtatCommande.Preparation;
                else if (joursDepuisVente < 2)
                    return EtatCommande.EnCoursLivraison;
                else
                    return EtatCommande.Livree;
            }

            return EtatCommande.Preparation;
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandesClient(int clientId)
        {
            var commandes = await _context.Commandes
                .Include(c => c.LignesCommande)
                .Where(c => c.ClientId == clientId)
                .ToListAsync();

            foreach (var commande in commandes)
            {
                var nouvelEtat = CalculerEtatCommande(commande);
                if (commande.EtatCommande != nouvelEtat)
                {
                    commande.EtatCommande = nouvelEtat;
                    _context.Entry(commande).Property(c => c.EtatCommande).IsModified = true;
                }
            }

            await _context.SaveChangesAsync();

            return commandes;
        }

        [HttpGet("admin/all")]
        public async Task<ActionResult<IEnumerable<CommandeDto>>> GetToutesLesCommandes()
        {
            var commandes = await _context.Commandes
                .Include(c => c.Client)
                .ToListAsync();

            var commandesDto = commandes.Select(c => new CommandeDto
            {
                Id = c.Id,
                DateVente = c.DateVente,
                Total = c.Total,
                NomClient = c.Client.Nom,
                EmailClient = c.Client.Email,
                EtatCommande = c.EtatCommande.ToString()
            }).ToList();

            return Ok(commandesDto);
        }

        /*
        [HttpPost]
        public async Task<IActionResult> CreerCommande([FromBody] CommandeCreateDto dto)
        {
            if (!_context.Utilisateurs.Any(u => u.Id == dto.ClientId))
                return BadRequest("Client non trouvé");

            var commande = new Commande
            {
                ClientId = dto.ClientId,
                DateVente = DateTime.Now,
                Total = dto.Total,
                LignesCommande = dto.Lignes.Select(l => new LigneCommande
                {
                    ProduitId = l.ProduitId,
                    Quantite = l.Quantite
                }).ToList()
            };

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return Ok(new { commande.Id });
        }
        */

        /*
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
        */




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

        /*
        // POST: api/Commandes
        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommande), new { id = commande.Id }, commande);
        }
        */

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
