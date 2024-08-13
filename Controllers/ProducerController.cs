using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace AzbusProducer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ProducerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ServiceBus:ConnectionString").Value;
            _queueName = configuration.GetSection("ServiceBus:QueueName").Value;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusSender sender = client.CreateSender(_queueName);

            try
            {
                ServiceBusMessage busMessage = new ServiceBusMessage(message);
                await sender.SendMessageAsync(busMessage);
                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}