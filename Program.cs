using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CLIUtility
{
    class Program
    {
        private static readonly string API_URL = "https://trackapi.nutritionix.com/v2/";
        private static readonly string APP_ID = "262b5880";
        private static readonly string APP_KEY = "b6ee74382a6ba02211f2ea3edd4413c1";
        static async Task Main(string[] args)
        {
            
            string query = string.Empty;
            while (true)
            {
                Console.WriteLine("");               
                Console.WriteLine("What is your query for nutrition lookup? (Type q to quit.)");
                query = Console.ReadLine();
                if(query.ToLower()=="q")
                {
                    break;
                }
                await CallWebAPIAsync(query);
                
            }
        }


        private static async Task CallWebAPIAsync(string queryText)
        {
            var queryBody = JObject.FromObject(new { query = queryText });
            var client = new HttpClient();
            client.BaseAddress = new Uri(API_URL);
            client.DefaultRequestHeaders.Add("x-app-id", APP_ID);
            client.DefaultRequestHeaders.Add("x-app-key", APP_KEY);
            client.DefaultRequestHeaders.Add("x-remote-user-id", "0");
            var response = await client.PostAsync("natural/nutrients", new StringContent(queryBody.ToString(), Encoding.UTF8, "application/json"));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Root>(jsonString);
                Console.WriteLine("------------------------------------");
                PrintData(result);
            }
            else
            {
                Console.WriteLine("No record found");
            }
        }

        private static void PrintData(Root data)
        {
            PrintRow("Food Name", "Quantity","Serving Unit","Calories","Protein","Potassium");
            foreach (var item in data.foods)
            {
                PrintRow(item.food_name,item.serving_qty.ToString(),item.serving_unit.ToString(),item.nf_calories.ToString(),item.nf_protein.ToString(),item.nf_potassium.ToString());
            }
        }

        private static void PrintRow(params string[] columns)
        {
            Console.WriteLine(String.Format("{0,5}{1,20}{2,20}{3,20}{4,20}{5,20}", columns[0], columns[1], columns[2], columns[3],columns[4],columns[5]));
        }


    }

}
