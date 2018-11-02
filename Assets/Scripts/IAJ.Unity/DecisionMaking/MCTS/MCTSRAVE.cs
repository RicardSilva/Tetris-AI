using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
/**/
namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSRAVE : MCTS
    {
        protected const float b = 1;
        protected List<Action> ActionHistory { get; set; }
        public MCTSRAVE(State worldModel) : base(worldModel)
        {
            ActionHistory = new List<Action>();
        }

        protected override MCTSNode BestUCTChild(MCTSNode node)
        {
            float MCTSValue;
            float RAVEValue;
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;
            //step 1, calculate beta and 1-beta. beta does not change from child to child. So calculate this only once
            //TODO: implement
            float beta;
            float b = 1;
            beta = node.NRAVE / (node.N + node.NRAVE + 4 * node.N * node.NRAVE * b * b);

            float beta1 = 1 - beta;



            float currentEstimation;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                MCTSValue = (node.ChildNodes[i].Q / node.ChildNodes[i].N);
                var explorationFactor = C * Mathf.Sqrt(Mathf.Log(node.N) / node.ChildNodes[i].N);
                RAVEValue = ((beta1* MCTSValue)+(beta*(node.ChildNodes[i].QRAVE / node.ChildNodes[i].NRAVE))) + explorationFactor;
                UCTValue = MCTSValue + explorationFactor;

                currentEstimation = Math.Max(MCTSValue, RAVEValue);
                currentEstimation = Math.Max(UCTValue,currentEstimation);

                if (currentEstimation > bestUCT)
                {
                    bestUCT = currentEstimation;
                    bestNode = node.ChildNodes[i];
                }
            }
            return bestNode;

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one

        }

        protected override Reward Playout(State initialPlayoutState)
        {
            State currentState = initialPlayoutState.GenerateChildState();

            Action randomAction;
            ActionHistory.Clear();
            int currentDepth = 0;
            while (!currentState.IsTerminal())
            {

                randomAction = currentState.getNextRandomAction(this.RandomGenerator);
                currentState.ApplyAction(randomAction);
                
                ActionHistory.Add(randomAction);
                currentDepth++;

            }
            if (currentDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = currentDepth;
            return new Reward(currentState.score);
        }

        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            int player = 0;
            MCTSNode currentNode = node;
            while (currentNode != null)
            {

                currentNode.N = currentNode.N + 1;
                currentNode.Q = currentNode.Q + currentNode.State.score;

                if (currentNode.Parent != null)
                {
                    ActionHistory.Add(currentNode.Action);
                }

                currentNode = currentNode.Parent;

                if(currentNode != null)
                {
                    foreach (MCTSNode child in currentNode.ChildNodes)
                    {
                        if(ActionHistory.Contains(child.Action))
                        {
                            child.NRAVE = child.NRAVE + 1;
                            child.QRAVE = child.QRAVE + child.State.score;

                        }



                    }


                }
            }
        }
    }
}
/**/