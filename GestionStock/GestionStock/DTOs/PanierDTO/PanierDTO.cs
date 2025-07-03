namespace GestionStock.DTOs.PanierDTO
{
    public class PanierDTO
    {
        public ICollection<LignePanierDTO> Lignes { get; set; } = new List<LignePanierDTO>();
    }

    public class LignePanierDTO
    {
        public int Quantite { get; set; }
        public int ProduitId { get; set; }
    }
}
