using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network;
using Network.Enums;
using Network.Packets;
using System.Threading;
using System.Threading.Tasks;
using TestServerClientPackets;

namespace PlayerSpace
{
    class PlayerProxy : IPlayer
    {
        ServerConnectionContainer serverConnectionContainer;
        string _response;

        public PlayerProxy(int port)
        {
            //Create a new server container.
            serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(port, false);

            //Set a delegate which will be called if we receive a connection
            serverConnectionContainer.ConnectionEstablished += ConnectionEstablished;

            //Start listening on port port
            serverConnectionContainer.StartTCPListener();
        }

        private void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            //connection.RegisterPacketHandler<PlayerResponsePacket>(PlayerResponseReceived, this);
            GetStoneAsync();
        }

        private void PlayerResponseReceived(PlayerResponsePacket response, Connection connection)
        {
            Console.WriteLine($"Response received {response.ResponseJToken}");
            _response = response.Response;
        }

        public void ReceiveStones(string stone)
        {
            PlayerRequestPacket packet = new PlayerRequestPacket(JToken.Parse(JsonConvert.SerializeObject("B")));
            serverConnectionContainer.TCP_Connections[0].Send(packet);
            Console.WriteLine("Stones Sent");
        }

        public string MakeAMove(string[][][] boards)
        {
            PlayerRequestPacket packet = new PlayerRequestPacket(JToken.Parse(JsonConvert.SerializeObject("string")));
            serverConnectionContainer.TCP_Connections[0].Send(packet, this);
            return _response.ToString();
        }

        public string GetStone()
        {
            Task<string> response = GetStoneAsync();
            while (!response.IsCompleted) { }
            return response.Result;
        }

        public async Task<string> GetStoneAsync()
        {
            PlayerRequestPacket packet = new PlayerRequestPacket(JToken.Parse(JsonConvert.SerializeObject("GetStone")));
            //PlayerResponsePacket response = await serverConnectionContainer.TCP_Connections[0].SendAsync<PlayerResponsePacket>(new RequestPacket());

            CalculationResponse response = await serverConnectionContainer.TCP_Connections[0].SendAsync<CalculationResponse>(new CalculationRequest(10, 10));

            //Console.WriteLine($"Response received {response.ResponseJToken}");
            _response = response.Result.ToString();
            return _response;
        }

        private PlayerResponsePacket Test()
        {
            return new PlayerResponsePacket(JToken.Parse(JsonConvert.SerializeObject("GetStone")), new RequestPacket());
        }

        public string GetName()
        {
            PlayerRequestPacket packet = new PlayerRequestPacket(JToken.Parse(JsonConvert.SerializeObject("string")));
            serverConnectionContainer.TCP_Connections[0].Send(packet, this);
            return _response.ToString();
        }


    }
}
