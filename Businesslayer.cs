using Newtonsoft.Json;
using System.Collections.Generic;

namespace SvtRoboticsTakehome
{
    public class Businesslayer
    {
        public List<bots>? Getbots()
        {
            var url = "https://60c8ed887dafc90017ffbd56.mockapi.io/robots";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var botlist = JsonConvert.DeserializeObject<List<bots>>(result);
                return botlist;
            }
            else return null;
        }
    }
}
