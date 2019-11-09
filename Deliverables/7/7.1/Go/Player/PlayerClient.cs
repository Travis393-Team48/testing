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
using Network.RSA;
using TestServerClientPackets;

namespace PlayerSpace
{
    public class PlayerClient
    {
        private PlayerAdapter _player;

        ClientConnectionContainer clientConnectionContainer;

        public PlayerClient(string ip, int port)
        {
            clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(ip, port);
            clientConnectionContainer.ConnectionEstablished += ConnectionEstablished;
            clientConnectionContainer.ConnectionLost += ConnectionLost;
        }

        private void ConnectionEstablished(Connection connection, ConnectionType connectionType)
        {
            if (connectionType == ConnectionType.TCP)
                connection.RegisterPacketHandler<PlayerRequestPacket>(PlayerRequestReceived, this);

            if (connectionType == ConnectionType.TCP)
                Console.WriteLine($"{connectionType} Connection received {connection.IPRemoteEndPoint}.");

            if (connectionType == ConnectionType.TCP)
                connection.RegisterStaticPacketHandler<CalculationRequest>(calculationReceived);
        }

        private void ConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            Console.WriteLine("Connection Lost: " + closeReason);
        }

        private static void PlayerRequestReceived(PlayerRequestPacket packet, Connection connection)
        {
            Console.WriteLine($"Request received {packet.RequestJToken}");

            connection.Send(new PlayerResponsePacket(packet.RequestJToken, packet));
        }

        private static void calculationReceived(CalculationRequest packet, Connection connection)
        {
            //4. Handle incoming packets.
            connection.Send(new CalculationResponse(packet.X + packet.Y, packet));
        }


    }
}
