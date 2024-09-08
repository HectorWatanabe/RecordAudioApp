using NAudio.Lame;
using NAudio.Wave;
using System.Net.Sockets;

namespace RecordSoundApp
{
    public class Recording
    {
        private WasapiLoopbackCapture? _capture;
        private LameMP3FileWriter? _writer;
        private FileStream? _mp3File;

        public bool IsRecording { get; set; }

        public Recording()
        {
            _capture = null;
            _writer = null;
            _mp3File = null;

            IsRecording = false;
        }

        public void StartRecording(string path)
        {
            _capture = new WasapiLoopbackCapture();
            _mp3File = new FileStream(path, FileMode.Create);
            _writer = new LameMP3FileWriter(_mp3File, _capture.WaveFormat, LAMEPreset.VBR_90);

            _capture.DataAvailable += (s, a) =>
            {
                _writer.Write(a.Buffer, 0, a.BytesRecorded);

            };

            _capture.StartRecording();

            IsRecording = true;
        }

        public void StartRecordingStreaming(NetworkStream stream, CancellationToken cancellationToken)
        {
            _capture = new WasapiLoopbackCapture();

            _capture.DataAvailable += async (s, a) =>
            {
                try
                {
                    await Utils.SendResponse(stream, new { Code = 210, Message = "Send Data", Data = a.Buffer }, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    StopRecording();
                }
            };

            _capture.StartRecording();

            IsRecording = true;
        }

        public void StopRecording()
        {
            IsRecording = false;
            _capture?.StopRecording();
            _writer?.Dispose();
            _mp3File?.Close();
            _capture?.Dispose();
        }
    }
}