using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
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
            //Create a new server container.
            ServerConnectionContainer serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(8080, false);

            //3. Set a delegate which will be called if we receive a connection
            serverConnectionContainer.ConnectionEstablished += ConnectionEstablished;

            //Start listening on port 8080
            serverConnectionContainer.StartTCPListener();

            PlayerClient p = new PlayerClient();

            Console.ReadLine();
        }

        //Delegate which will be called once the server has established a connection
        private static void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            PlayerRequestPacket packet = new PlayerRequestPacket(JToken.Parse(JsonConvert.SerializeObject("string")));
            connection.Send(packet);
        }


    }
}
