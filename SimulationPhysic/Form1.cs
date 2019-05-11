using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SimulationPhysic
{
    public partial class Form1 : Form
    {
        //farben nach ladung

        double minTimeStepS = 0.0000000001;
        double zoom = 50;

        Stopwatch sw = new Stopwatch();
        Pen pen;
        PhysicalSystem system;
        Thread thread;
        double startMouseX = 0, startMouseY = 0, offsetMouseX = 0, offsetMouseY = 0;
        bool mouseDown = false, paused = true;
        int frameTimeMs;

        private void Form1_Load(object sender, EventArgs e)
        {
            frameTimeMs = (int)(1000f / int.Parse(textBox1.Text));
            pen = new Pen(Color.Black, 2);
            var objects = new Object[] {
                new Positron(new Vector3(0, 0.1, 0))/*, new Vector3(8,0,0))*/,
                new Electron(new Vector3(0, -0.1, 0))/*, new Vector3(-8,0,0)),
                new Electron(new Vector3(-5, 0, 0)),
                new Electron(new Vector3(-4, 0, 0)),
                new Proton(new Vector3(0, 0, 0))*/
            };
            system = new PhysicalSystem(minTimeStepS, objects);
            thread = new Thread(new ThreadStart(ThreadRoutine));
            thread.Start();
        }

        void ThreadRoutine()
        {
            for (; ; )
            {
                if (!paused)
                {
                    long startMs = sw.ElapsedMilliseconds;
                    sw.Start();
                    while (sw.ElapsedMilliseconds - startMs < frameTimeMs)
                        system.Proceed(100);
                }
                else
                {
                    sw.Stop();
                    Thread.Sleep(frameTimeMs);
                }
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var b in system.objects)
            {
                double scaledXPos = (b.x.X - offsetMouseX) * zoom - (mouseDown ? startMouseX - MousePosition.X : 0);
                double scaledYPos = (b.x.Y - offsetMouseY) * zoom - (mouseDown ? startMouseY - MousePosition.Y : 0);
                if (scaledXPos > -Width / 2 && scaledXPos <= Width / 2 && scaledYPos > -Height / 2 && scaledYPos <= Height / 2)
                    e.Graphics.DrawCircle(pen, (int)scaledXPos + Width / 2, (int)scaledYPos + Height / 2, (float)Math.Ceiling(b.r * zoom));
            }
            label1.Text = system.time.ToString();
            label5.Text = (system.time * 1000 / sw.ElapsedMilliseconds).ToString();
            label4.Text = (offsetMouseX + (mouseDown ? startMouseX - MousePosition.X : 0) / zoom) + "; " + (offsetMouseY + (mouseDown ? startMouseY - MousePosition.Y : 0) / zoom);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zoom *= 2;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            frameTimeMs = (int)(1000f / double.Parse(textBox1.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zoom /= 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (paused == true)
            {
                paused = false;
                button3.Text = "| |";
            }
            else
            {
                paused = true;
                button3.Text = "|>";
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            offsetMouseX += (startMouseX - MousePosition.X) / zoom;
            offsetMouseY += (startMouseY - MousePosition.Y) / zoom;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            startMouseX = MousePosition.X;
            startMouseY = MousePosition.Y;
        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen, float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush, float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }
    }
}
