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

namespace WindowsFormsApp2
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
            // EXEMPLUL DE PE TABLĂ
            x = new double[] { 7.5, 10.5, 13, 15.5, 18, 21, 24, 27 };
            y = new double[] { 130, 121, 128, 96, 122, 138, 114, 90 };

            n = x.Length - 1;
        }

        void InitChart()
        {
            chart1.Series.Clear();

            // puncte (xi, yi) - ROSU
            var p = chart1.Series.Add("Puncte (xi, yi)");
            p.ChartType = SeriesChartType.Point;
            p.Color = Color.Red;
            p.MarkerSize = 8;

            // spline cubic natural - ALBASTRU
            var s = chart1.Series.Add("Spline cubic natural");
            s.ChartType = SeriesChartType.Line;
            s.Color = Color.Blue;
            s.BorderWidth = 2;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var s in chart1.Series)
                s.Points.Clear();

            double a = x[0];
            double b = x[n];
            int N = 1000;
            double p = (b - a) / N;

            // ---------------- PUNCTE ----------------
            for (int i = 0; i <= n; i++)
                chart1.Series["Puncte (xi, yi)"].Points.AddXY(x[i], y[i]);

            // ---------------- PASUL 1: h[i] ----------------
            double[] h = new double[n + 1];
            for (int i = 1; i <= n; i++)
                h[i] = x[i] - x[i - 1];

            // ---------------- PASUL 2 ----------------
            double[] aC = new double[n];
            double[] d = new double[n];

            for (int i = 1; i <= n - 1; i++)
            {
                aC[i] = 2;
                d[i] = (6.0 / (h[i] + h[i + 1])) *
                       ((y[i + 1] - y[i]) / h[i + 1] -
                        (y[i] - y[i - 1]) / h[i]);
            }

            // ---------------- PASUL 3 ----------------
            double[] bC = new double[n];
            double[] cC = new double[n];

            for (int i = 2; i <= n - 2; i++)
            {
                bC[i] = h[i] / (h[i] + h[i + 1]);
                cC[i] = 1 - bC[i];
            }

            bC[n - 1] = h[n - 1] / (h[n - 1] + h[n]);
            cC[1] = h[2] / (h[1] + h[2]);

            // ---------------- PASUL 4 ----------------
            double[] alpha = new double[n];
            double[] w = new double[n];

            alpha[1] = cC[1] / aC[1];

            for (int i = 2; i <= n - 2; i++)
            {
                w[i] = aC[i] - alpha[i - 1] * bC[i];
                alpha[i] = cC[i] / w[i];
            }

            w[n - 1] = aC[n - 1] - alpha[n - 2] * bC[n - 1];

            // ---------------- PASUL 5 ----------------
            double[] z = new double[n + 1];

            z[1] = d[1] / aC[1];

            for (int i = 2; i <= n - 1; i++)
                z[i] = (d[i] - bC[i] * z[i - 1]) / w[i];

            // ---------------- PASUL 6 ----------------
            double[] M = new double[n + 1];
            M[0] = 0;
            M[n] = 0;
            M[n - 1] = z[n - 1];

            for (int i = n - 2; i >= 1; i--)
                M[i] = z[i] - alpha[i] * M[i + 1];

            // ---------------- PASUL 7: S(k) ----------------
            for (int k = 0; k <= N; k++)
            {
                double uk = a + k * p;
                int j = FindInterval(uk);

                double Sk =
                    (Math.Pow(uk - x[j - 1], 3) / (6 * h[j])) * M[j]
                  + (Math.Pow(x[j] - uk, 3) / (6 * h[j])) * M[j - 1]
                  + ((uk - x[j-1]) / h[j]) * y[j]
                  + ((x[j] - uk) / h[j]) * y[j - 1];

                chart1.Series["Spline cubic natural"].Points.AddXY(uk, Sk);
            }
        }

        int FindInterval(double u)
        {
            for (int j = 1; j <= n; j++)
                if (u >= x[j - 1] && u <= x[j])
                    return j;

            return n;
        }
    }
}
