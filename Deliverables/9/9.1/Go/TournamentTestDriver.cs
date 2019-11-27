using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

namespace Go
{
	class TournamentTestDriver
	{
		static void Main(string[] args)
		{
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

            Console.WriteLine("Network setup");

            string tournament_type;
			int _number_of_remote_players;
			
			if (args.Length == 2)
			{
                tournament_type = args[0].Substring(args[0].LastIndexOf('-') + 1);
                _number_of_remote_players = Int32.Parse(args[1]);
			}
			else if (args.Length == 0)
			{
                string t = Console.ReadLine();
                tournament_type = t.Substring(t.LastIndexOf('-') + 1);
                _number_of_remote_players = Int32.Parse(Console.ReadLine());
            }
            else
            {
                throw new ArgumentException("Tournament type or number of remote players not specified");
            }

            Console.WriteLine("Tournament type determined");

            List<Admin.PlayerRanking> finalRankings = Admin.AdministerTournament(tournament_type, _number_of_remote_players, port, path, 9);

            Console.WriteLine("Games Finished");

            List<string> printRankings = new List<string>();
			printRankings.Add("=========== Final Rankings =============");
			int currRank = 0;
            for (int i = 0; i < finalRankings.Count; i++)
            {
                if (i != 0 && finalRankings[i].score == finalRankings[i - 1].score)
                {
                    printRankings[printRankings.Count - 1] = printRankings[printRankings.Count - 1] + ", " + finalRankings[i].name;
                }
                else
                {
                    printRankings.Add((i + 1).ToString() + "(Score: "+ finalRankings[i].score.ToString() +  ") : " + finalRankings[i].name);
                    currRank = i + 1;
					
                }
            }

            printRankings.Add("========================================");

			foreach (string s in printRankings)
			{
				Console.WriteLine(s);
			}

            //REMOVE BEFORE SUBMISSION
            //Console.ReadLine();

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
	}
}
