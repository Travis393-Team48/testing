﻿using System;
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

            RefereeWrapper referee = new RefereeWrapper(3);

            //Create local player
            Process.Start(Path.Combine(Environment.CurrentDirectory, path));
            PlayerWrapper player1 = new PlayerWrapper(true, port);

            PlayerWrapper player2 = new PlayerWrapper(false);


            string player1_name = player1.Register("player1", "illegal");
            string player2_name = player2.Register("player2", "dumb");
            player1.ReceiveStones("B");
            player2.ReceiveStones("W");
            referee.Register(player1_name);
            referee.Register(player2_name);

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
                catch (Exception e)
                {
                    //Used during debugging
                    Console.WriteLine(e);

                    Console.WriteLine(referee.GetVictors());
                    break;
                }
            }

            Console.ReadLine();
        }
    }
}
