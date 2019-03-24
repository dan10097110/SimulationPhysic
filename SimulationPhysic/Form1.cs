using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulationPhysic
{
    public partial class Form1 : Form
    {
        Pen pen;
        PhysicalSystem system;
        double minTimeStep = 0.00000005;
        int calculationsUntilRefresh = 20000;
        int timeUntilRefresh = 0;
        double zoom = 50;
        Thread thread;
        double startMouseX = 0, startMouseY = 0;
        double offsetMouseX = 0, offsetMouseY = 0;
        bool mouseDown = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            pen = new Pen(Color.Black, 2);
            system = new PhysicalSystem(minTimeStep);
            var charges = new Charge[] {
                new Electron(new Vector3(0, 0, 0)),
                new Positron(new Vector3(0, 4, 0))/*,
                new Electron(new Vector3(5, 0, 0)),
                new Electron(new Vector3(-5, 0, 0)),
                new Electron(new Vector3(-4, 0, 0)),
                new Proton(new Vector3(0, 0, 0))*/
            };
            system.AddCharge(charges);
            textBox1.Text = calculationsUntilRefresh.ToString();
            thread = new Thread(new ThreadStart(() =>
            {
                var sw = new Stopwatch();
                for (; ; )
                {
                    sw.Start();
                    for (int i = 0; i < calculationsUntilRefresh; i++)
                        system.Proceed();
                    timeUntilRefresh = (int)sw.ElapsedMilliseconds;
                    Invalidate();
                    sw.Reset();
                }
            }));
            thread.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var b in system.bodies)
            {
                double scaledXPos = b.pos.x * zoom - (offsetMouseX + (mouseDown ? startMouseX - MousePosition.X : 0));
                double scaledYPos = b.pos.y * zoom - (offsetMouseY + (mouseDown ? startMouseY - MousePosition.Y : 0));
                if (scaledXPos > -Width / 2 && scaledXPos <= Width / 2 && scaledYPos > -Height / 2 && scaledYPos <= Height / 2)
                    e.Graphics.DrawCircle(pen, (int)scaledXPos + Width / 2, (int)scaledYPos + Height / 2, (float)Math.Ceiling(b.radius * zoom));
            }
            label1.Text = system.time.ToString();
            label2.Text = (1000 / (float)timeUntilRefresh).ToString() + "Hz";
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

        private void button2_Click(object sender, EventArgs e)
        {
            zoom /= 2;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            offsetMouseX += (startMouseX - MousePosition.X);
            offsetMouseY += (startMouseY - MousePosition.Y);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            calculationsUntilRefresh = int.Parse(textBox1.Text);
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
