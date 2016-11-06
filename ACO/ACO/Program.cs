using System;

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
                int[][] dists = MakeGraphDistances(numCities);
                
                AntColonyOptimisation antColonyOptimisation = new AntColonyOptimisation(alpha, beta, rho, Q, numAnts, maxTime);

                int[] bestTrail = antColonyOptimisation.GetBestTrail(dists, numCities);

                Console.WriteLine("\nTime complete");

                Console.WriteLine("\nBest trail found:");
                Display(bestTrail);

                double bestLength = Length(bestTrail, dists);

                Console.WriteLine("\nLength of best trail found: " + bestLength.ToString("F1"));

                int startPoint = 15;

                Console.WriteLine("\nBegin Ant Colony Optimization with fixed start point {0}\n", startPoint);

                bestTrail = antColonyOptimisation.GetBestTrail(dists, numCities, 15);

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

        private static double Length(int[] trail, int[][] dists)
        {
            // total length of a trail
            double result = 0.0;
            for (int i = 0; i <= trail.Length - 2; i++)
            {
                result += Distance(trail[i], trail[i + 1], dists);
            }
            return result;
        }
        
        private static int[][] MakeGraphDistances(int numCities)
        {
            int[][] dists = new int[numCities][];
            for (int i = 0; i <= dists.Length - 1; i++)
            {
                dists[i] = new int[numCities];
            }
            for (int i = 0; i <= numCities - 1; i++)
            {
                for (int j = i + 1; j <= numCities - 1; j++)
                {
                    int d = random.Next(1, 9);
                    // [1,8]
                    dists[i][j] = d;
                    dists[j][i] = d;
                }
            }
            return dists;
        }

        private static double Distance(int cityX, int cityY, int[][] dists)
        {
            return dists[cityX][cityY];
        }

        private static void Display(int[] trail)
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
