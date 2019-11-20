using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;
using RefereeSpace;
using System.Net;
using System.Net.Sockets;

namespace Go
{
    class AdminTestDriver
    {
        static void Main(string[] args)
        {
            string go = File.ReadAllText("go.config");
            JObject config = JsonConvert.DeserializeObject<JObject>(go);

            string ip = config["IP"].ToObject<string>();
            int port = config["port"].ToObject<int>();
            string path = config["default-player"].ToObject<string>();

            //Create remote player (will need explicitly create a remote player to connect to this)
            PlayerWrapper player1 = new PlayerWrapper(port);

            //Create local player
            PlayerWrapper player2 = new PlayerWrapper("less dumb");

            List<string> victors = Admin.AdministerGame(player1, "remote player", player2, "local player", 9);
            Console.WriteLine(JsonConvert.SerializeObject(victors.ToArray()));

            Console.ReadLine();
        }
    }
}
