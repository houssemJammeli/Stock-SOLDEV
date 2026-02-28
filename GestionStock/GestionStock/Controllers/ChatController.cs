using GestionStock.DTOs.ChatDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace GestionStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatController()
        {
            _httpClient = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> EnvoyerMessage([FromBody] ChatRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Message vide");

            var pythonApiUrl = "http://127.0.0.1:8001/chat";

            var json = JsonSerializer.Serialize(new { message = dto.Message });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(pythonApiUrl, content);

            if (!response.IsSuccessStatusCode)
                return StatusCode(500, "Erreur IA");

            var responseJson = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<ChatResponseDto>(responseJson);
            return Ok(chatResponse);
        }
    }
}
