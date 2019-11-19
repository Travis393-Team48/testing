using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network;
using System.Threading.Tasks;
using CustomExceptions;

namespace PlayerSpace
{
    /* Server-side Player (implements IPlayer)
     * Networks between some server and a PlayerClient
     * Holds a server-side listener
     * Behavior is the same as a Player (although throws different type of exception)
     * Also holds the name and stone of player which is set afteer Register and ReceiveStones
     *  kept here as the remote player may not have methods available to retrieve this  data
     * 
     * Sends requests to the PlayerClient using Packets
     * Packets contains a JArray in a generic Json data format (string)
     * First item of JArray is the method name that should be called on client side
     * Other items are arguments passed to method
     * 
     * Protocals of Interaction:
     *  Register must be called before use of other functions
     *  ReceiveStones must be called before MakeAMove
     *  GetStones must be called after ReceiveStones
     */
    class PlayerProxy : IPlayer
    {
        ServerConnectionContainer _serverConnectionContainer;
        private string _name;
        private string _stone;

        public PlayerProxy(int port)
        {
            //Create a new server container.
            _serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(port, false);

            //Start listening on port port
            _serverConnectionContainer.StartTCPListener();
            
        }

        public string Register(string name)
        {
            Task<string> response = RegisterAsync(name);
            while (!response.IsCompleted) { }
            return response.Result;
        }

        private async Task<string> RegisterAsync(string name)
        {
            JArray array = new JArray();
            array.Add("register");

            PlayerRequestPacket packet = new PlayerRequestPacket(JsonConvert.SerializeObject(array));
            PlayerResponsePacket response = await _serverConnectionContainer.TCP_Connections[0].SendAsync<PlayerResponsePacket>(packet);
            return JsonConvert.DeserializeObject<string>(response.Response);
        }

        public void ReceiveStones(string stone)
        {
            Task response = ReceiveStonesAsync(stone);
            while (!response.IsCompleted) { }
            return;
        }

        private async Task ReceiveStonesAsync(string stone)
        {
            JArray array = new JArray();
            array.Add("receive-stones");
            array.Add(stone);

            PlayerRequestPacket packet = new PlayerRequestPacket(JsonConvert.SerializeObject(array));
            await _serverConnectionContainer.TCP_Connections[0].SendAsync<PlayerResponsePacket>(packet);
        }

        public string MakeAMove(string[][][] boards)
        {
            Task<string> response = MakeAMoveAsync(boards);
            while (!response.IsCompleted) { }
            return response.Result;
        }

        private async Task<string> MakeAMoveAsync(string[][][] boards)
        {
            JArray array = new JArray();
            array.Add("make-a-move");
            array.Add(JToken.Parse(JsonConvert.SerializeObject(boards)));

            PlayerRequestPacket packet = new PlayerRequestPacket(JsonConvert.SerializeObject(array));
            PlayerResponsePacket response = await _serverConnectionContainer.TCP_Connections[0].SendAsync<PlayerResponsePacket>(packet);
            return JsonConvert.DeserializeObject<string>(response.Response);
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
