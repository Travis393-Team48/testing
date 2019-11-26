using System;
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
	public class TournamentUnitTests
	{
		[TestMethod]
		public void LeagueCorrectOutput()
		{
			int port = 8100;
			List<Admin.PlayerRanking> ranking = Admin.AdministerTournament("league", 1, port, "go-player.config", 9);

			Assert.AreEqual(ranking.Count, 2);
			if (ranking.Count == 2)
				Assert.AreEqual(ranking[0].score, 1);

			port++;	
		}

		[TestMethod]
		public void CupCorrectOutput()
		{
			int port = 8200;
			List<Admin.PlayerRanking> ranking = Admin.AdministerTournament("cup", 1, port, "go-player.config", 9);

			Assert.AreEqual(ranking.Count, 2);
			if (ranking.Count == 2)
				Assert.AreEqual(ranking[0].score, 1);

			port++;
		}

		//public void EndGameMessage()
		//{

		//}

		//public void DisconnectInLeague()
		//{

		//}

		//public void IllegalMoveInLeague()
		//{

		//}

		//public void DisconnectInCup()
		//{

		//}

		//public void IllegalMoveInCup()
		//{

		//}
	}
}
