using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

namespace LocalPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            string go = File.ReadAllText("go.config");
            JObject config = JsonConvert.DeserializeObject<JObject>(go);

            PlayerClient player = new PlayerClient(config["IP"].ToObject<string>(), config["port"].ToObject<int>());
            while (true)
            {

            }
        }
    }
}
