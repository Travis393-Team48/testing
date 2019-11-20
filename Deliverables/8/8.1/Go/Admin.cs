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
    /* Given two players and the size of the game board, administers a game between the two players using referee
     * Returns an sorted list of strings containing the victor(s)
     * if a player is a remote player, the name given in function for that player doesn't matter
     */
    public static class Admin
    {
        public static List<string> AdministerGame(PlayerWrapper player1, string name1, PlayerWrapper player2, string name2, int size)
        {
            RefereeWrapper referee = new RefereeWrapper(player1, player2, size);
            try
            {
                referee.Register(name1); //Register player 1
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
                referee.Register(name2); //Register player 2
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

            List<string> victors = referee.RefereeGame();
            return victors;
        }
    }
}
