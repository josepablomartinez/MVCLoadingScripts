using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Canvas;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal class MarkerManager
    {
        public MarkerManager(RectangleShape currentExtent, double width, double height, double distance, Collection<Marker> markers)
        {
            this.Width = width;
            this.Height = height;
            this.CurrentExtent = currentExtent;
            this.Markers = markers;
            this.Distance = distance;
        }

        public double Distance
        {
            get;
            set;
        }

        public Collection<Marker> Markers
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public RectangleShape CurrentExtent
        {
            get;
            set;
        }

       
        public List<int> Cluster()
        {
            List<int> clusteredIndexes = new List<int>() { 0 };
            for (int i = 0; i < Markers.Count; i++)
            {
                PointShape position = Markers[i].Position;
                bool shouldAdded = false;
                foreach (int rowIndex in clusteredIndexes)
                {
                    if (ShouldCluster(position, Markers[rowIndex]))
                    {
                        shouldAdded = true;
                        break;
                    }
                }
                if (!shouldAdded)
                {
                    clusteredIndexes.Add(i);
                }
            }

            return clusteredIndexes;
        }

        public bool ShouldCluster(PointShape point, Marker marker)
        {
            double screenDistance = ExtentHelper.GetScreenDistanceBetweenTwoWorldPoints(CurrentExtent, point, marker.Position, (float)Width, (float)Height);
            return (screenDistance <= this.Distance);
        }
    }
}
