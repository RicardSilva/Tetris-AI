using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        
        private const float UPDATE_INTERVAL = 0.5f;
        //public fields, seen by Unity in Editor
        
        public Text TotalPiecesText;
        public Text PiecesPlacedText;
        public Text ScoreText;
        
        public Text TotalProcessingTimeText;
        public Text BestDiscontentmentText;
        public Text ProcessedActionsText;
        public GameObject GameEnd;
        public bool GameOver;
        public State game;
		public Drawing draw;

        public const bool MCTSearch = false;
        public const bool MCTSBias = true;
        public const bool MCTSRave = false;
        public const bool MCTSBiasRave = false;
        public const bool learning = false;

        private int piecesPlaced = 0;
        private int totalPieces = 1000;


        public Action CurrentAction { get; private set; }
        public MCTS MCTSDecisionMaking { get; set; }
        public MCTSFastEvo learner { get; set; }


        private float nextUpdateTime = 0.0f;

        public void Start()
        {
            if (learning)
            {
                this.GameOver = false;
                List<string> pieces = new List<string> { "z", "l", "t", "z", "o", "t", "s", "s", "s", "t", "s", "j", "l", "i", "s", "t", "j", "l", "i", "s" };
                game = new State(pieces);
                this.learner = new MCTSFastEvo(game);
                this.learner.InitializeMCTSearch();
            }
            else
            {
                this.GameOver = false;
                List<string> pieces = new List<string>();
                int j;
                for (int i = 0; i < 3; i++)
                {
                    j = Random.Range(0, Pieces.pieces.Length);
                    pieces.Add(Pieces.pieces[j]);
                }

                game = new State(pieces);
                draw = new Drawing(this.game.board);
                draw.Start();
                if (MCTSearch)
                {
                    this.MCTSDecisionMaking = new MCTS(game);
                }
                else if (MCTSBias)
                {
                    this.MCTSDecisionMaking = new MCTSBiasedPlayout(game);
                }
                else if (MCTSRave)
                {
                    this.MCTSDecisionMaking = new MCTSRAVE(game);
                }
                else if (MCTSBiasRave)
                {
                    this.MCTSDecisionMaking = new MCTSBiasedRAVE(game);
                }

                this.CurrentAction = null;
                this.MCTSDecisionMaking.InitializeMCTSearch();
            }

        }

        public void Update()
        {
            if (learning)
            {
                this.learner.Learn();
                Debug.Break();
            }
            else
            {

                if (Time.time > this.nextUpdateTime)
                {
                    this.nextUpdateTime = Time.time + UPDATE_INTERVAL;

                    if (!this.GameOver)
                    {
                        this.UpdateMCTS();

                        if (this.CurrentAction != null)
                        {

                            if (!this.game.IsTerminal())
                            {
                                //Execute action
                                this.game = game.GenerateChildState();
                                this.game.ApplyAction(CurrentAction);
                                if (this.piecesPlaced < this.totalPieces)
                                    this.game.AddPiece(Pieces.pieces[Random.Range(0, Pieces.pieces.Length)]);
                                this.piecesPlaced++;

                                if (MCTSearch)
                                {
                                    this.MCTSDecisionMaking = new MCTS(this.game);

                                }
                                else if (MCTSBias)
                                {
                                    this.MCTSDecisionMaking = new MCTSBiasedPlayout(this.game);
                                }
                                else if (MCTSRave)
                                {
                                    this.MCTSDecisionMaking = new MCTSRAVE(this.game);
                                }
                                else if (MCTSBiasRave)
                                {
                                    this.MCTSDecisionMaking = new MCTSBiasedRAVE(this.game);
                                }
                                this.CurrentAction = null;
                                this.MCTSDecisionMaking.InitializeMCTSearch();
                            }
                        }
                    }
                    if (this.game.IsTerminal()) //gameover
                    {
                        this.GameEnd.SetActive(true);
                        this.GameEnd.GetComponentInChildren<Text>().text = "Game Over";
                        this.GameOver = true;
                    }
                    this.TotalPiecesText.text = "Total pieces: " + this.game.RemainingPieces.Count;
                    this.PiecesPlacedText.text = "Pieces placed: " + this.piecesPlaced;
                    this.ScoreText.text = "Score: " + this.game.score;
                    draw.board = this.game.board;
                    draw.Update();
                }
            }
            
        }

        private void UpdateMCTS()
        {
            if (this.MCTSDecisionMaking.InProgress)
            {
                var action = this.MCTSDecisionMaking.Run();
                if (action != null)
                {
                    this.CurrentAction = action;
                }
            }

            this.TotalProcessingTimeText.text = "Process. Time: " + this.MCTSDecisionMaking.TotalProcessingTime.ToString("F");

            this.ProcessedActionsText.text = "Max Depth: " + this.MCTSDecisionMaking.MaxPlayoutDepthReached.ToString();

            if (this.MCTSDecisionMaking.BestFirstChild != null)
            {
                var q = this.MCTSDecisionMaking.BestFirstChild.Q / this.MCTSDecisionMaking.BestFirstChild.N;
                this.BestDiscontentmentText.text = "Best Exp. Q value: " + q.ToString("F");
            }
        }


    }
}
