namespace GestionStock.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string Adresse { get; set; }
        public Role Role { get; set; }

        public ICollection<Commande> Commandes { get; set; }
    }
}
