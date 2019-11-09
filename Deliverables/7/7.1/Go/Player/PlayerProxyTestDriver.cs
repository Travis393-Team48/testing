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
            PlayerProxy a = new PlayerProxy(8080);
            PlayerClient b = new PlayerClient("localhost", 8080);

            Console.ReadLine();

            //a.ReceiveStones("B");

            Console.ReadLine();

            Console.WriteLine(a.GetStone());

            Console.ReadLine();
        }


    }
}
