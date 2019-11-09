using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using System.Net;
using System.Net.Sockets;
using Network;
using Network.Enums;
using Network.Packets;
using Network.Interfaces;

namespace PlayerSpace
{
    public class PlayerClient
    {
        ClientConnectionContainer clientConnectionContainer;

        public PlayerClient()
        {
	        string IP;
	        int port;
	        cfJson = configJson("../go.config");
	        IP = cfJson["IP"];
	        port = cfJson["port"];

            clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(IP, port);
            clientConnectionContainer.ConnectionEstablished += ConnectionEstablished;

            Console.WriteLine("hello world");
        }

        private void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            clientConnectionContainer.TcpConnection.RegisterPacketHandler<PlayerRequestPacket>(PlayerRequestReceived, this);
        }

        private void TcpConnection_ConnectionClosed(CloseReason arg1, Connection arg2)
        {
            throw new NotImplementedException();
        }

        private void PlayerRequestReceived(PlayerRequestPacket request, Connection connection)
        {
            Console.WriteLine($"Request received {request.Request}");
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
