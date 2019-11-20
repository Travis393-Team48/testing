using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PlayerSpace
{
    /* Illegal Client-side Player that disconnects/send incorrect json depending on configuration
     * Configurations:
     * "disconnect on connect A" - disconnect client right after connecting to server, before calling StartReceiving()
     * "disconnect on connect B" - disconnect client right after connecting to server, after calling StartReceiving()
     * "send json array on register"
     * "send json array on make a move"
     * "send json object on make a move"
     * "disconnect on register"
     * "disconnect on receive stones"
     * "disconnect on make a move"
     */
    public class PlayerClientIllegal
    {
        private PlayerWrapper _player;
        private Socket sender;
        private string _name;
        string _configuration;

        public PlayerClientIllegal(string ip, int port, string aiType, string configuration)
        {
            _configuration = configuration;
            if (aiType == "illegal")
                _player = new PlayerWrapper(aiType, configuration);
            else
                _player = new PlayerWrapper("smart", 1);
            _name = "illegal player - " + configuration;

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
            sender.BeginConnect(localEndPoint, ConnectCallback, sender);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;
            // Complete the connection.  
            client.EndConnect(ar);

            if (_configuration == "disconnect on connect A")
                ForceDisconnect();

            StartReceiving();
        }

        private void StartReceiving()
        {
            try
            {
                while (sender.Connected)
                {
                    if (_configuration == "disconnect on connect B")
                        ForceDisconnect();

                    byte[] messageReceived = new byte[8192];
                    byte[] messageSent;
                    int byteRecv = sender.Receive(messageReceived);
                    string message = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);

                    JArray requestArray = JsonConvert.DeserializeObject<JArray>(message);

                    switch (requestArray[0].ToObject<string>())
                    {
                        case "register":
                            if (_configuration == "disconnect on register")
                                ForceDisconnect();
                            if (_configuration == "send json array on register")
                            {
                                messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new string[1] {"json array"}));
                                sender.Send(messageSent);
                                break;
                            }
                            //normal functionality:
                            string register = _player.Register(_name);
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(register));
                            sender.Send(messageSent);
                            break;

                        case "receive-stones":
                            if (_configuration == "disconnect on receive stones")
                                ForceDisconnect();
                            _player.ReceiveStones(requestArray[1].ToObject<string>());
                            break;

                        case "make-a-move":
                            if (_configuration == "disconnect on make a move")
                                ForceDisconnect();
                            if (_configuration == "send json array on make a move")
                            {
                                messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new string[1] { "json array" }));
                                sender.Send(messageSent);
                                break;
                            }
                            if (_configuration == "send json object on make a move")
                            {
                                JObject json = new JObject();
                                json.Add("invalid", "json");
                                messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json));
                                sender.Send(messageSent);
                                break;
                            }

                            //Normal functionality:
                            string move;
                            try
                            {
                                move = _player.MakeAMove(requestArray[1].ToObject<string[][][]>());
                            }
                            catch (PlayerException e) //Board history is illegal
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
                            string name = _player.GetName();
                            messageSent = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(name));
                            sender.Send(messageSent);
                            break;
                        default:
                            sender.Shutdown(SocketShutdown.Both);
                            sender.Close();
                            throw new PlayerClientException("Invalid operation sent to PlayerClient");
                    }
                }
            }

            catch (Exception e)
            {
                //Console.WriteLine(e.Message, this);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                //Console.ReadLine();
                throw;
            }

            // Close Socket using the method Close() 
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public bool IsConnected()
        {
            return sender.Connected;
        }

        private void ForceDisconnect()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            throw new PlayerClientException("Force Disconnection - " + _configuration);
        }
    }
}
