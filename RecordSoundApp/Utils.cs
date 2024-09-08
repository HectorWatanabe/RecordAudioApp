using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RecordSoundApp
{
    public class Utils
    {
        public static async Task SendResponse(NetworkStream stream, object response, CancellationToken stoppingToken)
        {
            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

            string messageToClient = JsonSerializer.Serialize(response, options);

            byte[] messageBytes = Encoding.UTF8.GetBytes(messageToClient);

            await stream.WriteAsync(messageBytes, stoppingToken);
        }
    }
}
