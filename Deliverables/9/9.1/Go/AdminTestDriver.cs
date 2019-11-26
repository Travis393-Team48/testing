using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

namespace Go
{
    class AdminTestDriver
    {
        static void Main(string[] args)
        {
            //Read from config
            string go = File.ReadAllText("go.config");
            JObject config = JsonConvert.DeserializeObject<JObject>(go);

            string ip = config["IP"].ToObject<string>();
            int port = config["port"].ToObject<int>();
            string path = config["default-player"].ToObject<string>();


            //Network setup
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            Admin.Socket = socket;

            //Create remote player (will need explicitly create a remote player to connect to this)
            PlayerWrapper player1 = new PlayerWrapper(socket);


            string goPlayer = File.ReadAllText(path);
            JObject player = JsonConvert.DeserializeObject<JObject>(goPlayer);

            int depth = player["depth"].ToObject<int>();

            //Create local player
            PlayerWrapper player2 = new PlayerWrapper("less dumb", depth);


            List<string> victors = Admin.AdministerSingleGame(player1, "remote player", player2, "local player", 9);

            Console.WriteLine(JsonConvert.SerializeObject(victors.ToArray()));

            //Console.ReadLine();

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
