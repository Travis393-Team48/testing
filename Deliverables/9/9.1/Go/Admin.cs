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

            List<PlayerData> players = new List<PlayerData>();
            for (int i = 0; i < _number_of_remote_players; i++)
            {
                try
                {
                    PlayerWrapper player = new PlayerWrapper(Socket, true);
                    Console.WriteLine("Successfully connected player" + i);

                    Console.Write("Trying to register player" + i + ": ");
                    string player_name = player.Register("default player" + i);
                    Console.WriteLine("Sucessfully registered player: " + player_name);

                    players.Add(new PlayerData(player, player_name));
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
        private static List<PlayerRanking> AdministerRoundRobin(List<PlayerData> players, int board_size, string aiType, int depth)
        {
            List<string> cheaters = new List<string>();

            //Start by replacing players who failed to register/receive-stones with default-players
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].HasCheated)
                {
                    Console.WriteLine("Replacing " + players[i].Name + " with replacement player" + players.Count);
                    cheaters.Add(players[i].Name);
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
	                Console.WriteLine("Match between " + players[i].Name + " and " + players[j].Name + " is beginning");
					referee = new RefereeWrapper(players[i].PlayerObject , players[j].PlayerObject, board_size);
                    victors = referee.RefereeGame(players[i].Name, players[j].Name, out has_cheater);

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
                    int winner = victor == players[i].Name ? i : j;
                    int loser = winner == i ? j : i;
                    players[winner].IncrementScore();
                    players[winner].AddToWonAgainst(players[loser]);

                    if (has_cheater)
                    {
                        //Disqualify cheater
                        int cheater = victors[0] == players[i].Name ? j : i;
                        players[cheater].Disqualify();
                        Console.WriteLine(players[cheater].Name + " cheated");

                        //Add to cheaters list and replace
                        cheaters.Add(players[cheater].Name);
                        players[cheater] = CreateDefaultPlayerStruct(aiType, depth, "replacement player " + players.Count);
                        Console.WriteLine("Adding new player: replacement player " + players.Count);
                    }

                    //call end game for all players (that didn't cheat)
                    List<PlayerData> ls = new List<PlayerData>();
                    ls.Add(players[winner]);
                    if (!has_cheater)
                        ls.Add(players[loser]);
                    foreach (PlayerData player in ls)
                    {
                        try
                        {
                            Console.Write("Sending End Game to " + player.Name + ": ");
                            string end = player.PlayerObject.EndGame();
                            Console.WriteLine("Successfully ended game");
                        }
                        catch (Exception e)
                        {
                            if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                            {
                                Console.Write("FAILED, ");

                                player.Disqualify();
                                cheaters.Add(player.Name);
                                if (player.Name == players[winner].Name)
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
                player_rankings.Add(new PlayerRanking(players[i].Name, players[i].Score));
            }

            player_rankings.Sort(SortPlayerRankings);

            foreach(string cheater in cheaters)
                player_rankings.Add(new PlayerRanking(cheater, -1));

            return player_rankings;
        }

        /*
         * Administers a single elimination tournament
         * Returns a sorted list of player rankings
         */
	    private static List<PlayerRanking> AdministerSingleElimination(List<PlayerData> players, int board_size,
		    string aiType, int depth)
	    {
		    List<PlayerData> remainingPlayers = players;
		    List<PlayerRanking> rankings = new List<PlayerRanking>();
		    int score = 1;

		    //Create a list of players, play games between them, and remove eliminated players from list
		    string victor;
		    while (remainingPlayers.Count != 1)
		    {
			    List<PlayerData> winners = new List<PlayerData>();
				while (remainingPlayers.Count != 0)
			    {
				    //Update players
				    PlayerData player1 = remainingPlayers[0];
				    PlayerData player2 = remainingPlayers[1];
				    remainingPlayers.RemoveAt(0);
				    remainingPlayers.RemoveAt(0);

                    //Check if either player cheated in the previous round
                    PlayerData cheater = player1.HasCheated ? player1 : player2.HasCheated ? player2 : null;
                    PlayerData not_cheater = cheater == player1 ? player2 : cheater == player2 ? player1 : null;
                    if (cheater != null)
                    {
                        rankings.Add(new PlayerRanking(cheater.Name, -1));
                        winners.Add(not_cheater);
                        Console.WriteLine(cheater.Name + " loses due to cheating in previous round");
                        Console.WriteLine(not_cheater.Name + " is the winner of this match");
                        Console.WriteLine("===========================================================");
                        continue;
                    }

                    //Play game
                    RefereeWrapper referee = new RefereeWrapper(player1.PlayerObject, player2.PlayerObject, board_size);
				    Console.WriteLine("Match between " + player1.Name + " and " + player2.Name + " is beginning");
				    List<string> victors = referee.RefereeGame(player1.Name, player2.Name, out bool has_cheater);

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
				    PlayerData winner = victor == player1.Name ? player1 : player2;
				    PlayerData loser = victor == player1.Name ? player2 : player1;

                    if (has_cheater)
                    {
                        //send cheater to bottom of ranking
                        Console.WriteLine(loser.Name + " cheated");
                        rankings.Add(new PlayerRanking(loser.Name, -1));
                    }
                    else
                        rankings.Add(new PlayerRanking(loser.Name, score));

				    try
				    {
					    Console.Write("Sending End Game to " + winner.Name + ": ");
					    string end = winner.PlayerObject.EndGame();
					    Console.WriteLine("Successfully ended game");
					}
				    catch (Exception e)
				    {
					    if (e is JsonSerializationException || e is ArgumentException || e is SocketException ||
					        e is WrapperException || e is JsonReaderException)
					    {
						    Console.WriteLine("FAILED, " + winner.Name + " disqualified");
                            winner.Disqualify();
					    }
					    else
						    throw;
				    }

                    Console.WriteLine("===========================================================");
                    winners.Add(winner);
				}
			    remainingPlayers = winners;
			    score++;
			}

            //end of matches, returh player rankings
		    rankings.Add(new PlayerRanking(remainingPlayers[0].Name, score));
		    rankings.Sort(SortPlayerRankings);
		    return rankings;
		}

	    private static PlayerData CreateDefaultPlayerStruct(string aiType, int depth, string name)
        {
            PlayerWrapper player = new PlayerWrapper(aiType, depth, true);
            string player_name = player.Register(name);
            Console.WriteLine("Sucessfully registered default player: " + player_name);

            return new PlayerData(player, player_name);
        }

        public class PlayerData
        {
            public PlayerWrapper PlayerObject { get; private set; }
            public string Name { get; private set; }
            public int Score { get; private set; }
            public bool HasCheated { get; private set; }
            private List<PlayerData> wonAgainst;

            public PlayerData(PlayerWrapper p, string n)
            {
                PlayerObject = p;
                Name = n;
                Score = 0;
                HasCheated = false;
                wonAgainst = new List<PlayerData>();
            }

            public void IncrementScore()
            {
                if (!HasCheated)
                    Score++;
            }

            //Reset score, set hasCheated, refund points
            public void Disqualify()
            {
                Score = -1;
                HasCheated = true;
                foreach (PlayerData p in wonAgainst)
                {
                    p.IncrementScore();
                }

            }

            public void AddToWonAgainst(PlayerData p)
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
