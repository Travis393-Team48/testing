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
				throw new ArgumentException("Tournament type or number of remote players not specified");
			}
			else
			{
				tournament_type = args[0];
				_number_of_remote_players = args[1];
			}

			List<string> finalRankings = Admin.AdministerTournament(tournament_type, _number_of_remote_players, port, path, 9);
			

			//Console.WriteLine(JsonConvert.SerializeObject(finalRankings.ToArray()));
		}
	}
}
