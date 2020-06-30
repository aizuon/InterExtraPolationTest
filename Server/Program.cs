using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace Server
{
    public static class Program
    {
        public static void Main()
        {
            var listener = new EventBasedNetListener();
            var server = new NetManager(listener);
            server.Start(IPAddress.Parse("127.0.0.1"), IPAddress.IPv6Loopback, 9050);

            listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("InterExtraPolationTest");
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var writer = new NetDataWriter();
                writer.Put(dataReader.GetInt());
                writer.Put(dataReader.GetInt());

                foreach (var peer in server.ConnectedPeerList.Where(p => p.Id != fromPeer.Id))
                    peer.Send(writer, DeliveryMethod.Unreliable);

                dataReader.Recycle();
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(8);
            }
            server.Stop();
        }
    }
}
