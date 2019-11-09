﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using Network;
using Network.Enums;

namespace PlayerSpace
{
    /* Server-side Player
     * Networks between PlayerProxy and a Player
     * Holds a client-side listener
     * Establishes a connection to server and PacketHandler when created
     * 
     * When a PlayerRequestPacket is received, deciphers the packet
     *  and calls a function in PlayerWrapper depending the packet
     * May also send a response message to the server
     */
    public class PlayerClient
    {
        private PlayerWrapper _player;

        ClientConnectionContainer clientConnectionContainer;

        public PlayerClient(string ip, int port)
        {
<<<<<<< HEAD
	        string IP;
	        int port;
	        cfJson = configJson("../go.config");
	        IP = cfJson["IP"];
	        port = cfJson["port"];

            clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(IP, port);
=======
            clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(ip, port);
>>>>>>> 0e484b7e3dedc373d8cc63227879408877d4544e
            clientConnectionContainer.ConnectionEstablished += ConnectionEstablished;
            clientConnectionContainer.ConnectionLost += ConnectionLost;

<<<<<<< HEAD
            Console.WriteLine("hello world");
=======
            _player = new PlayerWrapper(false);
>>>>>>> 0e484b7e3dedc373d8cc63227879408877d4544e
        }

        private void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            if (connectionType == ConnectionType.TCP)
                connection.RegisterPacketHandler<PlayerRequestPacket>(PlayerRequestReceived, this);

            if (connectionType == ConnectionType.TCP)
                Console.WriteLine($"{connectionType} Connection received {connection.IPRemoteEndPoint}.");
        }

        private void ConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            Console.WriteLine("Connection Lost: " + closeReason);
        }

        private void PlayerRequestReceived(PlayerRequestPacket packet, Connection connection)
        {
            Console.WriteLine($"Request received {packet.Request}");

            JArray requestArray = JsonConvert.DeserializeObject<JArray>(packet.Request);
            PlayerResponsePacket response;

<<<<<<< HEAD
	    private string configJson(string file)
	    {
		    string json;
		    using (StreamReader sr = new StreamReader(file))
		    {
			    json = sr.ReadToEnd();
		    }

		    return json;
	    }
=======
            switch (requestArray[0].ToObject<string>())
            {
                case "Register":
                    string register = _player.Register(
                        requestArray[1].ToObject<string>(),
                        requestArray[2].ToObject<string>(),
                        requestArray[3].ToObject<int>());
                    response = new PlayerResponsePacket(JsonConvert.SerializeObject(register), packet);
                    connection.Send(response);
                    return;
                case "ReceiveStones":
                    _player.ReceiveStones(requestArray[1].ToObject<string>());
                    return;
                case "MakeAMove":
                    string move = _player.MakeAMove(requestArray[1].ToObject<string[][][]>());
                    response = new PlayerResponsePacket(JsonConvert.SerializeObject(move), packet);
                    connection.Send(response);
                    return;
                case "GetStone":
                    string stone = _player.GetStone();
                    response = new PlayerResponsePacket(JsonConvert.SerializeObject(stone), packet);
                    connection.Send(response);
                    return;
                case "GetName":
                    string name = _player.GetStone();
                    response = new PlayerResponsePacket(JsonConvert.SerializeObject(name), packet);
                    connection.Send(response);
                    return;
            }

            throw new PlayerClientException("Invalid operation sent to PlayerClient");
        }
>>>>>>> 0e484b7e3dedc373d8cc63227879408877d4544e
    }
}
