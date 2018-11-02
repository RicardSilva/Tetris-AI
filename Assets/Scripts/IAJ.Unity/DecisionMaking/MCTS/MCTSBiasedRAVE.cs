/**/

using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedRAVE : MCTS
    {
        protected const float b = 1;
        protected List<Action> ActionHistory { get; set; }

        Action lastAction;
        float lastActionValue;

        List<Action> lastestActions;

        public MCTSBiasedRAVE(State WorldModel) : base(WorldModel)
        {
            ActionHistory = new List<Action>();
        }
        public override void InitializeMCTSearch()
        {
            base.InitializeMCTSearch();
            lastAction = null;
            lastActionValue = UnityEngine.Mathf.Infinity;
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
                RAVEValue = ((beta1 * MCTSValue) + (beta * (node.ChildNodes[i].QRAVE / node.ChildNodes[i].NRAVE))) + explorationFactor;
                UCTValue = MCTSValue + explorationFactor;

                currentEstimation = Math.Max(MCTSValue, RAVEValue);
                currentEstimation = Math.Max(UCTValue, currentEstimation);

                if (currentEstimation > bestUCT)
                {
                    bestUCT = currentEstimation;
                    bestNode = node.ChildNodes[i];
                }
            }
            return bestNode;

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one

        }
        protected override MCTSNode Selection(MCTSNode initialNode)
        {
            Action nextAction = initialNode.State.GetNextAction();
            var currentWorld = initialNode.State;
            float currentHValue = 0;
            State copy;

            if (lastAction != null)
            {
                if (nextAction != null)
                {
                    copy = currentWorld.GenerateChildState();
                    copy.ApplyAction(nextAction);
                    currentHValue = copy.GetHValue();
                }
                //vai estabilizando o mcts para valores mais baixos de HValues
                while (nextAction != null && currentHValue > lastActionValue)
                {
                    nextAction = initialNode.State.GetNextAction();
                    if (nextAction != null)
                    {
                        copy = currentWorld.GenerateChildState();
                        copy.ApplyAction(nextAction);
                        currentHValue = copy.GetHValue();
                    }
                }
                //senao houver HValues mais baixos faz reset
                lastAction = nextAction;
                if (lastAction != null)
                {
                    copy = currentWorld.GenerateChildState();
                    copy.ApplyAction(nextAction);
                    lastActionValue = copy.GetHValue();
                }
            }
            else
            {
                lastAction = nextAction;
                if (lastAction != null)
                {
                    copy = currentWorld.GenerateChildState();
                    copy.ApplyAction(nextAction);
                    lastActionValue = copy.GetHValue();
                }
            }


            MCTSNode currentNode = initialNode;
            while (!currentNode.State.IsTerminal())
            {

                if (nextAction != null)
                {
                    return Expand(currentNode, nextAction);
                }
                else
                {
                    currentNode = BestUCTChild(currentNode);
                    nextAction = currentNode.State.GetNextAction();
                }

            }

            return currentNode;
        }
        protected override Reward Playout(State initialPlayoutState)
        {
            State currentState = initialPlayoutState.GenerateChildState();

            Action randomAction;
            ActionHistory.Clear();
            int currentDepth = 0;
            while (!currentState.IsTerminal())
            {

                randomAction = currentState.getNextBiasRandomAction(this.RandomGenerator);
                currentState.ApplyAction(randomAction);

                ActionHistory.Add(randomAction);
                currentDepth++;

            }
            if (currentDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = currentDepth;
            return new Reward(currentState.score);
        }
        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            
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

                if (currentNode != null)
                {
                    foreach (MCTSNode child in currentNode.ChildNodes)
                    {
                        if (ActionHistory.Contains(child.Action))
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
