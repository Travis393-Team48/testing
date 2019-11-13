using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network.Attributes;

[PacketRequest(typeof(PlayerRequestPacket))]
public class PlayerResponsePacket : ResponsePacket
{
    public PlayerResponsePacket(string ResponseJson, RequestPacket request)
            : base(request)
    {
        Response = ResponseJson;
    }

    //This is not a string, it's a Json data type
    public string Response { get; set; }
}
