using System;
using System.IO;
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

			string tournament_type;
			int _number_of_remote_players;
			
			if (args.Length != 2)
			{
				tournament_type = Console.ReadLine().Substring(1);
				_number_of_remote_players = Int32.Parse(Console.ReadLine());
				//throw new ArgumentException("Tournament type or number of remote players not specified");
			}
			else
			{
				tournament_type = args[0].Substring(1);
				_number_of_remote_players = Int32.Parse(args[1]);
			}

			List<Admin.PlayerRanking> finalRankings = Admin.AdministerTournament(tournament_type, _number_of_remote_players, port, path, 9);
			List<string> printRankings = new List<string>();
			printRankings.Add("=========== Final Rankings =============");
			int currRank = 0;
			for (int i= 0; i < finalRankings.Count; i++)
			{
				if (i != 0 && finalRankings[i].score == finalRankings[i - 1].score)
				{
					printRankings[currRank] = printRankings[currRank] + ", " + finalRankings[i].name;
				}
				else
				{
					printRankings.Add((i+1).ToString() + ": " + finalRankings[i].name);
					currRank = i + 1;
				}
			}

			printRankings.Add("========================================");

			foreach (string s in printRankings)
			{
				Console.WriteLine(s);
			}	
		}
	}
}
