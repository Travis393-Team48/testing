using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerSpace;
using CustomExceptions;

namespace PlayerSpace
{
    static class PlayerProxyTestDriver
    {
        static void Main(string[] args)
        {
            string console = "";
            string input;

	        List<JToken> finalList = new List<JToken>();

	        string go_player = File.ReadAllText("go-player.config");
	        string go = File.ReadAllText("go.config");

	        JObject ipPort = JsonConvert.DeserializeObject<JObject>(go);
	        JObject depth = JsonConvert.DeserializeObject<JObject>(go_player);

	        PlayerAdapter aiPlayer = new PlayerAdapter(true, ipPort["port"].ToObject<int>());

	        PlayerClient client = new PlayerClient(ipPort["IP"].ToObject<string>(), ipPort["port"].ToObject<int>());

	        //Console.ReadLine();

	        JToken toAdd;

			//Read from console
			while ((input = Console.ReadLine()) != null)
	        {
		        console += input;
		        try
		        {
			        List<JToken> curr = ParsingHelper.ParseJson(console);
			        toAdd = aiPlayer.JsonCommand(curr[0], "no name", "less dumb", depth["depth"].ToObject<int>());
			        if (toAdd.Type != JTokenType.Null)
				        finalList.Add(toAdd);
			        console = "";
		        }
		        catch (InvalidJsonInputException)
		        {
			        finalList.Add("GO has gone crazy!");
			        break;
		        }
		        catch (Newtonsoft.Json.JsonReaderException)
		        {
					Console.WriteLine("blah");
		        }
			}
                

            //Parse console input
            //List<JToken> jTokenList = ParsingHelper.ParseJson(console);

            
            //foreach (JToken jtoken in jTokenList)
            //{
            //    try
            //    {
            //        toAdd = aiPlayer.JsonCommand(jtoken, "no name", "less dumb", depth["depth"].ToObject<int>());
            //        if (toAdd.Type != JTokenType.Null)
            //            finalList.Add(toAdd);
            //    }
            //    catch (InvalidJsonInputException)
            //    {
            //        finalList.Add("GO has gone crazy!");
            //        break;
            //    }
            //}

            Console.WriteLine(JsonConvert.SerializeObject(finalList));

            Console.ReadLine();
        }
    }
}
