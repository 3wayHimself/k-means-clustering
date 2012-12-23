using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClusteringGUI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Random generator
        /// </summary>
        private static Random Rand = new Random();

        /// <summary>
        /// Panel graphics
        /// </summary>
        private Graphics G;

        /// <summary>
        /// Predefined cluster colours
        /// </summary>
        private Color[] Colors = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Black, Color.Violet, Color.Orange, Color.Navy, Color.Turquoise };

        /// <summary>
        /// Points added by user
        /// </summary>
        private List<Point> Points = new List<Point>();
        
        /// <summary>
        /// The clusters generated
        /// </summary>
        private List<List<Point>> Clusters;

        /// <summary>
        /// Whether the points have been passed through the algorithm
        /// </summary>
        private Boolean FirstCluster = false;

        /// <summary>
        /// The k-means cluster solver
        /// </summary>
        private KCluster ClusterAlgo;

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            G = panel1.CreateGraphics();
            kTextBox.Text = "3";
        }

        /// <summary>
        /// Update all points
        /// </summary>
        private void solveButton_Click(object sender, EventArgs e)
        {
            G.Clear(BackColor);
            FirstCluster = false;
            UpdateClusters();
            DrawClusters();
        }

        /// <summary>
        /// Add point to cluster
        /// </summary>
        private void panel1_MouseClick(Object sender, MouseEventArgs e)
        {
            Points.Add(new Point(e.X, e.Y));

            UpdateClusters();

            //Draw clusters
            DrawClusters();
        }

        /// <summary>
        /// Draw the clusters to the panel
        /// </summary>
        private void DrawClusters()
        {
            //No clusters return
            if (Clusters == null)
                return;

            //Loop through each cluster
            int colourIndex = 0;
            foreach (var cluster in Clusters)
            {
                //Get the colour defined for the cluster if available otherwise get a random colour
                Pen pen;
                try
                {
                    pen = new Pen(Colors[colourIndex++]);
                }
                catch
                {
                    pen = new Pen(Color.FromArgb(Rand.Next(256), Rand.Next(256), Rand.Next(256)));
                }

                //Draw the points of the cluster
                foreach (var point in cluster)
                    G.DrawRectangle(pen, point.X - 1, point.Y - 1, 3, 3);
                
                pen.Dispose();
            }
        }

        /// <summary>
        /// Updates the clustering of any points
        /// </summary>
        private void UpdateClusters()
        {
            //Get K from textbox
            int k;
            try
            {
                k = int.Parse(kTextBox.Text);
            }
            catch
            {
                return;
            }

            //Less points than clusters - not possible
            if (Points.Count() < k)
                return;

            if (!FirstCluster)
            {
                //KCluster algorithm
                ClusterAlgo = new KCluster(Points);
                Clusters = ClusterAlgo.Solve(k);
                FirstCluster = true;
            }
            else
            {
                //Add the point to the nearest cluster
                Clusters = ClusterAlgo.ClusterNewPoint(Points.Last());
            }
        }

        /// <summary>
        /// Clear data
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            G.Clear(BackColor);
            Points.Clear();
            FirstCluster = false;
            Clusters.Clear();
        }
    }
}
