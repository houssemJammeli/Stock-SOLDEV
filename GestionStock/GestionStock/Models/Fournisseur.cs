namespace GestionStock.Models
{
    public class Fournisseur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Adresse { get; set; }

        public ICollection<Produit> Produits { get; set; }
    }
}
