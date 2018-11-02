using System.Collections.Generic;
using System;

public class GeneticAlgorithmPL : ParameterLearning {

	private List<List<float>> population; // list of parameters
	private float mutationProbability; // probability of mutation
	private int populationSize;
	private int current; //current member of population returned for getNextFunction

	public GeneticAlgorithmPL (List<float> initialSolution){
		RandomGenerator = new System.Random ();
		fitness= new Dictionary<List<float>, float>();
		populationSize = 12;
		mutationProbability = 0.3f;
		current = 0;
		population = generateSuccessors (populationSize, initialSolution);
	}

	private void createNewGeneration (){
		List<List<float>> newGeneration = selection (population, populationSize ); 
		var newGenerationSize = newGeneration.Count;
		for (int i = 0; i < newGenerationSize; i++) {
			var children = crossover (newGeneration [i], newGeneration [newGeneration.Count - 1 - i]);
			newGeneration.Add(children[0]);
			newGeneration.Add(children[1]);
		}
		mutation (newGeneration);
		population = newGeneration;
	}


	private List<List<float>> generateSuccessors(int K, List<float> parent){
		List<List<float>> successors = new List<List<float>>();
		for (int i = 0; i < K; i++) {
			var successor = randomChild (parent);
			successors.Add (successor);
		}
		return successors;
	}

	private List<float> randomChild(List<float> parent){
		List<float> child = new List<float>();
		for (int i = 0; i < parent.Count; i++) {
			child.Add(parent [i] + RandomGenerator.Next()%20-10);	//We add or divide values from 0 to 10
		}
		return child;
	}

	private List<List<float>> selection(List<List<float>> population, int K){ // selecting K individuals for next generation
		List<List<float>> newGeneration = new List<List<float>>();
		SortPopulation ();
		for(int i=0;i<population.Count/3;i++){
			var currentIndividual = population[i];
			newGeneration.Add (currentIndividual);
			if (newGeneration.Count >= K) //limiting amount of members of new gen
				break;
		}
		return newGeneration;
	}

	private List<List<float>> mutation(List<List<float>> population){ // ES(1+1)
		for (int i = 0; i < population.Count; i++) {
			if ( mutationProbability > RandomGenerator.NextDouble()) {
				var mutatedGene=(int)(population[i].Count*RandomGenerator.NextDouble());
				var unmutated = population[i];
				population[i][mutatedGene] = population[i][mutatedGene]+(float)RandomGenerator.NextDouble()*2-1;
				if (GetFitness (population[i]) <= GetFitness (unmutated)) // if mutated individual is not better then original we keep the original
					population[i] = unmutated;
			}
		}
		return population;
	}

	private List<float>[] crossover(List<float> parent1, List<float> parent2 ){
		List<float>[] children = new List<float>[2];
		int crossingPoint = (int)((parent1.Count-1) * RandomGenerator.NextDouble())+1; // value between 1 and parameter size 
		List<float> child1 = new List<float>();
		List<float> child2 = new List<float>();
		for (int i = 0; i < parent1.Count; i++) {
			if (i < crossingPoint) {
				child1.Add (parent1 [i]);
				child2.Add (parent2 [i]);
			} else {
				child1.Add (parent2 [i]);
				child2.Add (parent1 [i]);
			}
		}
		children [0] = child1;
		children [1] = child2;
		return children;
	}

	public override List<float> GetNext(){
		if (current >= population.Count) {
			createNewGeneration ();
			current = 0;
			Console.WriteLine ();
		}
		return population [++current-1];

	}
	private void SortPopulation(){
		population.Sort((x, y) => GetFitness(y).CompareTo(GetFitness(x)));
	}
}
