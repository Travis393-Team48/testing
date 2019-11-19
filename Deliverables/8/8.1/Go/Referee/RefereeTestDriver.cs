using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                referee.JsonCommand(jtoken, ref finalList);
            }
            catch (RefereeException)
            {
                break;
            }
        }

        Console.WriteLine(JsonConvert.SerializeObject(finalList));

        Console.ReadLine();
    }
}
