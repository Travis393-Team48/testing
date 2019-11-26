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

        public static List<PlayerRanking> AdministerTournament(string tournament_type, int _number_of_remote_players, int port, string path, int board_size = 9)
        {
            //locals and initialization
            string goPlayer = File.ReadAllText(path);
            JObject playerJObject = JsonConvert.DeserializeObject<JObject>(goPlayer);
            int depth = playerJObject["depth"].ToObject<int>();

            List<PlayerWrapper> players = new List<PlayerWrapper>();

            for (int i = 0; i < _number_of_remote_players; i++)
            {
                players.Add(new PlayerWrapper(Socket, true));
            }

            if (Math.Log(_number_of_remote_players, 2) != 0)
            {
                double additional_players = Math.Pow(2, Math.Ceiling(Math.Log(_number_of_remote_players, 2))) - _number_of_remote_players;

                for (int i = 0; i < additional_players; i++)
                {
                    players.Add(new PlayerWrapper("smart", depth, true));
                }
            }
            else if (_number_of_remote_players == 1)
            {
				players.Add(new PlayerWrapper("smart", depth, true));
            }

            //reset these lists
            List<string> player_names = new List<string>();
            List<bool> has_cheated = new List<bool>();

            //Register players and get their names
            int playerNumber = 0;
            foreach (PlayerWrapper player in players)
            {
                try
                {
                    player_names.Add(player.Register("player" + playerNumber));
					has_cheated.Add(false);
                    playerNumber++;
                }
                catch (Exception e)
                {
                    if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                    {
                        players[playerNumber] = new PlayerWrapper("smart", depth, true);                        
						has_cheated.Add(true);
                    }
                    else
                        throw;
                }
            }

            switch (tournament_type)
            {
                case "cup":
					Console.WriteLine("start cup game");
                    return AdministerSingleElimination(players, player_names, has_cheated, board_size);
                case "league":
					Console.WriteLine("starting league game");
                    return AdministerRoundRobin(players, player_names, has_cheated, board_size);
                default:
                    throw new AdminException("Invalid tournament type in Admin");
            }
        }

        /*
         * Administers a round robin tournament
         * Returns a sorted list of player rankings
         */
        private static List<PlayerRanking> AdministerRoundRobin(List<PlayerWrapper> players, List<string> player_names, List<bool> has_cheated, int board_size)
        {
            List<string> cheaters = new List<string>();

            for (int i = 0; i < players.Count; i++)
            {
                if (has_cheated[i])
                {
                    cheaters.Add(player_names[i]);
                    //technically incorrect default-player
                    players[i] = new PlayerWrapper("smart", 1, true);
                    player_names[i] = players[i].Register("replacement player" + i.ToString());
                }
            }

            //Play tournament
            int[][] matches = new int[players.Count][];
            for (int i = 0; i < players.Count; i++)
            {
                matches[i] = new int[players.Count];
                for (int j = 0; j < players.Count; j++)
                {
                    matches[i][j] = -1;
                }
            }

            RefereeWrapper referee;
            List<string> victors;
            string victor;
            bool has_cheater;
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (matches[i][j] != -1)
                        continue;

                    referee = new RefereeWrapper(players[i], players[j], board_size);
                    victors = referee.RefereeGame(player_names[i], player_names[j], out has_cheater);

                    if (has_cheater)
                    {
                        int cheater = victors[0] == player_names[i] ? j : i;
                        has_cheated[cheater] = true;

                        //replace with default player
                        cheaters.Add(player_names[cheater]);
                        //technically incorrect default-player
                        players[cheater] = new PlayerWrapper("smart", 1, true);
                        player_names[cheater] = players[cheater].Register("replacement player" + cheater.ToString());
                        
                        //modify scores
                        for (int c = 0; c < players.Count; c++)
                        {
                            if (matches[cheater][c] == cheater)
                            {
                                matches[cheater][c] = c;
                                matches[c][cheater] = c;
                            }
                        }
                    }
                    else
                    {
                        //If draw, return random player
                        if (victors.Count == 2)
                        {
                            Random rng = new Random();
                            victor = victors[rng.Next(0, 2)];
                        }
                        else if (victors.Count == 1)
                            victor = victors[0];
                        else
                            throw new AdminException(victors.Count.ToString() + " victors returned in Admin");

                        //Update matches
                        matches[i][j] = matches[j][i] = victor == player_names[i] ? i : j;
                    }

                    #region End Game
                    try
                    {
                        //assuming default players won't cheat
                        if (!has_cheated[i])
                        {
                            string end = players[i].EndGame();
                            if (end != "OK")
                                throw new WrapperException("invalid endgame message");
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                        {
                            has_cheated[i] = true;

                            cheaters.Add(player_names[i]);
                            //technically incorrect default-player
                            players[i] = new PlayerWrapper("smart", 1, true);
                            player_names[i] = players[i].Register("replacement player" + i.ToString());

                            //modify scores
                            for (int c = 0; c < players.Count; c++)
                            {
                                if (matches[i][c] == i)
                                {
                                    matches[i][c] = c;
                                    matches[c][i] = c;
                                }
                            }
                        }
                        else
                            throw;
                    }

                    try
                    {
                        //assuming default players won't cheat
                        if (!has_cheated[i])
                        {
                            string end = players[j].EndGame();
                            if (end != "OK")
                                throw new WrapperException("invalid endgame message");
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                        {
                            has_cheated[j] = true;

                            cheaters.Add(player_names[j]);
                            //technically incorrect default-player
                            players[j] = new PlayerWrapper("smart", 1, true);
                            player_names[j] = players[j].Register("replacement player" + j.ToString());

                            //modify scores
                            for (int c = 0; c < players.Count; c++)
                            {
                                if (matches[j][c] == j)
                                {
                                    matches[j][c] = c;
                                    matches[c][j] = c;
                                }
                            }
                        }
                        else
                            throw;
                    }
                    #endregion
                }
            }

            //Return rankings
            List<PlayerRanking> player_rankings = new List<PlayerRanking>();
            for(int i = 0; i < players.Count; i++)
            {
                int score = 0;
                for (int j = 0; j < players.Count; j++)
                {
                    if (matches[i][j] == i)
                        score++;
                }
                player_rankings.Add(new PlayerRanking(player_names[i], score));
            }

            player_rankings.Sort(SortPlayerRankings);

            foreach(string cheater in cheaters)
                player_rankings.Add(new PlayerRanking(cheater, 0));

            return player_rankings;
        }

        private static List<PlayerRanking> AdministerSingleElimination(List<PlayerWrapper> players, List<string> player_names, List<bool> has_cheated, int board_size)
        {
            List<PlayerWrapper> remainingPlayers = players;
            List<string> remainingPlayersNames = player_names;
            List<PlayerRanking> rankings = new List<PlayerRanking>();
            int score = 1;

            string victor;
            int replacementCount = 0;
            while (remainingPlayers.Count != 1)
            {
                List<PlayerWrapper> winners = new List<PlayerWrapper>();
                List<string> winnersNames = new List<string>();
                
                while(remainingPlayers.Count != 0)
                {
                    //Update players/player names
                    PlayerWrapper player1 = remainingPlayers[0];
                    PlayerWrapper player2 = remainingPlayers[1];
                    remainingPlayers.RemoveAt(0);
                    remainingPlayers.RemoveAt(0);
                    string player1Name = remainingPlayersNames[0];
                    string player2Name = remainingPlayersNames[1];
                    remainingPlayersNames.RemoveAt(0);
                    remainingPlayersNames.RemoveAt(0);

                    RefereeWrapper referee = new RefereeWrapper(player1, player2, board_size);
                    List<string> victors = referee.RefereeGame(player1Name, player2Name, out bool has_cheater);

                    if (victors.Count == 2)
                    {
                        Random rng = new Random();
                        victor = victors[rng.Next(0, 2)];
                    }
                    else if (victors.Count == 1)
                        victor = victors[0];
                    else
                        throw new AdminException(victors.Count.ToString() + " victors returned in Admin");

                    if (victor == player1Name)
                    {
                        try
                        {
                            string end = player1.EndGame();
                            if (end != "OK")
                                throw new WrapperException("invalid endgame message");
                            winners.Add(player1);
                            winnersNames.Add(player1Name);
                        }
                        catch (Exception e)
                        {
                            if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                            {
                                rankings.Add(new PlayerRanking(player1Name, 0));
                                //technically incorrect default-player
                                PlayerWrapper replacement = new PlayerWrapper("smart", 1, true);
                                winners.Add(replacement);
                                winnersNames.Add(replacement.Register("replacement player" + replacementCount.ToString()));
                                replacementCount++;
                            }
                            else
                                throw;
                        }

                        if (has_cheater)
                            rankings.Add(new PlayerRanking(player2Name, 0));
                        else
                        {
                            try
                            {
                                string end = player2.EndGame();
                                if (end != "OK")
                                    throw new WrapperException("invalid endgame message");
                                rankings.Add(new PlayerRanking(player2Name, score));
                            }
                            catch (Exception e)
                            {
                                if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                                    rankings.Add(new PlayerRanking(player2Name, 0));
                                else
                                    throw;
                            }
                        }
                    }
                    else if (victor == player2Name)
                    {
                        try
                        {
                            string end = player2.EndGame();
                            if (end != "OK")
                                throw new WrapperException("invalid endgame message");
                            winners.Add(player2);
                            winnersNames.Add(player2Name);
                        }
                        catch (Exception e)
                        {
                            if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                            {
                                rankings.Add(new PlayerRanking(player2Name, 0));
                                //technically incorrect default-player
                                PlayerWrapper replacement = new PlayerWrapper("smart", 1, true);
                                winners.Add(replacement);
                                winnersNames.Add(replacement.Register("replacement player" + replacementCount.ToString()));
                                replacementCount++;
                            }
                            else
                                throw;
                        }

                        if (has_cheater)
                            rankings.Add(new PlayerRanking(player1Name, 0));
                        else
                        {
                            try
                            {
                                string end = player1.EndGame();
                                if (end != "OK")
                                    throw new WrapperException("invalid endgame message");
                                rankings.Add(new PlayerRanking(player1Name, score));
                            }
                            catch (Exception e)
                            {
                                if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                                    rankings.Add(new PlayerRanking(player1Name, 0));
                                else
                                    throw;
                            }
                        }
                    }
                    else
                        throw new AdminException("Non-existant victor returned in Admin: " + victor);
                }

                remainingPlayers = winners;
	            remainingPlayersNames = winnersNames;
                score++;
            }

            rankings.Add(new PlayerRanking(remainingPlayersNames[0], score));

            rankings.Sort(SortPlayerRankings);
            return rankings;
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
            return player1.score.CompareTo(player2.score);
        }
    }
}
