namespace GestionStock.DTOs.CommandeDTO
{
    public class CommandeDto
    {
        public int Id { get; set; }
        public DateTime? DateVente { get; set; }
        public decimal Total { get; set; }
        public string NomClient { get; set; }
        public string EmailClient { get; set; }
        public string EtatCommande { get; set; }
    }
}
