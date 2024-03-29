﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PlayerSpace
{
    /* Server-side Player (implements IPlayer)
     * Networks between some server and a PlayerClient
     * Holds a server-side listener
     * Behavior is the same as a Player (although throws different type of exception)
     * Sends requests to the PlayerClient using as byte arrays and System.Net
     * 
     * Protocals of Interaction:
     *  Register must be called before use of other functions
     *  ReceiveStones must be called before MakeAMove
     *  GetStones must be called after ReceiveStones
     */
    class PlayerProxy : IPlayer
    {
        Socket clientSocket;
        private string _name;
        private string _stone;

        public PlayerProxy(int port)
        {
            // Establish the local endpoint for the socket. Dns.GetHostName 
            // returns the name of the host running the application. 
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

            // Creation TCP/IP Socket using Socket Class Costructor 
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(2);

            clientSocket = listener.Accept();
        }

        public string Register(string name)
        {
            try
            {
                JArray array = new JArray();
                array.Add("register");

                // Send a message to Client  using Send() method 
                byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(array));
                clientSocket.Send(message);

                byte[] bytes = new Byte[1024];
                int numByte = clientSocket.Receive(bytes);
                string data = Encoding.ASCII.GetString(bytes, 0, numByte);

                string remote_name = JsonConvert.DeserializeObject<string>(data);
                _name = remote_name;

                return remote_name;
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                throw;
            }
        }

        public void ReceiveStones(string stone)
        {
            try
            {
                JArray array = new JArray();
                array.Add("receive-stones");
                array.Add(stone);

                // Send a message to Client  using Send() method 
                byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(array));
                clientSocket.Send(message);

                _stone = stone;

                return;
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                throw;
            }
        }

        public string MakeAMove(string[][][] boards)
        {
            try
            {
                JArray array = new JArray();
                array.Add("make-a-move");
                array.Add(JToken.Parse(JsonConvert.SerializeObject(boards)));

                // Send a message to Client  using Send() method 
                byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(array));
                clientSocket.Send(message);

                byte[] bytes = new Byte[8192];
                int numByte = clientSocket.Receive(bytes);
                string data = Encoding.ASCII.GetString(bytes, 0, numByte);

                return JsonConvert.DeserializeObject<string>(data);
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                throw;
            }
        }

        public string GetStone()
        {
            return _stone;
        }

        public string GetName()
        {
            return _name;
        }

    }
}
