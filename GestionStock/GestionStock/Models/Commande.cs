using GestionStock.Models.EnumsCommande;

namespace GestionStock.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime? DateVente { get; set; }
        public int ClientId { get; set; }
        public decimal Total { get; set; }

        public TypeLivraison TypeLivraison { get; set; }
        public EtatCommande EtatCommande { get; set; }
        public DateTime DateLivraisonPrevue { get; set; }

        public Utilisateur Client { get; set; }
        public ICollection<LigneCommande> LignesCommande { get; set; }
    }
}
