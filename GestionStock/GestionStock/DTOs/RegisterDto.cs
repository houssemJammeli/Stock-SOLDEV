namespace GestionStock.DTOs
{
    public class RegisterDto
    {
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string Role { get; set; } // "Admin" ou "Client"
    }
}
