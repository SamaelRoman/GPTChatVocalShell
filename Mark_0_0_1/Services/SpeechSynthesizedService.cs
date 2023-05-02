using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace Mark_0_0_1.Services
{
    public class SpeechSynthesizedService
    {
        private string key = string.Empty;
        private string location = string.Empty;
        private string SpeechSynthesisVoiceName = string.Empty;
        public SpeechSynthesizedService(string key, string location,string SpeechSynthesisVoiceName)
        {
            this.key = key;
            this.location = location;
            this.SpeechSynthesisVoiceName = SpeechSynthesisVoiceName;
        }

        private static bool OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
        {
            switch (speechSynthesisResult.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    return true;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    }
                    return false;
                default:
                    return false;
            }
        }
        public async Task<bool> Say(string text)
        {
            var speechConfig = SpeechConfig.FromSubscription(key, location);

            // The language of the voice that speaks.
            speechConfig.SpeechSynthesisVoiceName = SpeechSynthesisVoiceName;

            using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
            {
                var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
                return OutputSpeechSynthesisResult(speechSynthesisResult, text);
            }
        }
    }
}
