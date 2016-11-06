using System;

namespace ACO
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
        private int startPoint;

        private bool isEndPointFixed;
        private int endPoint;

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

        public int[] GetBestTrail(int[][] dists, int numCities)
        {
            isStartPointFixed = false;
            isEndPointFixed = false;

            return FindBestTrail(dists, numCities);
        }

        public int[] GetBestTrail(int[][] dists, int numCities, int startPoint)
        {
            isStartPointFixed = true;
            isEndPointFixed = false;

            this.startPoint = startPoint;

            return FindBestTrail(dists, numCities);
        }

        public int[] GetBestTrail(int[][] dists, int numCities, int startPoint, int endPoint)
        {
            isStartPointFixed = true;
            isEndPointFixed = true;

            this.startPoint = startPoint;
            this.endPoint = endPoint;

            return FindBestTrail(dists, numCities);
        }

        private int[] FindBestTrail(int[][] dists, int numCities)
        {
            int[][] ants = InitAnts(numCities);
            // initialize ants to random trails

            int[] bestTrail = BestTrail(ants, dists);
            // determine the best initial trail
            double bestLength = Length(bestTrail, dists);
            // the length of the best trail

            double[][] pheromones = InitPheromones(numCities);

            int time = 0;
            while (time < maxTime)
            {
                UpdateAnts(ants, pheromones, dists);
                UpdatePheromones(pheromones, ants, dists);

                int[] currBestTrail = BestTrail(ants, dists);
                double currBestLength = Length(currBestTrail, dists);
                if (currBestLength < bestLength)
                {
                    bestLength = currBestLength;
                    bestTrail = currBestTrail;
                    //Console.WriteLine("New best length of " + bestLength.ToString("F1") + " found at time " + time);
                }
                time += 1;
            }
            return bestTrail;
        }

        private int[][] InitAnts(int numCities)
        {
            int[][] ants = new int[numAnts][];
            for (int k = 0; k <= numAnts - 1; k++)
            {
                int start = isStartPointFixed ? startPoint : random.Next(0, numCities);
                ants[k] = isEndPointFixed ? RandomTrail(start, endPoint, numCities) : RandomTrail(start, numCities);
            }
            return ants;
        }

        private int[] RandomTrail(int start, int end, int numCities)
        {
            int[] trail = RandomTrail(start, numCities);

            int idx = IndexOfTarget(trail, end);
            int temp = trail[numCities - 1];
            trail[numCities - 1] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private int[] RandomTrail(int start, int numCities)
        {
            // helper for InitAnts
            int[] trail = new int[numCities];

            // sequential
            for (int i = 0; i <= numCities - 1; i++)
            {
                trail[i] = i;
            }

            // Fisher-Yates shuffle
            for (int i = 0; i <= numCities - 1; i++)
            {
                int r = random.Next(i, numCities);
                int tmp = trail[r];
                trail[r] = trail[i];
                trail[i] = tmp;
            }

            int idx = IndexOfTarget(trail, start);
            // put start at [0]
            int temp = trail[0];
            trail[0] = trail[idx];
            trail[idx] = temp;

            return trail;
        }

        private int IndexOfTarget(int[] trail, int target)
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

        private double Length(int[] trail, int[][] dists)
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

        private int[] BestTrail(int[][] ants, int[][] dists)
        {
            // best trail has shortest total length
            double bestLength = Length(ants[0], dists);
            int idxBestLength = 0;
            for (int k = 1; k <= ants.Length - 1; k++)
            {
                double len = Length(ants[k], dists);
                if (len < bestLength)
                {
                    bestLength = len;
                    idxBestLength = k;
                }
            }
            int numCities = ants[0].Length;
            int[] bestTrail = new int[numCities];
            ants[idxBestLength].CopyTo(bestTrail, 0);
            return bestTrail;
        }

        // --------------------------------------------------------------------------------------------

        private double[][] InitPheromones(int numCities)
        {
            double[][] pheromones = new double[numCities][];
            for (int i = 0; i <= numCities - 1; i++)
            {
                pheromones[i] = new double[numCities];
            }
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    pheromones[i][j] = 0.01;
                    // otherwise first call to UpdateAnts -> BuildTrail -> NextNode -> MoveProbs => all 0.0 => throws
                }
            }
            return pheromones;
        }

        // --------------------------------------------------------------------------------------------

        private void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists)
        {
            int numCities = pheromones.Length;
            for (int k = 0; k <= ants.Length - 1; k++)
            {
                int start = isStartPointFixed ? startPoint : random.Next(0, numCities);
                int[] newTrail = isEndPointFixed ? BuildTrail(start, endPoint, pheromones, dists) : BuildTrail(start, pheromones, dists);
                ants[k] = newTrail;
            }
        }

        private int[] BuildTrail(int start, int end, double[][] pheromones, int[][] dists)
        {
            int numCities = pheromones.Length;
            int[] trail = new int[numCities];
            bool[] visited = new bool[numCities];
            trail[0] = start;
            visited[start] = true;
            trail[numCities - 1] = end;
            visited[end] = true;
            for (int i = 0; i <= numCities - 3; i++)
            {
                int cityX = trail[i];
                int next = NextCity(cityX, visited, pheromones, dists);
                trail[i + 1] = next;
                visited[next] = true;
            }
            return trail;
        }

        private int[] BuildTrail(int start, double[][] pheromones, int[][] dists)
        {
            int numCities = pheromones.Length;
            int[] trail = new int[numCities];
            bool[] visited = new bool[numCities];
            trail[0] = start;
            visited[start] = true;
            for (int i = 0; i <= numCities - 2; i++)
            {
                int cityX = trail[i];
                int next = NextCity(cityX, visited, pheromones, dists);
                trail[i + 1] = next;
                visited[next] = true;
            }
            return trail;
        }

        private int NextCity(int cityX, bool[] visited, double[][] pheromones, int[][] dists)
        {
            // for ant (with visited[]), at nodeX, what is next node in trail?
            double[] probs = MoveProbs(cityX, visited, pheromones, dists);

            double[] cumul = new double[probs.Length + 1];
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
                    return i;
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private double[] MoveProbs(int cityX, bool[] visited, double[][] pheromones, int[][] dists)
        {
            // for ant, located at nodeX, with visited[], return the prob of moving to each city
            int numCities = pheromones.Length;
            double[] taueta = new double[numCities];
            // inclues cityX and visited cities
            double sum = 0.0;
            // sum of all tauetas
            // i is the adjacent city
            for (int i = 0; i <= taueta.Length - 1; i++)
            {
                if (i == cityX)
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
                    taueta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / Distance(cityX, i, dists)), beta);
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

        private void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists)
        {
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = i + 1; j <= pheromones[i].Length - 1; j++)
                {
                    for (int k = 0; k <= ants.Length - 1; k++)
                    {
                        double length = Length(ants[k], dists);
                        // length of ant k trail
                        double decrease = (1.0 - rho) * pheromones[i][j];
                        double increase = 0.0;
                        if (EdgeInTrail(i, j, ants[k]))
                        {
                            increase = (Q / length);
                        }

                        pheromones[i][j] = decrease + increase;

                        if (pheromones[i][j] < 0.0001)
                        {
                            pheromones[i][j] = 0.0001;
                        }
                        else if (pheromones[i][j] > 100000.0)
                        {
                            pheromones[i][j] = 100000.0;
                        }

                        pheromones[j][i] = pheromones[i][j];
                    }
                }
            }
        }

        private bool EdgeInTrail(int cityX, int cityY, int[] trail)
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
        
        private double Distance(int cityX, int cityY, int[][] dists)
        {
            return dists[cityX][cityY];
        }

    }
}
