using NAudio.Lame;
using NAudio.Wave;

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

        public void StopRecording()
        {
            _capture?.StopRecording();
            _writer?.Dispose();
            _mp3File?.Close();
            _capture?.Dispose();

            IsRecording = false;
        }
    }
}