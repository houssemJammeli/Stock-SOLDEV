using GestionStock.Context;
using GestionStock.DTOs.IARecommandationDTOs;
using GestionStock.Services.IA;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IAController : ControllerBase
    {
        private readonly IAService _iaService;
        private readonly ApplicationDbContext _context;

        public IAController(IAService iaService, ApplicationDbContext context)
        {
            _iaService = iaService;
            _context = context;
        }

        [HttpPost("recommandation")]
        public async Task<IActionResult> Recommandation(
    [FromBody] IARecommandationRequestDto dto)
        {
            try
            {
                var iaResult = await _iaService.RecommanderAsync(dto);

                if (iaResult?.Produits == null || !iaResult.Produits.Any())
                    return Ok(new { imc = iaResult?.Imc, produits = new List<object>() });

                var produits = _context.Produits
                    .Where(p => iaResult.Produits.Contains(p.Id))
                    .ToList();

                return Ok(new
                {
                    imc = iaResult.Imc,
                    produits
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Erreur appel IA",
                    details = ex.Message
                });
            }
        }
    }

}
