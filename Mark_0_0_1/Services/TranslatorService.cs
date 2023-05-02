using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mark_0_0_1.Services
{
    public class TranslatorService
    {
        private string endpoint = "https://api.cognitive.microsofttranslator.com";
        HttpClient client;
        HttpRequestMessage request;
        private string key = string.Empty;
        private string location = string.Empty;
        public TranslatorService(string key, string location)
        {
            this.key = key;
            this.location = location;
        }
        private string ResponseFormmating(string responseMessage)
        {
            if (responseMessage == null || string.IsNullOrEmpty(responseMessage)) { throw new ArgumentNullException(); }
            try
            {
                var dynamicResult = JsonConvert.DeserializeObject<dynamic>(responseMessage);

                return dynamicResult[0].translations[0].text;
            }
            catch (Exception ex) { throw new Exception("We have a trouble with deserialize object. see more:" + ex.Message); }
        }
        public async Task<string> TranslateAsync(string from, string to, string textToTranslate)
        {
            client = new HttpClient();
            request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.Headers.Add("Ocp-Apim-Subscription-Key", key.ToLower());
            request.Headers.Add("Ocp-Apim-Subscription-Region", location.ToLower());


            request.RequestUri = new Uri(endpoint + $"/translate?api-version=3.0&from={from}&to={to}");

            object[] body = new object[] { new { Text = textToTranslate } };

            var requestBody = JsonConvert.SerializeObject(body);

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            string result = await response.Content.ReadAsStringAsync();

            result = ResponseFormmating(result);
            client.Dispose();
            request.Dispose();

            return result;
        }
    }
}
