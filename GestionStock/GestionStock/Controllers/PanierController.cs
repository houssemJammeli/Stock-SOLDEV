using GestionStock.DTOs.PanierDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionStock.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PanierController : ControllerBase
    {
        private static PanierDTO panier;

        public PanierController()
        {

        }

        public static PanierDTO GetPanier()
        {
            if (panier == null)
            {
                panier = new PanierDTO();
                return panier;
            }
            return panier;
        }

        // GET: api/Panier
        [HttpGet]
        public async Task<ActionResult<PanierDTO>> Get()
        {
            var panier = GetPanier();

            return panier;
        }

        [HttpPost]
        public async Task<IActionResult> AjouterProduitAuPanier([FromBody] LignePanierDTO dto)
        {
            if (GetPanier().Lignes.Count > 0 && GetPanier().Lignes.Any(l => l.ProduitId == dto.ProduitId))
            {
                var p = GetPanier().Lignes.FirstOrDefault(l => l.ProduitId == dto.ProduitId);
                p.Quantite += dto.Quantite;
                return Ok(panier);
            }
            GetPanier().Lignes.Add(dto);


            return Ok(panier);
        }

        [HttpDelete]
        public async Task<IActionResult> SupprimerProduitAuPanier([FromBody] LignePanierDTO dto)
        {
            GetPanier().Lignes.Remove(dto);
            return Ok(panier);
        }
    }
}
