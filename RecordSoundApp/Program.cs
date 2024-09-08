using Microsoft.Extensions.Configuration;
using NAudio.Lame;
using NAudio.Wave;

namespace AudioRecorder
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var filePath = config["ApplicationSettings:FilePath"];

            if (string.IsNullOrEmpty(filePath)) throw new Exception("File Path is required.");

            var fileName = $"{Guid.NewGuid()}.mp3";

            var outputPath = string.Concat(filePath, "\\", fileName);

            var capture = new WasapiLoopbackCapture();

            using (var mp3File = new FileStream(outputPath, FileMode.Create))
            {
                using var writer = new LameMP3FileWriter(mp3File, capture.WaveFormat, LAMEPreset.VBR_90);

                capture.DataAvailable += (s, a) =>
                {
                    writer.Write(a.Buffer, 0, a.BytesRecorded);
                };

                capture.StartRecording();

                Console.WriteLine("Grabando... presiona Enter para detener la grabación.");

                Console.ReadLine();

                capture.StopRecording();
            }

            Console.WriteLine("Grabación finalizada. Archivo guardado en " + outputPath);
        }
    }
}
