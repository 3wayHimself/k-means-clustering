using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ClusteringGUI
{
    /// <summary>
    /// Represents the algorithm to solve the k-means clustering
    /// </summary>
    class KCluster
    {
        /// <summary>
        /// The initial number of clusters in the graph
        /// </summary>
        protected int NumberOfClusters;

        /// <summary>
        /// The edges of the graph
        /// </summary>
        protected List<Edge> Edges = new List<Edge>();

        /// <summary>
        /// The clusters created - set of clusters which are a set of IDs
        /// </summary>
        protected List<List<int>> Clusters = new List<List<int>>();

        /// <summary>
        /// Maps the points the user inputs from the vertex IDs
        /// </summary>
        protected Dictionary<int, Point> PointMap = new Dictionary<int, Point>();

        /// <summary>
        /// Constructor - enters a list of Points into the class
        /// </summary>
        /// <param name="points">The points the user has entered</param>
        public KCluster(List<Point> points)
        {
            //For each point
            for (int p = 0; p < points.Count() - 1; p++)
            {
                //Add to point map
                PointMap.Add(p, points[p]);

                //For other pairs
                for (int q = p + 1; q < points.Count(); q++)
                {
                    //Add edge into class
                    int distance = GetDistanceSquared(points[p], points[q]);
                    Edges.Add(new Edge(p, q, distance));
                }
            }

            //Add last point
            PointMap.Add(points.Count() - 1, points.Last());

            //Set cluster number
            NumberOfClusters = points.Count();
        }

        /// <summary>
        /// Gets the manhattan distance squared between two points
        /// </summary>
        /// <param name="p">The start point</param>
        /// <param name="q">The end point</param>
        /// <returns>The distance squared</returns>
        private int GetDistanceSquared(Point p, Point q)
        {
            return (p.Y - q.Y)*(p.Y - q.Y) + (p.X - q.X)*(p.X - q.X);
        }

        /// <summary>
        /// Gets the edge which links the two vertices together
        /// </summary>
        /// <param name="a">The ID of a vertex</param>
        /// <param name="b">The ID of a vertex</param>
        /// <returns>The edge</returns>
        private Edge GetEdgeBetween(int a, int b)
        {
            return Edges.Where(e => (e.StartVertex == a && e.EndVertex == b) || (e.StartVertex == b && e.EndVertex == a)).First();
        }

        /// <summary>
        /// Merges two clusters together
        /// </summary>
        /// <param name="a">The first cluster</param>
        /// <param name="b">The second cluster</param>
        private void Merge(List<int> a, List<int> b)
        {
            a.AddRange(b);
            Clusters.Remove(b);
        }

        /// <summary>
        /// Gets the cluster which contains a vertex
        /// </summary>
        /// <param name="id">The ID of the vertex</param>
        /// <returns>The cluster</returns>
        private List<int> GetClusterOf(int id)
        {
            for (int i = 0; i < Clusters.Count(); i++)
                if (Clusters[i].Contains(id))
                    return Clusters[i];

            return null;
        }

        /// <summary>
        /// Adds a input point to the nearest cluster
        /// </summary>
        /// <param name="p">The point to add</param>
        /// <returns>The clusters as points</returns>
        public List<List<Point>> ClusterNewPoint(Point p)
        {
            //Add point to nearest cluster

            //Keep track of the cluster which is closest to the point to add
            int minDist = 999999;
            int minDistClusterIndex = -1;

            int currentClusterIndex = -1;
            foreach (var cluster in Clusters)
            {
                currentClusterIndex++;
                foreach (var point in cluster)
                {
                    int distance = GetDistanceSquared(PointMap[point], p);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minDistClusterIndex = currentClusterIndex;
                    }
                }
            }

            Clusters[minDistClusterIndex].Add(PointMap.Count);
            PointMap.Add(PointMap.Count, p);

            return ConvertClusters();
        }

        /// <summary>
        /// Converts the clusters in the int format into the Point format
        /// </summary>
        /// <returns>Formatted clusters</returns>
        private List<List<Point>> ConvertClusters()
        {
            var clusterPoints = new List<List<Point>>();
            foreach (var cluster in Clusters)
            {
                clusterPoints.Add(new List<Point>());
                foreach (var id in cluster)
                    clusterPoints.Last().Add(PointMap[id]);
            }

            return clusterPoints;
        }

        /// <summary>
        /// K-means clutsering algorithm
        /// </summary>
        /// <param name="k">The number of clusters</param>
        /// <returns>The clusters</returns>
        public List<List<Point>> Solve(int k)
        {
            //Add each point to its own clusters
            Clusters = new List<List<int>>();
            for (int i = 0; i < NumberOfClusters; i++)
            {
                Clusters.Add(new List<int>());
                Clusters[i].Add(i);
            }

            //While the clusters aren't k amount and there are edges left
            while (Clusters.Count() != k && Edges.Any())
            {
                //Find the edge with the minomum weight
                int minEdgeWeight = 99999999;
                int minEdgeIndex = -1;
                for (int i = 0; i < Edges.Count(); i++)
                {
                    if (Edges[i].Distance < minEdgeWeight)
                    {
                        minEdgeIndex = i;
                        minEdgeWeight = Edges[i].Distance;
                    }

                }
                var minEdge = Edges[minEdgeIndex];

                //If both in same cluster, ignore
                var c = GetClusterOf(minEdge.StartVertex);
                var b = GetClusterOf(minEdge.EndVertex);
                if (b == c)
                {
                    Edges.Remove(minEdge);
                }
                //If both in different cluster, merge
                else
                {
                    Merge(b, c);
                    Edges.Remove(minEdge);
                }
            }

            return ConvertClusters();   
        }
    }
}
