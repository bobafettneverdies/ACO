namespace ACO.AntColony
{
    class Pheromone
    {

        private const double MinValue = 0.0001;
        private const double MaxValue = 100000.0;
        private const double DefaultValue = 0.01;

        private double[][] pheromoneLevel;

        public Pheromone(int numCities)
        {
            pheromoneLevel = InitPheromones(numCities);
        }

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
                    pheromones[i][j] = DefaultValue;
                }
            }
            return pheromones;
        }

        public int Size => pheromoneLevel.Length;

        public double Get(int i, int j)
        {
            return pheromoneLevel[i][j];
        }

        public void Set(int i, int j, double value)
        {
            if (value < MinValue)
            {
                pheromoneLevel[i][j] = MinValue;
            }
            else if (value > MaxValue)
            {
                pheromoneLevel[i][j] = MaxValue;
            }
            else
            {
                pheromoneLevel[i][j] = value;
            }

            pheromoneLevel[j][i] = pheromoneLevel[i][j];

        } 

    }
}
