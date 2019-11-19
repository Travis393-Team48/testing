using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuleCheckerSpace;
using CustomExceptions;
using BoardSpace;

namespace PlayerSpace
{
    /*
     * Simulates an aiPlayer
     * What strategy the aiPlayer uses depends on aiType field
     * options are "dumb" and "less dumb"
     * 
     * Register must be called before use of other functions
     * ReceiveStones must be called before MakeAMove
     * GetStones must be called after ReceiveStones
     */
    class Player : IPlayer
    {
        string _name;
        string _stone;
        string _AIType;
        int _n;

        public Player(string aiType, int n)
        {
            _AIType = aiType;
            _n = n;
        }

        /* 
         * Initializes Player
         */
        public string Register(string name)
        {
            _name = name;
            return _name;
        }

        /*
         * Sets stone of player
         */
        public void ReceiveStones(string stone)
        {
            _stone = stone;
        }

        /*
         * Returns a point if there is a valid move given the board history
         *      point is determined by _AIType
         * Returns "pass" if there are no valid moves
         * Throws an exception if board history is illegal
         */
        public string MakeAMove(string[][][] boards)
        {
            try
            {
                RuleChecker.CheckHistory(_stone, boards);
            }
            catch(RuleCheckerException)
            {
                throw new PlayerException("This history makes no sense!");
            }

            switch (_AIType)
            {
                case "illegal":
                    Random rng = new Random();
                    if (rng.NextDouble() > 0.2)
                        goto case "dumb";
                    else
                        return "illegl move";
                case "less dumb":
                    string oppositeStone;
                    if (_stone == "B")
                        oppositeStone = "W";
                    else
                        oppositeStone = "B";

                    BoardWrapper boardObject = new BoardWrapper(boards[0], boards[0].Length);
                    List<string> pointsList = boardObject.GetPoints(oppositeStone);
                    List<string> iterate;
                    List<List<string>> possibleMoves = new List<List<string>>();

                    //Find all oppositeStones with only one adjacent liberty
                    //Add {point, adjacentLiberty} to a possibleMoves
                    foreach (string point in pointsList)
                    {
                        iterate = boardObject.GetAdjacentLiberties(point);
                        iterate.Insert(0, point);
                        if (iterate.Count == 2)
                            possibleMoves.Add(iterate);
                    }

                    List<List<string>> temp = new List<List<string>>();
                    //Check the validity of each possibleMove
                    //Also check that making that possibleMove will result in a capture
                    //Failing these conditions, remove move from possibleMoves
                    foreach (List<string> move in possibleMoves)
                    {
                        try
                        {
                            RuleChecker.CheckMove(_stone, move[1], boards);
                        }
                        catch (Exception e)
                        {
                            if (!(e is RuleCheckerException) && !(e is BoardException))
                                throw;
                            continue;
                        }

                        boardObject = new BoardWrapper(boards[0], boards[0].Length);
                        boardObject.PlaceStone(_stone, move[1]);
                        if (!boardObject.Reachable(move[0], " "))
                            temp.Add(move);
                    }
                    possibleMoves = temp;

                    //sort in lowest column lowest row order (numeric)
                    possibleMoves.Sort(ComparePossibleMoves);

                    //for n == 1, return the first possible Move (if it exists)
                    if (_n == 1)
                        if (possibleMoves.Count > 0)
                            return possibleMoves[0][1];
                    //Otherwise, n > 1, and if there is only one possibleMove, return it
                    if (possibleMoves.Count == 1)
                        return possibleMoves[0][1];
                    //Otherwise, play dumb
                    goto case "dumb";
                case "dumb":
                    for (int i = 0; i < boards[0].Length; i++)
                        for (int j = 0; j < boards[0].Length; j++)
                        {
                            try
                            {
                                string point = (i + 1).ToString() + "-" + (j + 1).ToString();
                                RuleChecker.CheckMove(_stone, point, boards);
                                return point;
                            }
                            catch (Exception e)
                            {
                                if (!(e is RuleCheckerException) && !(e is BoardException))
                                    throw;
                            }
                        }
                    break;
            }

            return "pass";
        }

        public string GetStone()
        {
            return _stone;
        }

        public string GetName()
        {
            return _name;
        }

        //Helper function for sorting possible moves()
        private static int ComparePossibleMoves(List<string> x, List<string> y)
        {
            int[] px = ParsingHelper.ParsePoint(x[1]);
            int[] py = ParsingHelper.ParsePoint(y[1]);

            if (px[0] == py[0])
            {
                if (px[1] > py[1])
                    return 1;
                return -1;
            }
            if (px[0] > py[0])
                return 1;
            return -1;
        }
    }
}
