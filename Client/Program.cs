using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public static class Program
    {
        public static volatile bool Running;

        public static NetManager Client;

        public static MainForm MainForm;

        public const int DeltaTime = 16;
        public const int DeltaFactor = 2;

        [STAThread]
        public static void Main()
        {
            var listener = new EventBasedNetListener();
            Client = new NetManager(listener);
            Client.Start();
            Client.Connect("127.0.0.1", 9050, "InterExtraPolationTest");

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                MainForm.NetworkedOldX = MainForm.NetworkedTargetX;
                MainForm.NetworkedOldY = MainForm.NetworkedTargetY;
                MainForm.NetworkedTargetX = dataReader.GetInt();
                MainForm.NetworkedTargetY = dataReader.GetInt();

                dataReader.Recycle();
            };

            Running = true;

            var thread = new Thread(() =>
            {
                while (Running)
                {
                    var writer = new NetDataWriter();
                    writer.Put(MainForm.LocalX);
                    writer.Put(MainForm.LocalY);
                    Client.FirstPeer.Send(writer, DeliveryMethod.Unreliable);

                    Client.PollEvents();
                    Thread.Sleep(DeltaFactor * DeltaTime);
                }
                Client.Stop();
            });
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
            Application.Run(MainForm);
        }
    }
}
