using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSNode
    {
        public State State { get; private set; }
        public MCTSNode Parent { get; set; }
        public Action Action { get; set; }
        public List<MCTSNode> ChildNodes { get; private set; }
        public int N { get; set; }
        public int NRAVE { get; set; }
        public float Q { get; set; }
        public float QRAVE { get; set; }
        public float H { get; set; }
        public double AcumulatedE {get; set; }

        public MCTSNode(State state)
        {
            this.State = state;
            this.ChildNodes = new List<MCTSNode>();
        }
    }
}
