using System;
using System.Collections.Generic;
using System.Linq;
using ACO.AntColony;

namespace ACO
{
    internal static class Program
    {

        private static Random random = new Random(0);
        // influence of pheromone on direction
        private static int alpha = 3;
        // influence of adjacent node distance
        private static int beta = 2;

        // pheromone decrease factor
        private static double rho = 0.01;
        // pheromone increase factor
        private static double Q = 2.0;

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("\nBegin Ant Colony Optimization demo\n");

                int numCities = 25;
                int numAnts = 3;
                int maxTime = 1000;

                Console.WriteLine("Number cities in problem = " + numCities);

                Console.WriteLine("\nNumber ants = " + numAnts);
                Console.WriteLine("Maximum time = " + maxTime);

                Console.WriteLine("\nAlpha (pheromone influence) = " + alpha);
                Console.WriteLine("Beta (local node influence) = " + beta);
                Console.WriteLine("Rho (pheromone evaporation coefficient) = " + rho.ToString("F2"));
                Console.WriteLine("Q (pheromone deposit factor) = " + Q.ToString("F2"));

                Console.WriteLine("\nInitialing dummy graph distances");
                IList<RouteDistance> dists = MakeGraphDistances(numCities);
                string[] routePoints = MakeRoutePoints(numCities);
                
                AntColonyOptimisation antColonyOptimisation = new AntColonyOptimisation(alpha, beta, rho, Q, numAnts, maxTime);

                Console.WriteLine("\nBegin Ant Colony Optimization");

                string[] bestTrail = antColonyOptimisation.GetBestTrail(dists, routePoints);

                Console.WriteLine("\nTime complete");

                Console.WriteLine("\nBest trail found:");
                Display(bestTrail);

                double bestLength = Length(bestTrail, dists);

                Console.WriteLine("\nLength of best trail found: " + bestLength.ToString("F1"));

                string startPoint = routePoints[15];

                Console.WriteLine("\nBegin Ant Colony Optimization with fixed start point {0}", startPoint);

                bestTrail = antColonyOptimisation.GetBestTrail(dists, routePoints, startPoint);

                Console.WriteLine("\nTime complete");

                Console.WriteLine("\nBest trail found:");
                Display(bestTrail);

                bestLength = Length(bestTrail, dists);

                Console.WriteLine("\nLength of best trail found: " + bestLength.ToString("F1"));

                string endPoint = routePoints[3];

                Console.WriteLine("\nBegin Ant Colony Optimization with fixed start point {0} and fixed end point {1}", startPoint, endPoint);

                bestTrail = antColonyOptimisation.GetBestTrail(dists, routePoints, startPoint, endPoint);

                Console.WriteLine("\nTime complete");

                Console.WriteLine("\nBest trail found:");
                Display(bestTrail);

                bestLength = Length(bestTrail, dists);

                Console.WriteLine("\nLength of best trail found: " + bestLength.ToString("F1"));

                Console.WriteLine("\nEnd Ant Colony Optimization demo\n");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

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
        
        private static IList<RouteDistance> MakeGraphDistances(int numCities)
        {
            IList<RouteDistance> dists = new List<RouteDistance>(numCities * numCities);
            for (int i = 0; i <= numCities - 1; i++)
            {
                for (int j = i + 1; j <= numCities - 1; j++)
                {
                    double d = random.Next(1, 9);
                    // [1,8]
                    dists.Add(new RouteDistance(GetCityName(i), GetCityName(j), d));
                    dists.Add(new RouteDistance(GetCityName(j), GetCityName(i), d));
                }
            }
            return dists;
        }

        private static string[] MakeRoutePoints(int numCities)
        {
            var result = new string[numCities];
            for (int i = 0; i < numCities; i++)
            {
                result[i] = GetCityName(i);
            }
            return result;
        }

        private static string GetCityName(int i)
        {
            return "City" + i;
        }

        private static double Distance(string cityX, string cityY, IList<RouteDistance> dists)
        {
            return dists.First(x => x.FirstPoint == cityX && x.SecondPoint == cityY).Distance;
        }

        private static void Display(string[] trail)
        {
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                Console.Write(trail[i] + " ");
                if (i > 0 && i % 20 == 0)
                {
                    Console.WriteLine("");
                }
            }
            Console.WriteLine("");
        }

    }
}
