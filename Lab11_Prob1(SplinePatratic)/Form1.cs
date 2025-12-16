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

namespace Lab11_Prob1_SplinePatratic_
{
    public partial class Form1 : Form
    {
        double[] x;
        double[] y;
        public Form1()
        {
            InitializeComponent();
            InitChart();
            InitData();
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

            y = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Math.Cos(x[i]);
        }

        void InitChart()
        {
            chart1.Series.Clear();

            // cos(x) – negru
            var cosSeries = chart1.Series.Add("cos(x)");
            cosSeries.ChartType = SeriesChartType.Line;
            cosSeries.Color = Color.Black;
            cosSeries.BorderWidth = 4;

            // Puncte (xi, yi) – rosu
            var pointsSeries = chart1.Series.Add("Puncte (xi, yi)");
            pointsSeries.ChartType = SeriesChartType.Point;
            pointsSeries.Color = Color.Red;
            pointsSeries.MarkerSize = 8;

            // Spline liniar – albastru
            var splineSeries = chart1.Series.Add("Spline liniar");
            splineSeries.ChartType = SeriesChartType.Line;
            splineSeries.Color = Color.Blue;
            splineSeries.BorderWidth = 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var s in chart1.Series)
                s.Points.Clear();

            // 1️⃣ cos(x) – negru
            int Ncos = 1000;
            double a = 0;
            double b = Math.PI / 2;
            double step = (b - a) / Ncos;

            for (int i = 0; i <= Ncos; i++)
            {
                double u = a + i * step;
                chart1.Series["cos(x)"].Points.AddXY(u, Math.Cos(u));
            }

            // 2️⃣ Punctele (xi, yi) – rosu
            for (int i = 0; i < x.Length; i++)
                chart1.Series["Puncte (xi, yi)"].Points.AddXY(x[i], y[i]);

            // 3️⃣ Spline liniar (u[k], S[k]) – albastru
            int N = 1000;
            double p = (b - a) / N;

            for (int k = 0; k <= N; k++)
            {
                double uk = a + k * p;
                int j = FindInterval(uk);

                double Sk =
                    y[j - 1] +
                    (y[j] - y[j - 1]) /
                    (x[j] - x[j - 1]) *
                    (uk - x[j - 1]);

                chart1.Series["Spline liniar"].Points.AddXY(uk, Sk);
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
