using FireUp.Config;
using FireUp.Samples.Json.Core;
using Newtonsoft.Json;

namespace FireUp.Samples.Json.Client
{
    public class JsonClient : Network.Client
    {
        public JsonClient(string host, int port, ClientConfiguration config = null) : base(host, port, config) { }

        public void Send(Packet packet)
        {
            Send(JsonConvert.SerializeObject(packet));
        }
    }
}
