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
        const double minTimeStep = 0.0000005;
        const double zoom = 1;
        readonly Vector3 observedCenter = new Vector3(), observedRange = new Vector3(1, 1, 1) * 50;
        Thread thread;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pen = new Pen(Color.Black, 2);
            system = new PhysicalSystem(minTimeStep);
            var e1 = new Electron(new Vector3(0, -2, 0));
            var e2 = new Electron(new Vector3(5, 0, 0));
            var e3 = new Electron(new Vector3(-5, 0, 0));
            var e4 = new Electron(new Vector3(-4, 0, 0));
            var p1 = new Positron(new Vector3(0, 2, 0));
            var p2 = new Proton(new Vector3(0, 0, 0));
            system.AddCharge(e1, e2, e3, e4, p1, p2);
            thread = new Thread(new ThreadStart(() =>
            {
                var sw = new Stopwatch();
                for (; ; )
                {
                    sw.Start();
                    for (int i = 0; i < 40000; i++)
                        system.Proceed();
                    d = (int)sw.ElapsedMilliseconds;
                    this.Invalidate();
                    sw.Reset();
                }
            }));
            thread.Start();
        }

        int d = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (Body b in system.bodies)
            {
                Vector3 zoomedObservedRange = observedRange * (1 - 1000 / (float)(trackBar1.Value - 1000)) / 3;
                var outputPos = (b.pos - observedCenter + zoomedObservedRange / 2);
                outputPos.x *= Width / zoomedObservedRange.x;
                outputPos.y *= Height / zoomedObservedRange.y;
                outputPos.z *= 1 / zoomedObservedRange.z;
                if (outputPos.x >= 0 && outputPos.x < Width && outputPos.y >= 0 && outputPos.y < Height)
                    e.Graphics.DrawCircle(pen, (int)outputPos.x, (int)outputPos.y, 3);
            }
            label1.Text = system.time.ToString();
            label2.Text = (1000 / ((float)d)).ToString() + "Hz";
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
