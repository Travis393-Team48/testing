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
    public PlayerResponsePacket(JToken ResponseJToken, RequestPacket request)
            : base(request)
    {
        this.ResponseJToken = ResponseJToken;
    }

    [PacketIgnoreProperty]
    public JToken ResponseJToken { get; set; }

    public string Response { get; set; }

    public override void BeforeSend()
    {
        Response = JsonConvert.SerializeObject(ResponseJToken);
        base.BeforeSend();
    }

    public override void BeforeReceive()
    {
        ResponseJToken = JToken.Parse(Response);
        base.BeforeReceive();
    }
}
