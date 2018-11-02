using System.Collections.Generic;
using System;

public class SimulatedAnnealingPL : ParameterLearning {

	private float temperature;

	public SimulatedAnnealingPL(List<float> initialParameters){
		parameters = initialParameters;
		RandomGenerator = new System.Random ();
		fitness = new Dictionary<List<float>, float>();
		temperature = 1000.0f;
	}

	public override List<float> GetNext ()
	{
		learn (temperature*0.7f); // in every loop we run 1000 simulations
		return parameters;
	}

	public void learn(float temp){
		if (temp > 0) {
			List<float> newParameters = randomChild (parameters);
			var deltaE = GetFitness (newParameters) - GetFitness (parameters);
			if (deltaE >= 0)
				parameters = newParameters;
			else if (RandomGenerator.NextDouble () < Math.Pow (Math.E, deltaE / temp))
				parameters = newParameters;
		}
	}

	private List<float> randomChild(List<float> parent){
		List<float> child = new List<float>();
		for (int i = 0; i < parent.Count; i++) {
			child.Add(parent [i] + (float)RandomGenerator.NextDouble()*2-1);	//To be changed, so that values in random children make sense
		}
		return child;
	}
}