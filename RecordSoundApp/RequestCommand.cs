using System.Net.Sockets;

namespace RecordSoundApp
{
    public class RequestCommand
    {
        public string? Command { get; set; }
        public string? Path { get; set; }
        public NetworkStream? Stream { get; set; }
        public CancellationToken? CancellationToken { get; set; }
    }
}
