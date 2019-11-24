using System;
using System.Collections.Generic;
using RefereeSpace;
using CustomExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

class RefereeTestDriver
{
    /* TestDriver for assignment 6
     * Simulates a game using the referee and keeps track of referee's outputs
     * Once referee terminates due to the game ending, an illegal move, or no more inputs
     *  writes all outputs of referee to console
     */
    public static void Main(string[] args)
    {
        string console = "";
        string input;
        bool _player1_set = false;
        bool _player2_set = false;

        //Read from console
        while ((input = Console.ReadLine()) != null)
            console += input;

        //Parse console input
        List<JToken> jTokenList = ParsingHelper.ParseJson(console);

        List<JToken> finalList = new List<JToken>();

        PlayerWrapper player1 = new PlayerWrapper("human");
        PlayerWrapper player2 = new PlayerWrapper("human");

        RefereeAdapter referee = new RefereeAdapter(player1, player2);
        foreach (JToken jtoken in jTokenList)
        {
            try
            {
                if (!_player1_set)
                {
                    finalList.Add(JToken.Parse(JsonConvert.SerializeObject(
                        player1.Register(jTokenList[0].ToObject<string>()))));
                    _player1_set = true;
                }
                else if (!_player2_set)
                {
                    finalList.Add(JToken.Parse(JsonConvert.SerializeObject(
                        player2.Register(jTokenList[1].ToObject<string>()))));
                    _player2_set = true;
                }
                else
                {
                    try
                    {
                        referee.JsonCommand(jtoken, finalList);
                    }
                    catch (RefereeException)
                    {
                        break;
                    }
                }
            }
            catch
            {
                break;
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(finalList));

        Console.ReadLine();
    }
}
