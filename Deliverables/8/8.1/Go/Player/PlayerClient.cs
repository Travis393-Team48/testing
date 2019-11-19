using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using Network;
using Network.Enums;
using Network.Packets;

namespace PlayerSpace
{
    /* Client-side Player
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
        private string _name;

        ClientConnectionContainer clientConnectionContainer;

        public PlayerClient(string ip, int port, string aiType, int n = 1, string name = "my player client")
        {
            clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(ip, port);
            clientConnectionContainer.ConnectionEstablished += ConnectionEstablished;
            clientConnectionContainer.ConnectionLost += ConnectionLost;

            _player = new PlayerWrapper(aiType, n);
            _name = name;

            while (!clientConnectionContainer.IsAlive) { }
        }

        private void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            if (connectionType == ConnectionType.TCP)
                connection.RegisterPacketHandler<PlayerRequestPacket>(PlayerRequestReceived, this);

            //if (connectionType == ConnectionType.TCP)
            //    Console.WriteLine($"{connectionType} Connection received {connection.IPRemoteEndPoint}.");
        }

        private void ConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            //Console.WriteLine("Connection Lost: " + closeReason);
        }

        private void PlayerRequestReceived(PlayerRequestPacket packet, Connection connection)
        {
            //Console.WriteLine($"Request received {packet.Request}");

            JArray requestArray = JsonConvert.DeserializeObject<JArray>(packet.Request);
            PlayerResponsePacket response;

            switch (requestArray[0].ToObject<string>())
            {
                case "register":
                    string register = _player.Register(_name);
                    response = new PlayerResponsePacket(JsonConvert.SerializeObject(register), packet);
                    connection.Send(response);
                    return;
                case "receive-stones":
                    _player.ReceiveStones(requestArray[1].ToObject<string>());
                    return;
                case "make-a-move":
                    string move;
                    try
                    {
                        move = _player.MakeAMove(requestArray[1].ToObject<string[][][]>());
                    }
                    catch (PlayerException e)
                    {
                        move = e.Message;
                    }
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

        public bool IsAlive()
        {
            return clientConnectionContainer.IsAlive;
        }
    }
}
