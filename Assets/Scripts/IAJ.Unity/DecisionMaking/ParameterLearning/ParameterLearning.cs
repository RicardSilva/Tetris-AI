using System.Collections.Generic;
using System;
using System.Linq;

public abstract class ParameterLearning {

	public List<float> parameters;
    //public List<List<float>> fitness;
    public Dictionary<List<float>, float> fitness;
    protected System.Random RandomGenerator { get; set; }

	public abstract List<float> GetNext ();
    public void setFitness(List<float> S, float score) {
        float val;
        if (fitness.TryGetValue(S, out val))
        {
            // yay, value exists!
           if(score > fitness[S]) fitness[S] = score;
        }
        else
        {
            // darn, lets add the value
            fitness.Add(S, score);
        }
        
    }
	public virtual float GetFitness (List<float> parameters){
        float score;
        fitness.TryGetValue(parameters, out score);
        return score;

    }
	public virtual List<float> GetBest(){
      return fitness.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    
	}

}
