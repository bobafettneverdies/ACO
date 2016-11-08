using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACO.AntColony
{
    public class RouteDistance
    {

        private string firstPoint;

        private string secondPoint;

        private double distance;

        public RouteDistance(string firstPoint, string secondPoint, double distance)
        {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.distance = distance;
        }

        public string FirstPoint => firstPoint;

        public string SecondPoint => secondPoint;

        public double Distance => distance;
    }
}
