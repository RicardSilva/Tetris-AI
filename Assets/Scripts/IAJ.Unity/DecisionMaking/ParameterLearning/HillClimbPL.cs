using System.Collections.Generic;

public class HillClimbPL : ParameterLearning {

	private double currentParameter { get; set; }
	private List<float> oldParameters;

	public HillClimbPL(List<float> initialParameters){
        fitness = new Dictionary<List<float>, float>();
        RandomGenerator = new System.Random ();
		parameters = initialParameters;
		oldParameters = initialParameters;
	}

	public override List<float> GetNext(){

        parameters = randomChild(oldParameters);

        if (GetFitness (parameters) >= GetFitness (oldParameters))
			oldParameters = parameters;
        

		return oldParameters;
	}


    private List<float> randomChild(List<float> parent)
    {
        List<float> child = new List<float>();
        var parameterCount = parent.Count;
        for (int j = 0; j < parameterCount; j++)
        {          
            child.Add(parent[j] + (float)RandomGenerator.NextDouble() * 2 - 1);  //To be changed, so that values in random children make sense
        }
        return child;
    }
    private List<float> randomHigherChild(List<float> parent)
    {
        List<float> child = new List<float>();
        var parameterCount = parent.Count;
        var randomParameter = RandomGenerator.Next() % parameterCount;
        for (int j = 0; j < parameterCount; j++)
        {
            if (j == randomParameter)
                child.Add(parent[j] + 1f);  //To be changed, so that values in random children make sense
            else
                child.Add(parent[j]);
        }
        return child;
    }

    private List<float> randomLowerChild(List<float> parent)
    {
        List<float> child = new List<float>();
        var parameterCount = parent.Count;
        var randomParameter = RandomGenerator.Next() % parameterCount;
        for (int j = 0; j < parameterCount; j++)
        {
            if (j == randomParameter)
                child.Add(parent[j] - 1f);   
            else
                child.Add(parent[j]);
        }
        return child;
    }



}
