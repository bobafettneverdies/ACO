using System;
using System.Collections.Generic;
using System.Linq;

namespace ACO.AntColony
{
    public class AntColonyOptimisation
    {

        private Random random;
        private int alpha;
        private int beta;

        private double rho;
        private double Q;

        private int numAnts;

        private int maxTime;

        private bool isStartPointFixed;
        private string startPoint;

        private bool isEndPointFixed;
        private string endPoint;

        public AntColonyOptimisation(int pheromoneImportanceRate, int distanceImportanceRate,
            double pheromoneDecreaseFactor, double pheromoneIncreaseFactor, int numAnts, int maxTime)
        {
            alpha = pheromoneImportanceRate;
            beta = distanceImportanceRate;
            rho = pheromoneDecreaseFactor;
            Q = pheromoneIncreaseFactor;

            this.numAnts = numAnts;
            this.maxTime = maxTime;

            random = new Random(0);
        }

        public string[] GetBestTrail(IList<RouteDistance> dists, string[] routePoints)
        {
            isStartPointFixed = false;
            isEndPointFixed = false;

            return FindBestTrail(dists, routePoints);
        }

        public string[] GetBestTrail(IList<RouteDistance> dists, string[] routePoints, string startPoint)
        {
            isStartPointFixed = true;
            isEndPointFixed = false;

            this.startPoint = startPoint;

            return FindBestTrail(dists, routePoints);
        }

        public string[] GetBestTrail(IList<RouteDistance> dists, string[] routePoints, string startPoint, string endPoint)
        {
            isStartPointFixed = true;
            isEndPointFixed = true;

            this.startPoint = startPoint;
            this.endPoint = endPoint;

            return FindBestTrail(dists, routePoints);
        }

        private string[] FindBestTrail(IList<RouteDistance> dists, string[] routePoints)
        {
            var ants = InitAnts(routePoints);
            // initialize ants to random trails

            string[] bestTrail = BestTrail(ants, dists, routePoints);
            // determine the best initial trail
            double bestLength = Length(bestTrail, dists);
            // the length of the best trail

            Pheromone pheromone = new Pheromone(routePoints);

            int time = 0;
            while (time < maxTime)
            {
                UpdateAnts(ants, pheromone, dists, routePoints);
                UpdatePheromones(pheromone, ants, dists, routePoints);

                string[] currBestTrail = BestTrail(ants, dists, routePoints);
                double currBestLength = Length(currBestTrail, dists);
                if (currBestLength < bestLength)
                {
                    bestLength = currBestLength;
                    bestTrail = currBestTrail;
                    Console.WriteLine("New best length of " + bestLength.ToString("F1") + " found at time " + time);
                }
                time += 1;
            }
            return bestTrail;
        }

        private Ant[] InitAnts(string[] routePoints)
        {
            var ants = new Ant[numAnts];
            for (int k = 0; k <= numAnts - 1; k++)
            {
                string start = isStartPointFixed ? startPoint : routePoints[random.Next(0, routePoints.Length)];
                ants[k] = new Ant(isEndPointFixed ? RandomTrail(start, endPoint, routePoints) : RandomTrail(start, routePoints));
            }
            return ants;
        }

        private string[] RandomTrail(string start, string end, string[] routePoints)
        {
            string[] trail = RandomTrail(start, routePoints);

            int idx = IndexOfTarget(trail, end);
            // put end at [routePoints.Length - 1]
            string temp = trail[routePoints.Length - 1];
            trail[routePoints.Length - 1] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private string[] RandomTrail(string start, string[] routePoints)
        {
            // helper for InitAnts
            string[] trail = new string[routePoints.Length];

            // sequential
            for (int i = 0; i <= routePoints.Length - 1; i++)
            {
                trail[i] = routePoints[i];
            }

            // Fisher-Yates shuffle
            for (int i = 0; i <= routePoints.Length - 1; i++)
            {
                int r = random.Next(i, routePoints.Length);
                string tmp = trail[r];
                trail[r] = trail[i];
                trail[i] = tmp;
            }

            int idx = IndexOfTarget(trail, start);
            // put start at [0]
            string temp = trail[0];
            trail[0] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private static double Length(string[] trail, IList<RouteDistance> dists)
        {
            // total length of a trail
            double result = 0.0;
            for (int i = 0; i <= trail.Length - 2; i++)
            {
                result += Distance(trail[i], trail[i + 1], dists);
            }
            return result;
        }

        // -------------------------------------------------------------------------------------------- 

        private string[] BestTrail(Ant[] ants, IList<RouteDistance> dists, string[] routePoints)
        {
            // best trail has shortest total length
            double bestLength = Length(ants[0].Trail, dists);
            int idxBestLength = 0;
            for (int k = 1; k <= ants.Length - 1; k++)
            {
                double len = Length(ants[k].Trail, dists);
                if (len < bestLength)
                {
                    bestLength = len;
                    idxBestLength = k;
                }
            }
            int numCities = ants[0].Trail.Length;
            string[] bestTrail = new string[numCities];
            ants[idxBestLength].Trail.CopyTo(bestTrail, 0);
            return bestTrail;
        }

        private void UpdateAnts(Ant[] ants, Pheromone pheromone, IList<RouteDistance> dists, string[] routePoints)
        {
            int numCities = pheromone.Size;
            for (int k = 0; k <= ants.Length - 1; k++)
            {
                string start = isStartPointFixed ? startPoint : routePoints[random.Next(0, numCities)];
                var newTrail = isEndPointFixed ? BuildTrail(start, endPoint, pheromone, dists, routePoints) : BuildTrail(start, pheromone, dists, routePoints);
                ants[k].Trail = newTrail;
            }
        }

        private string[] BuildTrail(string start, string end, Pheromone pheromone, IList<RouteDistance> dists, string[] routePoints)
        {
            int numCities = routePoints.Length;
            var trail = new string[numCities];
            var visited = new bool[numCities];
            trail[0] = start;
            visited[IndexOfTarget(routePoints, start)] = true;
            trail[numCities - 1] = end;
            visited[IndexOfTarget(routePoints, end)] = true;
            for (int i = 0; i <= numCities - 3; i++)
            {
                string cityX = trail[i];
                string next = NextCity(cityX, visited, pheromone, dists, routePoints);
                trail[i + 1] = next;
                visited[IndexOfTarget(routePoints, next)] = true;
            }
            return trail;
        }

        private string[] BuildTrail(string start, Pheromone pheromone, IList<RouteDistance> dists, string[] routePoints)
        {
            int numCities = routePoints.Length;
            var trail = new string[numCities];
            var visited = new bool[numCities];
            trail[0] = start;
            visited[IndexOfTarget(routePoints, start)] = true;
            for (int i = 0; i <= numCities - 2; i++)
            {
                string cityX = trail[i];
                string next = NextCity(cityX, visited, pheromone, dists, routePoints);
                trail[i + 1] = next;
                visited[IndexOfTarget(routePoints, next)] = true;
            }
            return trail;
        }

        private string NextCity(string cityX, bool[] visited, Pheromone pheromone, IList<RouteDistance> dists, string[] routePoints)
        {
            // for ant (with visited[]), at nodeX, what is next node in trail?
            var probs = MoveProbs(cityX, visited, pheromone, dists, routePoints);

            var cumul = new double[probs.Length + 1];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
                // consider setting cumul[cuml.Length-1] to 1.00
            }

            double p = random.NextDouble();

            for (int i = 0; i <= cumul.Length - 2; i++)
            {
                if (p >= cumul[i] && p < cumul[i + 1])
                {
                    return routePoints[i];
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private double[] MoveProbs(string cityX, bool[] visited, Pheromone pheromone, IList<RouteDistance> dists, string[] routePoints)
        {
            // for ant, located at nodeX, with visited[], return the prob of moving to each city
            int numCities = pheromone.Size;
            double[] taueta = new double[numCities];
            // inclues cityX and visited cities
            double sum = 0.0;
            // sum of all tauetas
            // i is the adjacent city
            for (int i = 0; i <= taueta.Length - 1; i++)
            {
                if (routePoints[i] == cityX)
                {
                    taueta[i] = 0.0;
                    // prob of moving to self is 0
                }
                else if (visited[i])
                {
                    taueta[i] = 0.0;
                    // prob of moving to a visited city is 0
                }
                else
                {
                    taueta[i] = Math.Pow(pheromone.Get(cityX, routePoints[i]), alpha) * Math.Pow((1.0 / Distance(cityX, routePoints[i], dists)), beta);
                    // could be huge when pheromone[][] is big
                    if (taueta[i] < 0.0001)
                    {
                        taueta[i] = 0.0001;
                    }
                    else if (taueta[i] > (double.MaxValue / (numCities * 100)))
                    {
                        taueta[i] = double.MaxValue / (numCities * 100);
                    }
                }
                sum += taueta[i];
            }

            double[] probs = new double[numCities];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                probs[i] = taueta[i] / sum;
                // big trouble if sum = 0.0
            }
            return probs;
        }

        private void UpdatePheromones(Pheromone pheromone, Ant[] ants, IList<RouteDistance> dists, string[] routePoints)
        {
            for (int i = 0; i <= pheromone.Size - 1; i++)
            {
                for (int j = i + 1; j <= pheromone.Size - 1; j++)
                {
                    for (int k = 0; k <= ants.Length - 1; k++)
                    {
                        double length = Length(ants[k].Trail, dists);
                        // length of ant k trail
                        double decrease = (1.0 - rho) * pheromone.Get(routePoints[i], routePoints[j]);
                        double increase = 0.0;
                        if (EdgeInTrail(routePoints[i], routePoints[j], ants[k].Trail))
                        {
                            increase = (Q / length);
                        }

                        pheromone.Set(routePoints[i], routePoints[j], decrease + increase);
                    }
                }
            }
        }

        private bool EdgeInTrail(string cityX, string cityY, string[] trail)
        {
            // are cityX and cityY adjacent to each other in trail[]?
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if (idx == 0 && trail[1] == cityY)
            {
                return true;
            }

            if (idx == 0 && trail[lastIndex] == cityY)
            {
                return true;
            }

            if (idx == 0)
            {
                return false;
            }

            if (idx == lastIndex && trail[lastIndex - 1] == cityY)
            {
                return true;
            }

            if (idx == lastIndex && trail[0] == cityY)
            {
                return true;
            }

            if (idx == lastIndex)
            {
                return false;
            }

            if (trail[idx - 1] == cityY)
            {
                return true;
            }

            if (trail[idx + 1] == cityY)
            {
                return true;
            }
            
            return false;
        }

        private static double Distance(string cityX, string cityY, IList<RouteDistance> dists)
        {
            return dists.First(x => x.FirstPoint == cityX && x.SecondPoint == cityY).Distance;
        }

        private int IndexOfTarget(string[] trail, string target)
        {
            // helper for RandomTrail
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (trail[i] == target)
                {
                    return i;
                }
            }
            throw new Exception("Target not found in IndexOfTarget");
        }

    }
}
