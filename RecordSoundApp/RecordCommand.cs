namespace RecordSoundApp
{
    public class RecordCommand
    {
        private readonly Recording recording;

        public RecordCommand()
        {
            recording = new Recording();
        }

        public ResponseCommand Execute(RequestCommand request)
        {
            if (string.IsNullOrEmpty(request.Command))
            {
                throw new Exception("Command is empty.");
            }

            switch (request.Command)
            {
                case "start":
                    {
                        if (string.IsNullOrEmpty(request.Path))
                        {
                            throw new Exception("Path is required.");
                        }

                        if (recording.IsRecording)
                        {
                            throw new Exception("There is already a recording in progress.");
                        }

                        recording.StartRecording(request.Path);

                        return new ResponseCommand { Code = 200, Message = "Recording in progress.." };
                    }
                case "start-streaming":
                    {
                        if (request.Stream == null)
                        {
                            throw new Exception("Stream is required.");
                        }

                        if (request.CancellationToken == null)
                        {
                            throw new Exception("Cancellation Token is required.");
                        }

                        if (recording.IsRecording)
                        {
                            throw new Exception("There is already a recording in progress.");
                        }

                        recording.StartRecordingStreaming(request.Stream, request.CancellationToken.Value);

                        return new ResponseCommand { Code = 200, Message = "Recording in progress.." };
                    }
                case "stop":
                    {
                        if (!recording.IsRecording)
                        {
                            throw new Exception("There is no recording in progress.");
                        }

                        recording.StopRecording();

                        return new ResponseCommand { Code = 220, Message = "Recording is stopped" };
                    }
                default:
                    {
                        throw new Exception("Command not found.");
                    }
            }
        }
    }
}
