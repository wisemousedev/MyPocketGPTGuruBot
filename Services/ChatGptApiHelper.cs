using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MyPocketGPTGuruBot.Services
{
    public class ChatGptApiHelper
    {

        private readonly string _apiUrl = "https://api.openai.com/v1/engines/text-davinci-002/completions";

        private readonly string _apiToken;

        public ChatGptApiHelper(string apiToken)
        {
            _apiToken = apiToken;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

            string prompt = $"User: {message}\nAI:";
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                prompt = prompt,
                max_tokens = 4000,
                n = 1,
                stop = (string)null
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(_apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: ChatGPT API request failed. Status code: {response.StatusCode}");
                Console.WriteLine($"Error: Response content: {await response.Content.ReadAsStringAsync()}");
                throw new Exception("ChatGPT API request failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return parsedResponse.choices[0].text.ToString().Trim();
        }


    }




}
