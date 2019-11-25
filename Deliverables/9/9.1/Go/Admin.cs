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

        public static List<string> AdministerTournament(string tournament_type, int _number_of_remote_players, int port, string path, int board_size = 9)
        {
            //locals and initialization
            string goPlayer = File.ReadAllText(path);
            JObject playerJObject = JsonConvert.DeserializeObject<JObject>(goPlayer);
            int depth = playerJObject["depth"].ToObject<int>();

            List<PlayerWrapper> players = new List<PlayerWrapper>();

            for (int i = 0; i < _number_of_remote_players; i++)
            {
                players.Add(new PlayerWrapper(port, true));
                port++;
            }

            if (Math.Log(_number_of_remote_players, 2) != 0)
            {
                double additional_players = Math.Pow(2, Math.Ceiling(Math.Log(_number_of_remote_players, 2))) - _number_of_remote_players;

                for (int i = 0; i < additional_players; i++)
                {
                    players.Add(new PlayerWrapper("less dumb", depth, true));
                }
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
                    playerNumber++;
                }
                catch (Exception e)
                {
                    if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                    {
                        players[playerNumber] = new PlayerWrapper("less dumb", depth, true);
                        has_cheated[playerNumber] = true;
                    }
                    else
                        throw;
                }
            }

            switch (tournament_type)
            {
                case "cup":
                    return AdministerSingleElimination(players, player_names, has_cheated, board_size);
                case "league":
                    return AdministerRoundRobin(players, player_names, has_cheated, board_size);
                default:
                    throw new AdminException("Invalid tournament type in Admin");
            }
        }

        private static List<string> AdministerRoundRobin(List<PlayerWrapper> players, List<string> player_names, List<bool> has_cheated, int board_size)
        {
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
                        int cheater = victors[0] == player_names[i] ? i : j;
                        has_cheated[cheater] = true;

                        throw new NotImplementedException();
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
                }
            }

            //Return rankings
            throw new NotImplementedException();
        }

        private static List<string> AdministerSingleElimination(List<PlayerWrapper> players, List<string> player_names, List<bool> has_cheated, int board_size)
        {

            throw new NotImplementedException();
        }
    }
}
