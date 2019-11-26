using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using FluentAssertions.Json;
using RefereeSpace;
using CustomExceptions;
using PlayerSpace;
using Go;

namespace UnitTests
{
    [TestClass]
    public class AdminUnitTests
    {
        [TestMethod]
        public void ClientDisconnectTests()
        {
            int port = 8180;
            List<string> configs = new List<string>()
            {
                "disconnect on connect B",
                "disconnect on connect A",
                "disconnect on register",
                "disconnect on receive stones",
                "disconnect on make a move"
            };

            //Network setup
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            Admin.Socket = socket;

            foreach (string config in configs)
            {
                try
                {
                    PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                }
                catch (PlayerClientException) { };
                PlayerWrapper player1 = new PlayerWrapper(socket);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerSingleGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                if (victors.Count == 1)
                    Assert.AreEqual(victors[0], "local player");

                port++;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        [TestMethod]
        public void ClientInvalidJsonTests()
        {
            int port = 8280;
            List<string> configs = new List<string>()
            {
                "send json object on make a move",
                "send json array on register",
                "send json array on make a move",
            };

            //Network setup
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            Admin.Socket = socket;

            foreach (string config in configs)
            {
                PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                PlayerWrapper player1 = new PlayerWrapper(socket);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerSingleGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                if (victors.Count == 1)
                    Assert.AreEqual(victors[0], "local player");

                port++;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        [TestMethod]
        public void PlayerIllegalMovesTests()
        {
            int port = 8380;
            List<string> configs = new List<string>()
            {
                "always return 1-1",
                "return 19-19 once",
                "return 99-99 once",
                "return characters once",
                "return numbers once",
                "return array once",
				"pass forever", //not illegal, but will let other player win
				"return empty",
            };

            //Network setup
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            Admin.Socket = socket;

            foreach (string config in configs)
            {
                PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                PlayerWrapper player1 = new PlayerWrapper(socket);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerSingleGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                if (victors.Count == 1)
                    Assert.AreEqual(victors[0], "local player");

                port++;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        [TestMethod]
        public void RankingSimulator()
        {
            List<Admin.PlayerRanking> finalRankings = new List<Admin.PlayerRanking>();
            finalRankings.Add(new Admin.PlayerRanking("name", 20));
            finalRankings.Add(new Admin.PlayerRanking("name", 9));
            finalRankings.Add(new Admin.PlayerRanking("name", 9));
            finalRankings.Add(new Admin.PlayerRanking("name", 8));
            finalRankings.Add(new Admin.PlayerRanking("name", 8));
            finalRankings.Add(new Admin.PlayerRanking("name", 0));
            finalRankings.Add(new Admin.PlayerRanking("name", 0));
            finalRankings.Add(new Admin.PlayerRanking("name", 0));
            finalRankings.Add(new Admin.PlayerRanking("name", 0));
            finalRankings.Add(new Admin.PlayerRanking("name", 0));
            Rank(finalRankings);
        }

        private List<string> Rank(List<Admin.PlayerRanking> finalRankings)
        {
            int currRank = 0;
            List<string> printRankings = new List<string>();
            for (int i = 0; i < finalRankings.Count; i++)
            {
                if (i != 0 && finalRankings[i].score == finalRankings[i - 1].score)
                {
                    printRankings[printRankings.Count - 1] = printRankings[printRankings.Count - 1] + ", " + finalRankings[i].name;
                }
                else
                {
                    printRankings.Add((i + 1).ToString() + ": " + finalRankings[i].name);
                    currRank = i + 1;
                }
            }

            return printRankings;
        }
    }
}
