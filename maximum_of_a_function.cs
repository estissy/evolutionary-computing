// Implement evolutionary algorithm to find maximum value of a function f(x)=x*sin(x)*sin(10x)
// in interval [-2, 2].

// Tournament selection
// One-ponint cross-over
// Mutation: random genotype shifts, left or right, by from 1 to 5 bits.

using System;

class Program {

    public static void Main(string[] arguments) {
        Random randomGenerator = new Random();

        int populationSize = 20;
        int epochs = 1000;

        // Parsing arguments from program input
        if(arguments.Length == 1) {
            populationSize = Convert.ToInt32(arguments[0]);
        }

        if (arguments.Length == 2) {
            epochs = Convert.ToInt32(arguments[1]);
        }

        uint[] population = getRandomPopulation(populationSize, randomGenerator);

        for (int i = 0; i < epochs; i++) {
          uint[] offspring = new uint[population.Length];

          for (int j = 0; j < population.Length-1; j++) {
            int firstParentIndex = getRandomParentIndex(population, randomGenerator);
            int secondParentIndex = getRandomParentIndex(population, randomGenerator);

            uint firstParent = population[firstParentIndex];
            uint secondParent = population[secondParentIndex];
            uint[] newIndividuals = getNewIndividuals(firstParent, secondParent, randomGenerator);


            offspring[j] = getMutatedIndividual(newIndividuals[0], 0.05, randomGenerator);
            offspring[j + 1] = getMutatedIndividual(newIndividuals[1], 0.05, randomGenerator);
          }

          population = offspring;
        }

        // Avg fitness
        double sum = 0;
        foreach(uint inidividual in population) { sum = sum + getFitness(getPhenotype(inidividual)); }
        Console.WriteLine(String.Format("Average value in population: {0}", sum / population.Length) );

        // Max fitness
        double maxFitness = 0;
        double bestPhenotype = 0;

        foreach(uint inidividual in population) {
          if (getFitness(getPhenotype(inidividual)) > maxFitness) {
            maxFitness = getFitness(getPhenotype(inidividual));
            bestPhenotype = getPhenotype(inidividual);
          }
        }

        Console.WriteLine(String.Format("Max value in population: {0}, for {1}", maxFitness, bestPhenotype));
    }

    public static double getPhenotype(uint genotype) {
      return -2.0 + genotype / (double) uint.MaxValue * 4.0;
    }

    public static double getFitness(double phenotype) {
        return phenotype * Math.Sin(phenotype) * Math.Sin(10 * phenotype);
    }

    public static uint[] getRandomPopulation(int populationSize, Random randomGenerator) {
        uint[] result = new uint[populationSize];

        for (int i = 0; i < populationSize; i++) {

            // Generating random uint value: https://stackoverflow.com/a/17080161
            uint thirtyBits = (uint) randomGenerator.Next(1 << 30);
            uint twoBits = (uint) randomGenerator.Next(1 << 2);
            uint fullRange = (thirtyBits << 2) | twoBits;

            result[i] = fullRange;
        }

        return result;
    }

    public static int getRandomParentIndex(uint[] population, Random randomGenerator) {
      int populationLength = population.Length;

      int firstRandomIndex = randomGenerator.Next(populationLength);
      int secondRandomIndex = randomGenerator.Next(populationLength);

      uint firstIndividual = population[firstRandomIndex];
      uint secondIndividual = population[secondRandomIndex];

      double firstIndividualPhenotype = getPhenotype(firstIndividual);
      double secondIndividualPhenotype = getPhenotype(secondIndividual);

      double firstIndividualFitness = getFitness(firstIndividualPhenotype);
      double secondIndividualFitness = getFitness(secondIndividualPhenotype);

      if (firstIndividualFitness > secondIndividualFitness) {
        return firstRandomIndex;
      } else {
        return secondRandomIndex;
      }
    }

    public static uint[] getNewIndividuals(uint firstIndividual, uint secondIndividual, Random randomGenerator) {

      // Random integer from 1 to 31, determines split point
      int splitPoint = randomGenerator.Next(31) + 1;

      uint mask = uint.MaxValue;
      mask = mask << 32 - splitPoint;

      uint negativeMask = ~ mask;

      uint firstIndividulaMasked = firstIndividual & mask;
      uint firstIndividualNegativlyMasked = firstIndividual & negativeMask;

      uint secondIndividualMasked = secondIndividual & mask;
      uint secondIndividualNegativlyMasked = secondIndividual & negativeMask;

      uint firstNewIndividual = firstIndividulaMasked | secondIndividualNegativlyMasked;
      uint secondNewIndividual = secondIndividualMasked | firstIndividualNegativlyMasked;

      uint[] results = new uint[2];
      results[0] = firstNewIndividual;
      results[1] = secondNewIndividual;

      return results;
    }

    public static uint getMutatedIndividual(uint individual, double mutationRate, Random randomGenerator) {

        if (randomGenerator.NextDouble() < mutationRate) {
            if (randomGenerator.NextDouble() < 0.5) {

                // Mutate inidividual genotype by shiftig right for random number of bits (1, 5)
                individual = individual >> randomGenerator.Next(5) + 1;
            } else {

                // Mutate inidividual genotype by shiftig left for random number of bits (1, 5)
                individual = individual << randomGenerator.Next(5) + 1;
            }
      }

      return individual;
    }
}
