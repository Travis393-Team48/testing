using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Network.Packets;
using Network.Attributes;

public class PlayerRequestPacket : RequestPacket
{
    public PlayerRequestPacket(JToken RequestJToken)
    {
        this.RequestJToken = RequestJToken;
    }

    [PacketIgnoreProperty]
    public JToken RequestJToken { get; set; }

    public string Request { get; set; }

    public override void BeforeSend()
    {
        Request = JsonConvert.SerializeObject(RequestJToken);
        base.BeforeSend();
    }

}
