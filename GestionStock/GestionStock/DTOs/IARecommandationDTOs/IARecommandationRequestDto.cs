namespace GestionStock.DTOs.IARecommandationDTOs
{
    public class IARecommandationRequestDto
    {
        public double Poids { get; set; }
        public double Taille { get; set; }
        public int Objectif { get; set; } // 0:santé, 1:masse, 2:perte poids
        public int Niveau { get; set; }   // 0:débutant, 1:intermédiaire, 2:avancé
    }
}
