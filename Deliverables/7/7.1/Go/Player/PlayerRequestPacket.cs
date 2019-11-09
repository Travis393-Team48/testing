using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network.Packets;
using Network.Attributes;

public class PlayerRequestPacket : RequestPacket
{
    public PlayerRequestPacket(string RequestJson)
    {
        Request = RequestJson;
    }

    public string Request { get; set; }

}
