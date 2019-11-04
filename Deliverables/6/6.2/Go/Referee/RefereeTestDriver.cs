using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeSpace;
using CustomExceptions;
using Newtonsoft.Json.Linq;

class RefereeTestDriver
{
    public static void Main(string[] args)
    {
        string input;
        string console = "";
        List<JToken> jTokenList;
        RefereeAdapter referee = new RefereeAdapter();

        //Read from console
        while ((input = Console.ReadLine()) != null)
        {
            console += input;
            jTokenList = ParsingHelper.ParseJson(console);
            if (jTokenList.Count != 0)
            {
                while (jTokenList.Count != 0)
                {
                    try
                    {
                        referee.JsonCommand(jTokenList[0]);
                    }
                    catch (RefereeException)
                    {
                        break;
                    }
                    jTokenList.RemoveAt(0);
                }
                console = "";
            }
        }

        Console.ReadLine();
    }
}
