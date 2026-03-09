using GestionStock.Controllers;
using GestionStock.DTOs.ChatDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class ChatControllerTests
    {
        // ❌ MESSAGE VIDE
        [Fact]
        public async Task EnvoyerMessage_ReturnsBadRequest_WhenMessageIsEmpty()
        {
            var controller = new ChatController();

            var dto = new ChatRequestDto
            {
                Message = ""
            };

            var result = await controller.EnvoyerMessage(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Message vide", badRequest.Value);
        }

        // ❌ MESSAGE NULL
        [Fact]
        public async Task EnvoyerMessage_ReturnsBadRequest_WhenMessageIsNull()
        {
            var controller = new ChatController();

            var dto = new ChatRequestDto
            {
                Message = null
            };

            var result = await controller.EnvoyerMessage(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
