using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
/**/
namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        Action lastAction;
        float lastActionValue;
        public const bool BiasSelection = true;

        List<Action> lastestActions;

        public MCTSBiasedPlayout(State currentStateWorldModel) : base(currentStateWorldModel)
        {
            
        }
        public override void InitializeMCTSearch()
        {
            base.InitializeMCTSearch();
            lastAction = null;
            lastActionValue = UnityEngine.Mathf.Infinity;
            
        }
        protected override Reward Playout(State initialPlayoutState)
        {
            State currentState = initialPlayoutState.GenerateChildState();
            Action randomAction;
            int currentDepth = 0;
            while (!currentState.IsTerminal())
            {
                randomAction = currentState.getNextBiasRandomAction(base.RandomGenerator);
                currentState.ApplyAction(randomAction);
                currentDepth++;
                //if (currentDepth > 4) break;
            }
            if (currentDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = currentDepth;
            return new Reward(currentState.score);
        }

        protected Reward Playout(State initialPlayoutState, List<float> weights)
        {
            State currentState = initialPlayoutState.GenerateChildState();
            Action randomAction;
            int currentDepth = 0;
            while (!currentState.IsTerminal())
            {
                currentState.weights = weights;
                randomAction = currentState.getNextBiasRandomAction(base.RandomGenerator);
                currentState.ApplyAction(randomAction);
                currentDepth++;
            }
            if (currentDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = currentDepth;
            return new Reward(currentState.score);
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



    }
}
/**/