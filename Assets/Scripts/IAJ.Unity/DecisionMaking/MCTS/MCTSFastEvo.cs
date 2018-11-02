using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
	public class MCTSFastEvo : MCTSBiasedPlayout
    {

		public List<float> BestK;
        public ParameterLearning evo;
		public MCTSFastEvo(State worldModel): base(worldModel)
		{
			BestK = new List<float>{1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
            //this.evo = new GeneticAlgorithmPL(BestK);
            this.evo = new HillClimbPL(BestK);
            //this.evo = new SimulatedAnnealingPL(BestK);

        }

        public override void InitializeMCTSearch()
		{
			base.InitializeMCTSearch();
		}

		public List<float> Learn()
		{
			MCTSNode selectedNode;
			Reward reward;

			var startTime = Time.realtimeSinceStartup;
			this.CurrentIterationsInFrame = 0;

			List<float> k; // will use w as list of K values
                          
            float averageScore = 0;
            while (Time.realtimeSinceStartup - startTime < this.MaxTimePerFrame)
            {
                k = evo.GetNext ();
                averageScore = 0;
                for (int i=0;i<10;i++){
					//CurrentStateWorldModel.weights = k;
                    selectedNode = Selection (this.InitialNode);
                    reward = Playout (selectedNode.State, k);
					Backpropagate (selectedNode, reward);

                    averageScore += reward.Value;
				}
				evo.setFitness(k, averageScore / 10);
			}

            List<float> best = evo.GetBest();
            Debug.Log("Score:" + evo.GetFitness(best) + " Weights" + best[0] + " " + best[1] + " " + best[2] + " " + best[3] + " " + best[4]);
            
			return best;

			
		}


	}
}