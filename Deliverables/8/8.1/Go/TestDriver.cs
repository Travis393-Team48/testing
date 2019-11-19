using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;

static class TestDriver
{
    static void Main(string[] args)
    {
        string console = "";
        string input;

        //Read from console
        while ((input = Console.ReadLine()) != null)
            console += input;

        //Parse console input
        List<JToken> jTokenList = ParsingHelper.ParseJson(console);

        List<JToken> finalList = new List<JToken>();

        PlayerAdapter aiPlayer = new PlayerAdapter("dumb");
        JToken toAdd;
        foreach (JToken jtoken in jTokenList)
        {
            toAdd = aiPlayer.JsonCommand(jtoken);
            if (toAdd.Type != JTokenType.Null)
                finalList.Add(toAdd);
        }

        Console.WriteLine(JsonConvert.SerializeObject(finalList));

        Console.ReadLine();
    }
}
