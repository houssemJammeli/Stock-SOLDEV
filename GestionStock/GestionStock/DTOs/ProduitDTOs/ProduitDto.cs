namespace GestionStock.DTOs.ProduitDTOs
{
    public class ProduitDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public int QuantiteEnStock { get; set; }
        public decimal PrixUnitaire { get; set; }
        public string Categorie { get; set; }
        public int FournisseurId { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
