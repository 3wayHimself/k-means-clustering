using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClusteringGUI
{
    /// <summary>
    /// Represents the distance between 2 vertices
    /// </summary>
    class Edge
    {
        /// <summary>
        /// The ID of a vertex
        /// </summary>
        public int StartVertex { get; set; }

        /// <summary>
        /// The ID of a vertex
        /// </summary>
        public int EndVertex { get; set; }

        /// <summary>
        /// The distance between the vertices
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">The ID of a vertex</param>
        /// <param name="e">The ID of a vertex</param>
        /// <param name="d">The distance between the vertices</param>
        public Edge(int s, int e, int d)
        {
            StartVertex = s;
            EndVertex = e;
            Distance = d;
        }
    }
}
