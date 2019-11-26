using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using System.Net;
using System.Net.Sockets;

namespace PlayerSpace
{
    static class PlayerProxyTestDriver
    {
        static void Main(string[] args)
        {
            string console = "";
            string input;

            //Read from console
            while ((input = Console.ReadLine()) != null)
                console += input;

            List<JToken> finalList = new List<JToken>();

            string go_player = File.ReadAllText("go-player.config");
            string go = File.ReadAllText("go.config");

            JObject ipPort = JsonConvert.DeserializeObject<JObject>(go);
            JObject depth = JsonConvert.DeserializeObject<JObject>(go_player);


            //Network setup
            IPAddress ipAddr = IPAddress.Parse(ipPort["IP"].ToObject<string>());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, ipPort["port"].ToObject<int>());

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);


            //Create remote player
            PlayerClient client = new PlayerClient(ipPort["IP"].ToObject<string>(),
                ipPort["port"].ToObject<int>(), "less dumb", 1, "no name");

            PlayerAdapter aiPlayer = new PlayerAdapter(socket);

            //Parse console input while testing
            JsonTextReader reader = new JsonTextReader(new StringReader(console));
            reader.SupportMultipleContent = true;
            JsonSerializer serializer = new JsonSerializer();

            JToken toAdd;
            while (true)
            {
                //Parse console input while testing
                if (!reader.Read())
                    break;
                JToken jtoken = serializer.Deserialize<JToken>(reader);

                try
                {
                    toAdd = aiPlayer.JsonCommand(jtoken, "no name");
                    if (toAdd.Type != JTokenType.Null)
                        finalList.Add(toAdd);
                }
                catch (InvalidJsonInputException)
                {
                    finalList.Add("GO has gone crazy!");
                    break;
                }
            }

            Console.WriteLine(JsonConvert.SerializeObject(finalList));

            Console.ReadLine();
        }
    }
}
