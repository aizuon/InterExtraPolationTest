using Client.Extensions;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public static int LocalX;
        public static int LocalY;
        public static readonly Pen LocalPen = new Pen(Color.Blue);

        public static int NetworkedTargetX;
        public static int NetworkedTargetY;
        public static int NetworkedX;
        public static int NetworkedY;
        public static readonly Pen NetworkedPen = new Pen(Color.Red);
        public static int NetworkedOldX;
        public static int NetworkedOldY;

        private const int Speed = 5;

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawCircle(LocalPen, LocalX, LocalY, 5);

            if (NetworkedX != NetworkedTargetX || NetworkedY != NetworkedTargetY)
            {
                NetworkedX = PositionUtils.Lerp(NetworkedOldX, NetworkedTargetX, 1.0f / Program.DeltaFactor);
                NetworkedY = PositionUtils.Lerp(NetworkedOldY, NetworkedTargetY, 1.0f / Program.DeltaFactor);
            }
            e.Graphics.DrawCircle(NetworkedPen, NetworkedX, NetworkedY, 5);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    LocalY -= Speed;
                    break;

                case Keys.S:
                    LocalY += Speed;
                    break;

                case Keys.D:
                    LocalX += Speed;
                    break;

                case Keys.A:
                    LocalX -= Speed;
                    break;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Running = false;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            var thread2 = new Thread(() =>
            {
                while (Program.Running)
                {
                    Invoke(new Action(() => Refresh()));
                    Thread.Sleep(Program.DeltaTime);
                }
            });
            thread2.Start();
        }
    }
}
