using System.Text.Json.Serialization;

namespace GestionStock.DTOs.ChatDTOs
{
    public class ChatResponseDto
    {
        [JsonPropertyName("reponse")]
        public string Reponse { get; set; }
    }
}
