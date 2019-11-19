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

            PlayerClientRaw player = new PlayerClientRaw(config["IP"].ToObject<string>(), config["port"].ToObject<int>(), "disconnect", 1, "no name");
            while (true)
            {
                if (player.IsConnected())
                {
                    while (true)
                    {
                        if (!player.IsConnected())
                            break;
                    }
                    break;
                }
                    
            }
        }
    }
}
