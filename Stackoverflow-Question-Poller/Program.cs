//Copyright 2014 Prescott Nasser
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Stackoverflow_Question_Poller
{
    // To learn more about Microsoft Azure WebJobs, please see http://go.microsoft.com/fwlink/?LinkID=401557
    class Program
    {
        static void Main()
        {
            var jsonQuestions = GetLatestQuestions().Result;

            foreach (var question in jsonQuestions.items)
            {
                Console.Out.WriteLine(question.title);
                Console.Out.WriteLine(question.body);
                Console.Out.WriteLine(question.link);
                Console.Out.WriteLine();
                Email.SendEmail(question.title.Value.ToString(), question.body.Value.ToString(), question.link.Value.ToString());
            }
        }

        static async Task<dynamic> GetLatestQuestions()
        {
            var newQuestionsAsOfMinutes = Int32.Parse(ConfigurationManager.AppSettings["minutes_since_asked"]);
            var tags = ConfigurationManager.AppSettings["tags"];

            using (var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
                                         | DecompressionMethods.Deflate
            }))
            {
                client.BaseAddress = new Uri("https://api.stackexchange.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var fromDate = DateTime.Now.AddDays(-newQuestionsAsOfMinutes).ToUnixTime();
                var filter = String.Format("/2.2/search?fromdate={0}&order={1}&sort={2}&site={3}&filter={4}&tagged={5}", fromDate,
                    "desc", "activity", "stackoverflow", "withbody", tags);
                
                var response = await client.GetAsync(filter);
                if (!response.IsSuccessStatusCode) return null;

                var message = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(message);
            }
        }
    }
}
