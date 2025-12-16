using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        double[] x;
        double[] y;
        int n;
        public Form1()
        {
            InitializeComponent();
            InitData();
            InitChart();
        }

        void InitData()
        {
            x = new double[]
            {
                0,
                Math.PI / 8,
                Math.PI / 6,
                Math.PI / 4,
                Math.PI / 3,
                Math.PI / 2
            };

            n = x.Length - 1;

            y = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Math.Cos(x[i]);
        }

        void InitChart()
        {
            chart1.Series.Clear();

            AddSeries("cos(x)", Color.Black, SeriesChartType.Line, 3);
            AddSeries("Puncte", Color.Red, SeriesChartType.Point, 0);
            chart1.Series["Puncte"].MarkerSize = 8;

            //AddSeries("Spline liniar", Color.Blue, SeriesChartType.Line, 2);
            AddSeries("Spline patratic", Color.Green, SeriesChartType.Line, 2);
        }

        void AddSeries(string name, Color color, SeriesChartType type, int width)
        {
            var s = chart1.Series.Add(name);
            s.ChartType = type;
            s.Color = color;
            s.BorderWidth = width;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var s in chart1.Series)
                s.Points.Clear();

            double a = 0;
            double b = Math.PI / 2;
            int N = 1000;
            double p = (b - a) / N;

            // ---------------- COS(x) ----------------
            for (int k = 0; k <= N; k++)
            {
                double u = a + k * p;
                chart1.Series["cos(x)"].Points.AddXY(u, Math.Cos(u));
            }

            // ---------------- PUNCTE ----------------
            for (int i = 0; i < x.Length; i++)
                chart1.Series["Puncte"].Points.AddXY(x[i], y[i]);

            // ---------------- SPLINE LINIAR ----------------
            //for (int k = 0; k <= N; k++)
            //{
            //    double uk = a + k * p;
            //    int j = FindInterval(uk);

            //    double Sk =
            //        y[j - 1] +
            //        (y[j] - y[j - 1]) /
            //        (x[j] - x[j - 1]) *
            //        (uk - x[j - 1]);

            //    chart1.Series["Spline liniar"].Points.AddXY(uk, Sk);
            //}

            // ---------------- SPLINE PĂTRATIC ----------------

            // Pas 1: h[i]
            double[] h = new double[x.Length];
            for (int i = 1; i <= n; i++)
                h[i] = x[i] - x[i - 1];

            // Pas 2: m[i]
            double[] m = new double[x.Length];

            m[0] =
                ((2 * h[1] + h[2]) / (h[1] * (h[1] + h[2]))) * (y[1] - y[0])
                - (1.0 / (h[1] + h[2])) * (y[2] - y[1]);

            for (int i = 1; i <= n; i++)
                m[i] = -m[i - 1] + (2.0 / h[i]) * (y[i] - y[i - 1]);

            // Pas 3: S(k)
            for (int k = 0; k <= N; k++)
            {
                double uk = a + k * p;
                int j = FindInterval(uk);

                double Sk =
                    ((m[j] - m[j - 1]) / (2 * h[j])) * Math.Pow(uk - x[j - 1], 2)
                    + m[j - 1] * (uk - x[j - 1])
                    + y[j - 1];

                chart1.Series["Spline patratic"].Points.AddXY(uk, Sk);
            }
        }

        int FindInterval(double u)
        {
            for (int j = 1; j < x.Length; j++)
                if (u >= x[j - 1] && u <= x[j])
                    return j;

            return x.Length - 1;
        }
    }
}
