using System;

namespace ACO.AntColony
{
    public class Pheromone
    {

        private const double MinValue = 0.0001;
        private const double MaxValue = 100000.0;
        private const double DefaultValue = 0.01;

        private string[] routePoints;
        private double[][] pheromoneLevel;

        public Pheromone(string[] routePoints)
        {
            this.routePoints = routePoints;
            InitPheromones();
        }

        private void InitPheromones()
        {
            double[][] pheromones = new double[routePoints.Length][];
            for (int i = 0; i < routePoints.Length; i++)
            {
                pheromones[i] = new double[routePoints.Length];
            }
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    pheromones[i][j] = DefaultValue;
                }
            }
            pheromoneLevel = pheromones;
        }

        public int Size => routePoints.Length;
        

        public double Get(string i, string j)
        {
            return pheromoneLevel[IndexOf(i)][IndexOf(j)];
        }

        public void Set(string i, string j, double value)
        {
            if (value < MinValue)
            {
                pheromoneLevel[IndexOf(i)][IndexOf(j)] = MinValue;
            }
            else if (value > MaxValue)
            {
                pheromoneLevel[IndexOf(i)][IndexOf(j)] = MaxValue;
            }
            else
            {
                pheromoneLevel[IndexOf(i)][IndexOf(j)] = value;
            }

            pheromoneLevel[IndexOf(j)][IndexOf(i)] = pheromoneLevel[IndexOf(i)][IndexOf(j)];

        }

        private int IndexOf(string target)
        {
            // helper for RandomTrail
            for (int i = 0; i <= routePoints.Length - 1; i++)
            {
                if (routePoints[i] == target)
                {
                    return i;
                }
            }
            throw new Exception("Target not found in IndexOfTarget");
        }

    }
}
