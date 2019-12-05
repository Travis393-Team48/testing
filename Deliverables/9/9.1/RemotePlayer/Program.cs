using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

namespace RemotePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            string go = File.ReadAllText("go.config");
            JObject config = JsonConvert.DeserializeObject<JObject>(go);
			Console.Write("name: ");
	        String name = Console.ReadLine();
            PlayerClient player = new PlayerClient(config["IP"].ToObject<string>(), config["port"].ToObject<int>(), "smart", 1, name);
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
