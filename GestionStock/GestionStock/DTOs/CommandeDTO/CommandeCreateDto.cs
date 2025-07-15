using GestionStock.Models.EnumsCommande;

namespace GestionStock.DTOs.CommandeDTO
{
    public class CommandeCreateDto
    {
        public int ClientId { get; set; }
        public decimal Total { get; set; }
        public List<LigneCommandeDto> Lignes { get; set; }
        public TypeLivraison TypeLivraison { get; set; }
    }
}
