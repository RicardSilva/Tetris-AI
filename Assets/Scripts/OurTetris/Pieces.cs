
public class Pieces {

    public static readonly string[] pieces = new string[7] { "i", "l", "j", "o", "s", "z", "t" };

    //i
    public static readonly bool[,] i0 = new bool[4, 1] {{true}, {true}, {true}, {true}};
    public static readonly bool[,] i1 = new bool[1, 4] {{true, true, true, true}};

    //l
    public static readonly bool[,] l0 = new bool[3, 2] {{true, true}, { true, false }, {true, false}};
    public static readonly bool[,] l1 = new bool[2, 3] {{true, false, false}, {true, true, true}};
    public static readonly bool[,] l2 = new bool[3, 2] {{false, true}, {false, true}, {true, true}};
    public static readonly bool[,] l3 = new bool[2, 3] {{true, true, true}, {false, false, true}};

    //j
    public static readonly bool[,] j0 = new bool[3, 2] { { true, true }, { false, true }, { false, true } };
    public static readonly bool[,] j1 = new bool[2, 3] { { true, true, true }, { true, false, false } };
    public static readonly bool[,] j2 = new bool[3, 2] { { true, false }, { true, false }, { true, true } };
    public static readonly bool[,] j3 = new bool[2, 3] { { false, false, true }, { true, true, true } };

    //o
    public static readonly bool[,] o0 = new bool[2, 2] { { true, true }, { true, true } };

    //s
    public static readonly bool[,] s0 = new bool[2, 3] { { true, true, false }, { false, true, true } };
    public static readonly bool[,] s1 = new bool[3, 2] { { false, true }, { true, true }, { true, false } };

    //z
    public static readonly bool[,] z0 = new bool[2, 3] { { false, true, true }, { true, true, false } };
    public static readonly bool[,] z1 = new bool[3, 2] { { true, false }, { true, true }, { false, true } };

    //t
    public static readonly bool[,] t0 = new bool[2, 3] { { true, true, true }, { false, true, false } };
    public static readonly bool[,] t1 = new bool[3, 2] { { true, false }, { true, true }, { true, false } };
    public static readonly bool[,] t2 = new bool[2, 3] { { false, true, false }, { true, true, true } };
    public static readonly bool[,] t3 = new bool[3, 2] { { false, true }, { true, true }, { false, true } };

}
