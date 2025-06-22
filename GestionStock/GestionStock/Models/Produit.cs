namespace GestionStock.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public int QuantiteEnStock { get; set; }
        public decimal PrixUnitaire { get; set; }
        public string Categorie { get; set; }

        public ICollection<LigneCommande> LignesCommande { get; set; }
        public int FournisseurId { get; set; }
        public Fournisseur Fournisseur { get; set; }
    }
}
