using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeSpace;
using CustomExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class RefereeTestDriver
{
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

        RefereeAdapter referee = new RefereeAdapter();
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
