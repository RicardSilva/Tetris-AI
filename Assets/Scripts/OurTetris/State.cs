using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class State {

    public Board board { set; get; }
    public int score { set; get; }
    //public string nextPiece { get; set; }
    private List<Action> Actions { get; set; }
    public List<string> RemainingPieces { get; set; }
    protected IEnumerator<Action> ActionEnumerator { get; set; }

	
    //public List<float> weights = new List<float>{ -2.079252f, 13.219479f, 1.533691f, -10.49888f, 6.048898f };
    public List<float> weights = new List<float> { -4.08502f, -20.81908f, -52.5731f, 0.07976406f, 6.183765f };
    

    public State (List<string> pieces)
    {
        this.board = new Board();
        this.score = 0;
        this.RemainingPieces = pieces;
        this.Actions = this.GetPossibleActions(this.RemainingPieces[0]);
        this.ActionEnumerator = Actions.GetEnumerator();
    }
    public State (State original)
    {
        this.board = new Board(original.board);
        this.score = original.score;
        this.RemainingPieces = new List<string>();
        this.weights = new List<float>();
        for (int i = 0; i < 5; i++)
            this.weights.Add(original.weights[i]);

        if (original.RemainingPieces.Count > 1)
        {
            for (int i = 1; i < original.RemainingPieces.Count; i++)
                this.RemainingPieces.Add(original.RemainingPieces[i]);

            this.Actions = this.GetPossibleActions(this.RemainingPieces[0]);
            if(this.Actions != null)
                this.ActionEnumerator = Actions.GetEnumerator();
        }

        else
        {
            this.Actions = null;
            this.ActionEnumerator = null;
        }
        
    }
    public State GenerateChildState()
    {
        return new State(this);
    }

    public void AddPiece(string s)
    {
        this.RemainingPieces.Add(s);
    }
    public void Initialize()
    {
        if(this.ActionEnumerator != null)
        this.ActionEnumerator.Reset();
    }
    public bool IsTerminal()
    {
        return this.board.IsTopRowFull() || this.RemainingPieces.Count == 0;
    }
    public bool IsSolution()
    {
        return !this.board.IsTopRowFull();
    }

    private List<Action> GetPossibleActions(string nextPiece) {
        if (this.IsTerminal()) return null;
        List<Action> l = new List<Action>();
        if(string.Equals(nextPiece, "i"))
        {
            l.AddRange(PiecePossibilities(Pieces.i0, 10));
            l.AddRange(PiecePossibilities(Pieces.i1, 7));
        }
        else if (string.Equals(nextPiece, "l"))
        {
            l.AddRange(PiecePossibilities(Pieces.l0, 9));
            l.AddRange(PiecePossibilities(Pieces.l1, 8));
            l.AddRange(PiecePossibilities(Pieces.l2, 9));
            l.AddRange(PiecePossibilities(Pieces.l3, 8));
        }
        else if (string.Equals(nextPiece, "j"))
        {
            l.AddRange(PiecePossibilities(Pieces.j0, 9));
            l.AddRange(PiecePossibilities(Pieces.j1, 8));
            l.AddRange(PiecePossibilities(Pieces.j2, 9));
            l.AddRange(PiecePossibilities(Pieces.j3, 8));
        }
        else if (string.Equals(nextPiece, "o"))
        {
            l.AddRange(PiecePossibilities(Pieces.o0, 9));
        }
        else if (string.Equals(nextPiece, "s"))
        {
            l.AddRange(PiecePossibilities(Pieces.s0, 8));
            l.AddRange(PiecePossibilities(Pieces.s1, 9));
        }
        else if (string.Equals(nextPiece, "z"))
        {
            l.AddRange(PiecePossibilities(Pieces.z0, 8));
            l.AddRange(PiecePossibilities(Pieces.z1, 9));
        }
        else if (string.Equals(nextPiece, "t"))
        {
            l.AddRange(PiecePossibilities(Pieces.t0, 8));
            l.AddRange(PiecePossibilities(Pieces.t1, 9));
            l.AddRange(PiecePossibilities(Pieces.t2, 8));
            l.AddRange(PiecePossibilities(Pieces.t3, 9));
        }
        return l;
    }
    private List<Action> PiecePossibilities(bool[,] piece, int possibilities) {
        List<Action> l = new List<Action>();
        for (int i = 0; i < possibilities; i++)
        {
            l.Add(new Action(i, piece));
        }
        return l;
    }
    public Action GetNextAction()
    {
        if (this.ActionEnumerator == null) return null;
        if (this.ActionEnumerator.MoveNext())
        {
           return this.ActionEnumerator.Current;
        }
        return null;       
    }

    public Action getNextRandomAction(System.Random RandomGenerator)
    {
        if (this.Actions.Count > 0)
        {
            return this.Actions[RandomGenerator.Next(this.Actions.Count)];
        }
        else
        {
            return null;
        }
        
    }
    public Action getNextBiasRandomAction(System.Random RandomGenerator)
    {
        if (this.Actions.Count > 0)
        {
            float[] actionsAccumulatedValues = new float[this.Actions.Count];
            float[] actionsValues = new float[this.Actions.Count];
            State nextState;
            float accumulation = 0;
            if (this.Actions.Count > 0)
            {
                for (int actionIndex = 0; actionIndex < this.Actions.Count; actionIndex++)
                {
                    nextState = this.GenerateChildState();
                    var action = this.Actions[actionIndex];
                    nextState.ApplyAction(action);
                    var actionValue = nextState.GetHValue();

                    accumulation += 100 / (actionValue + 1);

                    actionsValues[actionIndex] = actionValue;
                    actionsAccumulatedValues[actionIndex] = accumulation;

                }

                var roundUp = UnityEngine.Mathf.CeilToInt(accumulation);
                int random = RandomGenerator.Next(roundUp);
                int index = 0;
                for (int actionIndex = 0; actionIndex < this.Actions.Count; actionIndex++)
                {
                    var value = actionsAccumulatedValues[actionIndex];
                    if (random <= value)
                    {

                        index = actionIndex;

                        break;
                    }
                }

                return this.Actions[index];
            }
            else
            {
                return null;
            }
        }
        else
            return null;
        
    }
    public void ApplyAction(Action a)
    {
        int removedRows = 0;
        this.board.InsertPiece(a);
        if (this.board.IsTopRowFull() == true)
            return;

        //remove full rows
        for (int y = 17; y >= 0; y--)
        {
            if (this.board.IsRowComplete(y))
            {
                this.board.RemoveRow(y);
                removedRows++;
            }
        }

        //compute score
        if      (removedRows == 1) { this.score += 100; }
        else if (removedRows == 2) { this.score += 300; }
        else if (removedRows == 3) { this.score += 500; }
        else if (removedRows == 4) { this.score += 800; }
    }

    public float GetHValue()
    {
        //FIXME: THIS IS WHERE THE HEURISTICS NEED TO BE IMPLEMENTED
        //H = K1 * H1 + K2 * H2 + ...

        return Math.Max( weights[0] * this.Bumpiness()
             + weights[1] * this.HighestBlock()
             + weights[2] * this.FreeSpaces()
             + weights[3] * this.score
             + weights[4] * this.AverageHeight(), 0);
    }
    private float Bumpiness()
    {
        float bumpiness = 0;
        int boardW = Board.w;
        int[] heights = new int[boardW];

        for (int x = 0; x < boardW; x++)
        {
            heights[x] = board.ColumnHeight(x);
        }

        for (int x = 0; x < boardW - 1; x++)
        {
             bumpiness += System.Math.Abs (heights[x] - heights[x + 1]);
            
        }
        return bumpiness;
    }
    private float HighestBlock() {
        float maxHeight = 0;
        float currentHeight;
        int boardW = Board.w;
        for (int x = 0; x < boardW; x++) {
            currentHeight = board.ColumnHeight(x);
            if (currentHeight > maxHeight)
                maxHeight = currentHeight;
        }

        return maxHeight;
    }
    private float FreeSpaces() {
        int boardW = Board.w;
        int boardH = Board.h;
        float freeSpaces = boardW * boardH;
        bool keepGoing = true;
        bool[,] b = this.board.board;

        for(int y = 0; y < boardH && keepGoing; y++)
        {
            keepGoing = false;
            for(int x = 0; x < boardW; x++)
            {
                if (b[x, y] == true)
                {
                    keepGoing = true;
                    freeSpaces--;
                }
            }

        }


        return freeSpaces;
    }
    private float AverageHeight()
    {
        int boardW = Board.w;
        int sum = 0;

        for(int x = 0; x < boardW; x++)
        {
            sum += this.board.ColumnHeight(x);
        }
        return sum / boardW;
    }



}
