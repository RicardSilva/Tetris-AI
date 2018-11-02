using UnityEngine;
using System.Collections;

public class Action {

    public int column { get; set; }
    public bool[,] piece { get; set; }

    public Action(int column, bool[,] piece) {
        this.column = column;
        this.piece = piece;
    }
  
 
}
