using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Mark_0_0_1.Services
{
    public class AIClientService
    {
        private HttpClient client;
        private readonly string endpoint = "https://api.openai.com/v1/completions";
        private string APIKey = string.Empty;
        private string RequestBodySignature = "{" +
            "\"model\": \"text-davinci-003\"," +
            "\"prompt\":\"*****\"," +
            "\"temperature\": 1 ," +
            "\"max_tokens\": 100 " +
            "}";

        public AIClientService(string APIKey)
        {

            this.APIKey = APIKey;
        }
        public async Task<string> SendAsync(string Text)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", $"Bearer {APIKey}");
            string contentText = RequestFormmating(Text);
            StringContent content = new StringContent(contentText, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(endpoint, content);

            var responseMessage = await response.Content.ReadAsStringAsync();

            return ResponseFormmating(responseMessage);

        }
        private string ResponseFormmating(string responseMessage)
        {
            if (responseMessage == null || string.IsNullOrEmpty(responseMessage)) { throw new ArgumentNullException(); }
            try
            {
                var dynamicResult = JsonConvert.DeserializeObject<dynamic>(responseMessage);

                return dynamicResult.choices[0].text;
            }
            catch (Exception ex) { throw new Exception("We have a trouble with deserialize object. see more:" + ex.Message); }
        }
        private string RequestFormmating(string Text)
        {
            return RequestBodySignature.Replace("*****", Text);
        }
    }
}
