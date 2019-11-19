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
    class Admin
    {
        static void Main(string[] args)
        {
            string go = File.ReadAllText("go.config");
            JObject config = JsonConvert.DeserializeObject<JObject>(go);

            string ip = config["IP"].ToObject<string>();
            int port = config["port"].ToObject<int>();
            string path = config["default-player"].ToObject<string>();

            //Create remote player (will need explicitly create a remote player to connect to this)
            PlayerWrapper player1 = new PlayerWrapper(port);

            //Create local player
            //Process.Start(Path.Combine(Environment.CurrentDirectory, path));
            PlayerWrapper player2 = new PlayerWrapper("dumb");

            RefereeWrapper referee = new RefereeWrapper(player1, player2, 3);

            referee.Register(); //Register player 1
            referee.Register("hello player"); //Register player 2

            PlayerWrapper current_player = player1;
            string[][][] board_history;
            string next_move;
            while (true)
            {
                try
                {
                    board_history = referee.GetBoardHistory();
                    next_move = current_player.MakeAMove(board_history);
                    //Used during debugging
                    //if (next_move == "This history makes no sense!")
                    //    Console.WriteLine(JsonConvert.SerializeObject(board_history));
                    if (next_move == "pass")
                        referee.Pass();
                    else
                        referee.Play(next_move);
                    current_player = current_player == player1 ? player2 : player1;
                }
                catch (SocketException e)
                {
                    JArray array;
                    if (current_player == player1)
                    {
                        array = new JArray { player2.GetName() };
                    }
                    else
                    {
                        array = new JArray { player1.GetName() };
                    }
                    //Console.WriteLine(JsonConvert.SerializeObject(array));
	                Console.WriteLine("socket exception " + e);
                    break;
                }
                catch (WrapperException e)
                {
                    JArray array;
                    if (current_player == player1)
                    {
                        array = new JArray { player2.GetName() };
                    }
                    else
                    {
                        array = new JArray { player1.GetName() };
                    }
                    //Console.WriteLine(JsonConvert.SerializeObject(array));
	                Console.WriteLine("wrapper exception " + e);
                    break;
                }
                catch (RefereeException e)
                {
                    List<PlayerWrapper> victors = referee.GetVictors();
                    List<string> names = new List<string>();
                    foreach (PlayerWrapper victor in victors)
                        names.Add(victor.GetName());

                    //Console.WriteLine(JsonConvert.SerializeObject(names.ToArray()));
	                Console.WriteLine("referee exception " + e);
                    break;
                }

            }

            Console.ReadLine();
        }
    }
}
