using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network;
using Network.Enums;
using Network.Packets;

namespace PlayerSpace
{
    class PlayerProxyTestDriver
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
	        string IP;
	        int port;

	        cfJson = configJson("../go.config");
	        IP = cfJson["IP"];
	        port = cfJson["port"];


            //Create a new server container.
            ServerConnectionContainer serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(port, false);
=======
            PlayerProxy a = new PlayerProxy(8080);
            PlayerClient b = new PlayerClient("localhost", 8080);
>>>>>>> 0e484b7e3dedc373d8cc63227879408877d4544e

            Console.ReadLine();

            //a.ReceiveStones("B");

            Console.ReadLine();

            Console.WriteLine(a.GetStone());

            Console.ReadLine();
        }

	    private string configJson(string file)
	    {
		    string json;
		    using (StreamReader sr = new StreamReader(file))
		    {
			    json = sr.ReadToEnd();
		    }

		    return json;
	    }
    }
}
