using GPTChatVocalShell;
using Mark_0_0_1.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Drawing.Text;
using System.Reflection;
using System.Resources;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        #region Configurating
        //create configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Set this key on appsettings.json file you can take this key on https://platform.openai.com/account/api-keys
        string OpenAIKey = configuration["OpenAIKey"];

        //  Set this key on appsettings.json file you can take this key on https://portal.azure.com/
        //  Create tranlator service. more information on link: https://learn.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-translator?tabs=csharp
        string Microsoft_Azure_Translator_Key = configuration["Microsoft_Azure_Translator_Key"];

        // Set this property on appsettings.json file you can take this property on https://portal.azure.com/
        // Create tranlator service. more information on link: https://learn.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-translator?tabs=csharp
        string Microsoft_Azure_Translator_Region = configuration["Microsoft_Azure_Translator_Region"];

        //Set this key on appsettnigs.json file you can take this key on https://portal.azure.com/
        //Create Speech Service. more information on link: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-recognize-speech?pivots=programming-language-csharp
        //You can use Speech Service for recognitision or Synthesizing voice.
        string Microsoft_Azure_SpeechService_Key = configuration["Microsoft_Azure_SpeechService_Key"];

        //Set this property on appsettnigs.json file you can take this key on https://portal.azure.com/
        //Create Speech Service. more information on link: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-recognize-speech?pivots=programming-language-csharp
        //You can use Speech Service for recognitision or Synthesizing voice.
        string Microsoft_Azure_SpeechService_Region = configuration["Microsoft_Azure_SpeechService_Region"];

        //Set this property on appsettings.json file 
        //Learn more about this on link: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=tts
        string SpeechSynthesisVoiceName = configuration["SpeechSynthesisVoiceName"];

        //Set this property on appsettings.json file
        //Learn more about supported languages on link: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt
        string SpeechRecognitionLanguage = configuration["SpeechRecognitionLanguage"];

        //Set this property on appsetting.json file
        //Learn more about supported languages on link: https://learn.microsoft.com/en-us/azure/cognitive-services/translator/language-support
        string SpeechTranslatorLanguage = configuration["SpeechTranslatorLanguage"];
        #endregion


        AIClientService aIClient = new AIClientService(OpenAIKey);

        TranslatorService translator = new TranslatorService(Microsoft_Azure_Translator_Key, Microsoft_Azure_Translator_Region);

        SpeechSynthesizedService speechService = new SpeechSynthesizedService(Microsoft_Azure_SpeechService_Key, Microsoft_Azure_SpeechService_Region, SpeechSynthesisVoiceName);

        #region getting resources and creating variables for use in request

        //Name of Vocal shell used on speech command for started ask
        string NameOfVocalShell = Resource.NameOfVocalShell;
        //word on speech command to start ask
        string SpeechСommandWord = Resource.SpeechСommandWord;
        //full speech command to start ask
        string SpeechСommand = $"{NameOfVocalShell} {SpeechСommandWord}";

        string GreetingWord = Resource.GreetingWord;

        string Greeting = $"{NameOfVocalShell}:  {GreetingWord}?";

        string You = Resource.You + ':';

        string CloseCommandWord = Resource.CloseCommandWord;

        string CloseCommand = $"{NameOfVocalShell} {CloseCommandWord}";

        
        string Question = string.Empty;
        string Result = string.Empty;

        #endregion

        Console.OutputEncoding = UTF8Encoding.UTF8;
        //we go into a closed loop for processing requests
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Greeting);
            Console.ForegroundColor= ConsoleColor.White;
            Console.Write(You);

            //processing voice to text
            //we listening and waiting speech command for start process
            //I see trouble with this loop, but i can't fix this now
            //Maybe we can use listening on specific word on this service
            bool SpeechСommandSpoken = false;
            while (SpeechСommandSpoken == false) {
                SpeechRecognitionService recognitionService = new SpeechRecognitionService(Microsoft_Azure_SpeechService_Key, Microsoft_Azure_SpeechService_Region, SpeechRecognitionLanguage);
                Question = await recognitionService.listen();
                if (Question.ToLower().Contains(CloseCommand))
                {
                    Environment.Exit(0);
                }
                if(Question.ToLower().Contains(SpeechСommand))
                {
                    SpeechСommandSpoken = true;
                    var Index = Question.IndexOf(SpeechСommand) + SpeechСommand.Length;
                    Question = Question.Remove(0, Index + 1);
                    recognitionService.Dispose();

                }
                else
                {
                    Question = string.Empty;
                    recognitionService.Dispose();
                }
            }
            Console.WriteLine($"{Question}");
            Console.ForegroundColor = ConsoleColor.Green;

            //we translate question into the english language
            Question = await translator.TranslateAsync(SpeechTranslatorLanguage, "en", Question);

            //Sending request to GPT chat
            var GPT_3_Answer = await aIClient.SendAsync(Question);

            //translate answer for desired language
            Result = await translator.TranslateAsync("en",SpeechTranslatorLanguage, GPT_3_Answer);

            //output the result to the speakers
            bool speechSynthesisEnd = await speechService.Say(Result);

            //output the result into console
            if (speechSynthesisEnd)
            {
                Console.WriteLine($"{NameOfVocalShell}: " + Result);
            }

        }
    }
}