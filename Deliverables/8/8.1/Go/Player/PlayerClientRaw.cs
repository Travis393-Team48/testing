﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
    public class PlayerClientRaw
    {
        private PlayerWrapper _player;
        private Socket sender;

        public PlayerClientRaw(string ip, int port)
        {
            _player = new PlayerWrapper(false);

            if (ip == "localhost")
                ip = "127.0.0.1";

            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            ConnectToServer(localEndPoint);
        }

        private void ConnectToServer(IPEndPoint localEndPoint)
        {
            // Connect Socket to the remote endpoint using method Connect() 
            sender.Connect(localEndPoint);
            StartReceiving();
        }

        private void StartReceiving()
        {
            try
            {
                while (sender.Connected)
                {
                    // Data buffer 
                    byte[] messageReceived = new byte[8192];
                    byte[] messageSent;

                    // We receive the message using the method Receive(). This  
                    // method returns number of bytes received, that we'll use to convert them to string 
                    int byteRecv = sender.Receive(messageReceived);
                    string message = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);

                    JArray requestArray = JsonConvert.DeserializeObject<JArray>(message);

                    switch (requestArray[0].ToObject<string>())
                    {
                        case "register":
                            string register;
                            if (requestArray.Count > 1)
                            {
                                register = _player.Register(
                                    requestArray[1].ToObject<string>(),
                                    requestArray[2].ToObject<string>(),
                                    requestArray[3].ToObject<int>());
                            }
                            else
                            {
                                register = _player.Register("remote player", "illegal");
                            }
                            // Creation of message that we will send to Server
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(register));
                            sender.Send(messageSent);
                            break;

                        case "receive-stones":
                            _player.ReceiveStones(requestArray[1].ToObject<string>());
                            break;

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
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(move));
                            sender.Send(messageSent);
                            break;

                        case "GetStone":
                            string stone = _player.GetStone();
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(stone));
                            sender.Send(messageSent);
                            break;

                        case "GetName":
                            string name = _player.GetStone();
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(name));
                            sender.Send(messageSent);
                            break;
                        default:
                            throw new PlayerClientException("Invalid operation sent to PlayerClient");
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message, this);
                Console.ReadLine();
            }

            // Close Socket using the method Close() 
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public bool IsConnected()
        {
            return sender.Connected;
        }
    }
}