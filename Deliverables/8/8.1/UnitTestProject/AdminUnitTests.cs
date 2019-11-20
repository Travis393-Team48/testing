﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                "disconnect on connect A",
                "disconnect on connect B",
                "disconnect on register",
                "disconnect on receive stones",
                "disconnect on make a move"
            };

            foreach (string config in configs)
            {
                PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                PlayerWrapper player1 = new PlayerWrapper(port);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                Assert.AreEqual(victors[1], "local player");

                port++;
            }
        }

        [TestMethod]
        public void ClientInvalidJsonTests()
        {
            int port = 8280;
            List<string> configs = new List<string>()
            {
                "send json int on register",
                "send json array on register",
                "send json int on make a move",
                "send json array on make a move",
                "send json object on make a move",
            };

            foreach (string config in configs)
            {
                PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                PlayerWrapper player1 = new PlayerWrapper(port);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                Assert.AreEqual(victors[1], "local player");

                port++;
            }
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
                "return characters",
                "return numbers",
                "return array"
            };

            foreach (string config in configs)
            {
                PlayerClientIllegal client = new PlayerClientIllegal("localhost", port, "illegal", config);
                PlayerWrapper player1 = new PlayerWrapper(port);
                PlayerWrapper player2 = new PlayerWrapper("less dumb");
                List<string> victors = Admin.AdministerGame(player1, "remote player", player2, "local player", 9);

                Assert.AreEqual(victors.Count, 1);
                Assert.AreEqual(victors[1], "local player");

                port++;
            }
        }
    }
}
