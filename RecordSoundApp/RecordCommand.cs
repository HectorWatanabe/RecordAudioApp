namespace RecordSoundApp
{
    public class RecordCommand
    {
        private readonly Recording recording;

        public RecordCommand()
        {
            recording = new Recording();
        }

        public string Execute(string command, string path)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new Exception("Command is empty.");
            }

            switch (command)
            {
                case "start":
                    {
                        if (recording.IsRecording)
                        {
                            throw new Exception("There is already a recording in progress.");
                        }

                        recording.StartRecording(path);

                        return "Recording in progress..";
                    }
                case "stop":
                    {
                        if (!recording.IsRecording)
                        {
                            throw new Exception("There is no recording in progress.");
                        }

                        recording.StopRecording();

                        return "Recording is stopped";
                    }
                default:
                    {
                        throw new Exception("Command not found.");
                    }
            }
        }
    }
}
