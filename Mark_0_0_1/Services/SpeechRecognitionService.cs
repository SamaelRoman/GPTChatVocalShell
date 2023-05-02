using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mark_0_0_1.Services
{
    public class SpeechRecognitionService : IDisposable
    {
        private bool _disposed = false;
        private SpeechConfig speechConfig;
        private AudioConfig audioConfig;
        private SpeechRecognizer speechRecognizer;
        public SpeechRecognitionService(string key, string location,string SpeechRecognitionLanguage)
        {
            speechConfig = SpeechConfig.FromSubscription(key, location);
            speechConfig.SpeechRecognitionLanguage = SpeechRecognitionLanguage;
            audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        }
        private static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    //Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    }
                    break;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                speechRecognizer.Dispose();
                _disposed = true;
            }
        }

        public async Task<string> listen()
        {
            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

            OutputSpeechRecognitionResult(speechRecognitionResult);

            return speechRecognitionResult.Text;
        }

    }
}
