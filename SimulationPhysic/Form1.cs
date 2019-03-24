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
        double minTimeStep = 0.000000001;
        int calculationsUntilRefresh = 40000;
        int timeUntilRefresh = 0;
        double zoom = 50;
        readonly Vector3 observedCenter = new Vector3();
        Thread thread;

        private void Form1_Load(object sender, EventArgs e)
        {
            pen = new Pen(Color.Black, 2);
            system = new PhysicalSystem(minTimeStep);
            var charges = new Charge[] {
                new Electron(new Vector3(0, -0.5, 0)),
                /*new Electron(new Vector3(5, 0, 0)),
                new Electron(new Vector3(-5, 0, 0)),
                new Electron(new Vector3(-4, 0, 0)),*/
                new Positron(new Vector3(0, 0.5, 0))//,
                //new Proton(new Vector3(0, 0, 0))
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
                double outputPosX = (b.pos.x - observedCenter.x) * zoom + Width / 2;
                double outputPosY = (b.pos.y - observedCenter.y) * zoom + Height / 2;
                if (outputPosX >= 0 && outputPosX < Width && outputPosY >= 0 && outputPosY < Height)
                    e.Graphics.DrawCircle(pen, (int)outputPosX, (int)outputPosY, (float)Math.Ceiling(b.radius*zoom));
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            calculationsUntilRefresh = int.Parse(textBox1.Text);
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
