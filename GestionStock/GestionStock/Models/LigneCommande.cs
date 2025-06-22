namespace GestionStock.Models
{
    public class LigneCommande
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public int ProduitId { get; set; }
        public int Quantite { get; set; }

        public Commande Commande { get; set; }
        public Produit Produit { get; set; }
    }
}
