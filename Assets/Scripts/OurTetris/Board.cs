using UnityEngine;
using System.Collections;
using System;

public class Board  {

    public const int w = 10;
    public const int h = 18;

    public bool[,] board { get; set; }

    public Board()
    {
        this.board = new bool[w, h];
               
    }
    public Board(Board original)
    {
        this.board = new bool[w, h];
        bool[,] originalBoard = original.board;
        bool keepGoing = true;
        //for (int x = 0; x < w; x++)
        //    for (int y = 0; y < h; y++)
        //    {
        //        this.board[x, y] = originalBoard[x, y];
        //    }

        for (int y = 0; y < h && keepGoing; y++)
        { //OPTIMIZATION: STOP COPYING WHEN EMPTY ROW IS FOUND
            keepGoing = false;
            for (int x = 0; x < w; x++)
            {
                if (originalBoard[x, y] == true)
                {
                    keepGoing = true;
                    this.board[x, y] = true;
                }
            }
        }

    }

    public int ColumnHeight(int x)
    {
        int y = h;
        bool[,] b = this.board;
        while (--y >= 0)
            if (b[x, y] == true)
                break;
        return y + 1;
    }
    public void FillPosition(int x, int y)
    {
        if ((x >= 0 && x < w) && (y >= 0 && y < h))
            this.board[x, y] = true;
    }
    public bool IsPositionFull(int x, int y)
    {
        if (x < 0 || x >= w) return false;
        else if (y < 0 || y >= h) return false;
        return this.board[x, y];
    }
    public bool IsRowComplete(int y)
    {
        bool[,] b = this.board;
        for (int x = 0; x < w; x++)
            if (b[x, y] == false)
                return false;
        return true;
    }
    public void RemoveRow(int y)
    {
        for (int x = 0; x < w; x++)
            this.board[x, y] = false;
        while (y < (h - 1))
        {
            for (int x = 0; x < w; x++)
                this.board[x, y] = this.board[x, y + 1];
            y++;
        }
        for (int x = 0; x < w; x++)
            this.board[x, h - 1] = false;
    }
    public bool IsTopRowFull()
    {
        bool[,] b = this.board;
        for (int x = 0; x < w; x++)
            if (b[x, h - 1] == true)
                return true;
        return false;
    }
    public bool SameBoard(Board other)
    {
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (this.board[x, y] != other.board[x, y])
                    return false;
        return true;
    }

    public void InsertPiece(Action action)
    {
        bool[,] piece = action.piece;
        int column = action.column;
        
        int x = column;
        int y = h - 1;

        int pieceWidth = piece.GetLength(0);
        int pieceHeight = piece.GetLength(1);

        bool DecreaseRow = true;

        while (DecreaseRow && y > 0)
        {
            
            for (int i = 0; i < pieceWidth; i++)
                for (int j = 0; j < pieceHeight; j++)
                    if (piece[i, j] && this.IsPositionFull(x + i, y - 1 + j))
                        DecreaseRow = false;
            if(DecreaseRow) y--;
        }

        for (int i = 0; i < pieceWidth; i++)
            for (int j = 0; j < pieceHeight; j++)
                if(piece[i,j]) this.FillPosition(x + i, y + j);
    }

    
    
    public void Draw()
    {
        for (int y = h - 1; y >= 0; y--)
        {
            for (int x = 0; x < w; x++)
				Debug.Log(this.board[x, y] + " ");
            Debug.Log(" ");
        }
    }
    
}
