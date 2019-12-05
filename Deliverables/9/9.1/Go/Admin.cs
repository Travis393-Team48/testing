using System;
using System.IO;
using System.Collections.Generic;
using CustomExceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PlayerSpace;
using RefereeSpace;
using System.Net.Sockets;

namespace Go
{
    /*
     * Provides functions to administer games
     */
    public static class Admin
    {
        public static Socket Socket;

        /* Given two UNREGISTERED players and the size of the game board, administers a game between the two players using referee
         * Returns a sorted list of strings containing the victor(s)
         * if a player is a remote player, the name given in function for that player doesn't matter
         */
        public static List<string> AdministerSingleGame(PlayerWrapper player1, string name1, PlayerWrapper player2, string name2, int size)
        {
            try
            {
                player1.Register(name1); //Assign player 2
            }
            catch (Exception e)
            {
                if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                {
                    List<string> victor = new List<string>();
                    victor.Add(name2);
                    return victor;
                }
                else
                    throw;
            }

            try
            {
                player2.Register(name2); //Assign player 2
            }
            catch (Exception e)
            {
                if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                {
                    List<string> victor = new List<string>();
                    victor.Add(name1);
                    return victor;
                }
                else
                    throw;
            }

            RefereeWrapper referee = new RefereeWrapper(player1, player2, size);
            List<string> victors = referee.RefereeGame(name1, name2);
            return victors;
        }

        public static List<PlayerRanking> AdministerTournament(string tournament_type, int _number_of_remote_players, 
            int port, string path, int board_size = 9)
        {
            //locals and initialization
            string goPlayer = File.ReadAllText(path);
            JObject playerJObject = JsonConvert.DeserializeObject<JObject>(goPlayer);
            int depth = playerJObject["depth"].ToObject<int>();
            string aiType = playerJObject["aiType"].ToObject<string>();

            List<PlayerStruct> players = new List<PlayerStruct>();
            for (int i = 0; i < _number_of_remote_players; i++)
            {
                try
                {
                    PlayerWrapper player = new PlayerWrapper(Socket, true);
                    Console.WriteLine("Successfully connected player" + i);

                    Console.Write("Trying to register player" + i + ": ");
                    string player_name = player.Register("default player" + i);
                    Console.WriteLine("Sucessfully registered player: " + player_name);

                    players.Add(new PlayerStruct(player, player_name));
                }
                catch (Exception e)
                {
                    if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                    {
                        Console.WriteLine("Unsuccessful registration of player " + i);
                        players[i].Disqualify();
                    }
                }
            }

            if (Math.Log(_number_of_remote_players, 2) != 0)
            {
                double additional_players = Math.Pow(2, Math.Ceiling(Math.Log(_number_of_remote_players, 2))) - _number_of_remote_players;

                int player_num = players.Count;
                for (int i = 0; i < additional_players; i++)
                {
                    players.Add(Admin.CreateDefaultPlayerStruct(aiType, depth, "default player " + player_num));
                    player_num++;
                }
            }
            else if (_number_of_remote_players == 1)
            {
                players.Add(Admin.CreateDefaultPlayerStruct(aiType, depth, "default player " + 1));
            }
            else if (_number_of_remote_players < 1)
            {
                players.Add(Admin.CreateDefaultPlayerStruct(aiType, depth, "default player " + 0));
                players.Add(Admin.CreateDefaultPlayerStruct(aiType, depth, "default player " + 1));
            }

            switch (tournament_type)
            {
                case "cup":
					Console.WriteLine("start cup game");
                    return AdministerSingleElimination(players, board_size, aiType, depth);
                case "league":
					Console.WriteLine("starting league game");
                    return AdministerRoundRobin(players, board_size, aiType, depth);
                default:
                    throw new AdminException("Invalid tournament type in Admin: " + tournament_type);
            }
        }

        /*
         * Administers a round robin tournament
         * Returns a sorted list of player rankings
         */
        private static List<PlayerRanking> AdministerRoundRobin(List<PlayerStruct> players, int board_size, string aiType, int depth)
        {
            List<string> cheaters = new List<string>();

            //Start by replacing players who failed to register/receive-stones with default-players
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].hasCheated)
                {
                    Console.WriteLine("Replacing " + players[i].name + " with replacement player" + players.Count);
                    cheaters.Add(players[i].name);
                    players[i] = (CreateDefaultPlayerStruct(aiType, depth, "replacement player " + players.Count));
                }
            }

            //Play tournament
            RefereeWrapper referee;
            List<string> victors;
            string victor;
            bool has_cheater;
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = i + 1; j < players.Count; j++)
                {
	                Console.WriteLine("Match between " + players[i].name + " and " + players[j].name + " is beginning");
					referee = new RefereeWrapper(players[i].playerObject , players[j].playerObject, board_size);
                    victors = referee.RefereeGame(players[i].name, players[j].name, out has_cheater);

                    //If draw, set victor to random player
                    if (victors.Count == 2)
                    {
                        Random rng = new Random();
                        victor = victors[rng.Next(0, 2)];
                        Console.WriteLine("There was as tie! " + victor + " was chosen as the winner");
                    }
                    else
                        victor = victors[0];

                    Console.WriteLine(victor + " is the winner of this match");

                    //Update matches
                    int winner = victor == players[i].name ? i : j;
                    int loser = winner == i ? j : i;
                    players[winner].IncrementScore();
                    players[winner].AddToWonAgainst(players[loser]);

                    if (has_cheater)
                    {
                        //Disqualify cheater
                        int cheater = victors[0] == players[i].name ? j : i;
                        players[cheater].Disqualify();
                        Console.WriteLine(players[cheater].name + " cheated");

                        //Add to cheaters list and replace
                        cheaters.Add(players[cheater].name);
                        players[cheater] = CreateDefaultPlayerStruct(aiType, depth, "replacement player " + players.Count);
                        Console.WriteLine("Adding new player: replacement player " + cheater.ToString());
                    }

                    //call end game for all players (that didn't cheat)
                    List<PlayerStruct> ls = new List<PlayerStruct>();
                    ls.Add(players[winner]);
                    if (!has_cheater)
                        ls.Add(players[loser]);
                    foreach (PlayerStruct player in ls)
                    {
                        try
                        {
                            Console.Write("Sending End Game to " + player.name + ": ");
                            string end = player.playerObject.EndGame();
                            if (end != "OK")
                                throw new WrapperException("invalid endgame message");
                            Console.WriteLine("Successfully ended game");
                        }
                        catch (Exception e)
                        {
                            if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                            {
                                Console.Write("FAILED, ");

                                player.Disqualify();
                                cheaters.Add(player.name);
                                if (players[winner].name == player.name)
                                    players[winner] = CreateDefaultPlayerStruct(aiType, depth, "replacement player " + players.Count);
                                else
                                    players[loser] = CreateDefaultPlayerStruct(aiType, depth, "replacement player " + players.Count);

                                Console.WriteLine("Replaced with replacement player" + players.Count);
                            }
                            else
                                throw;
                        }
                    }

                    Console.WriteLine("==========================================================");
                }
            }

            //Return rankings
            List<PlayerRanking> player_rankings = new List<PlayerRanking>();
            for(int i = 0; i < players.Count; i++)
            {
                player_rankings.Add(new PlayerRanking(players[i].name, players[i].score));
            }

            player_rankings.Sort(SortPlayerRankings);

            foreach(string cheater in cheaters)
                player_rankings.Add(new PlayerRanking(cheater, 0));

            return player_rankings;
        }

        /*
         * Administers a single elimination tournament
         * Returns a sorted list of player rankings
         */
	    private static List<PlayerRanking> AdministerSingleElimination(List<PlayerStruct> players, int board_size,
		    string aiType, int depth)
	    {
		    List<PlayerStruct> remainingPlayers = players;
		    List<PlayerRanking> rankings = new List<PlayerRanking>();
		    int score = 1;

		    //Create a list of players, play games between them, and remove eliminated players from list
		    string victor;
		    while (remainingPlayers.Count != 1)
		    {
			    List<PlayerStruct> winners = new List<PlayerStruct>();
				while (remainingPlayers.Count != 0)
			    {
				    //Update players
				    PlayerStruct player1 = remainingPlayers[0];
				    PlayerStruct player2 = remainingPlayers[1];
				    remainingPlayers.RemoveAt(0);
				    remainingPlayers.RemoveAt(0);

				    RefereeWrapper referee = new RefereeWrapper(player1.playerObject, player2.playerObject, board_size);
				    Console.WriteLine("Match between " + player1.name + " and " + player2.name + " is beginning");
				    List<string> victors = referee.RefereeGame(player1.name, player2.name, out bool has_cheater);

				    //If draw, set victor to random player
				    if (victors.Count == 2)
				    {
					    Random rng = new Random();
					    victor = victors[rng.Next(0, 2)];
					    Console.WriteLine("There was as tie! " + victor + " was chosen as the winner");
				    }
				    else
					    victor = victors[0];

				    Console.WriteLine(victor + " is the winner of this match");

				    //Update winner/loser
				    PlayerStruct winner = victor == player1.name ? player1 : player2;
				    PlayerStruct loser = victor == player1.name ? player2 : player1;

				    if (has_cheater)
				    {
					    //send cheater to bottom of ranking
					    Console.WriteLine(loser.name + " cheated");
					    rankings.Add(new PlayerRanking(loser.name, 0));
				    }
				    else
				    {
					    rankings.Add(new PlayerRanking(loser.name, score));
				    }


				    try
				    {
					    Console.Write("Sending End Game to " + winner.name + ": ");
					    string end = winner.playerObject.EndGame();
					    if (end != "OK")
						    throw new WrapperException("invalid endgame message");
					    Console.WriteLine("Successfully ended game");
					    Console.WriteLine("=============================================");
					}
				    catch (Exception e)
				    {
					    if (e is JsonSerializationException || e is ArgumentException || e is SocketException ||
					        e is WrapperException || e is JsonReaderException)
					    {
						    Console.Write("FAILED, ");

						    rankings.Add(new PlayerRanking(winner.name, 0));
						    remainingPlayers.Add(CreateDefaultPlayerStruct(aiType, depth, "replacement player " + rankings.Count));
						    Console.WriteLine("Replaced with replacement player" + rankings.Count);
					    }
					    else
						    throw;
				    }
					winners.Add(winner);
				}
			    remainingPlayers = winners;
			    score++;
			}
		    rankings.Add(new PlayerRanking(remainingPlayers[0].name, score));

		    rankings.Sort(SortPlayerRankings);
		    return rankings;
		}

	    private static PlayerStruct CreateDefaultPlayerStruct(string aiType, int depth, string name)
        {
            PlayerWrapper player = new PlayerWrapper(aiType, depth, true);
            string player_name = player.Register(name);
            Console.WriteLine("Sucessfully registered default player: " + player_name);

            return new PlayerStruct(player, player_name);
        }

        public struct PlayerStruct
        {
            public PlayerWrapper playerObject { get; private set; }
            public string name { get; private set; }
            public int score { get; private set; }
            public bool hasCheated { get; private set; }
            private List<PlayerStruct> wonAgainst;

            public PlayerStruct(PlayerWrapper p, string n)
            {
                playerObject = p;
                name = n;
                score = 0;
                hasCheated = false;
                wonAgainst = new List<PlayerStruct>();
            }

            public void IncrementScore()
            {
                if (!hasCheated)
                    score++;
            }

            //Reset score, set hasCheated, refund points
            public void Disqualify()
            {
                score = 0;
                hasCheated = true;
                foreach (PlayerStruct p in wonAgainst)
                {
                    p.IncrementScore();
                }

            }

            public void AddToWonAgainst(PlayerStruct p)
            {
                wonAgainst.Add(p);
            }
        }

        public struct PlayerRanking
        {
            public string name;
            public int score;

            public PlayerRanking(string n, int s)
            {
                name = n;
                score = s;
            }
        }

        private static int SortPlayerRankings(PlayerRanking player1, PlayerRanking player2)
        {
            return player2.score.CompareTo(player1.score);
        }
    }
}
