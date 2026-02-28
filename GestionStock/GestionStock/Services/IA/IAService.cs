using GestionStock.DTOs.IARecommandationDTOs;
using System.Text;
using System.Text.Json;

namespace GestionStock.Services.IA
{
    public class IAService
    {
        private readonly HttpClient _httpClient;

        public IAService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IARecommandationResponseDto> RecommanderAsync(IARecommandationRequestDto dto)
        {
            var json = JsonSerializer.Serialize(dto);

            var response = await _httpClient.PostAsync(
                "http://127.0.0.1:8000/recommandation",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur IA Python : {content}");
            }

            return JsonSerializer.Deserialize<IARecommandationResponseDto>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
