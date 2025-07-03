namespace GestionStock.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime? DateVente { get; set; }
        public int ClientId { get; set; }
        public decimal Total { get; set; }

        public Utilisateur Client { get; set; }
        public ICollection<LigneCommande> LignesCommande { get; set; }
    }
}
