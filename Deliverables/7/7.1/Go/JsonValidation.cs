using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

static class JsonValidation
{
    /*
     * New Validation Methods for Board
     */
    public static void ValidateJTokenBoard(JToken jtoken)
    {
        try
        {
            jtoken.ElementAt(0); //check for board
            JToken statement = jtoken.ElementAt(1); //check for statement
            try
            {
                statement.ToObject<string[]>();
            }
            catch
            {
                throw new InvalidJsonInputException("Invalid Jtoken passed to Adapter: statement cannot be converted into string[]");
            }
            statement.ElementAt(1); //check for query/command
            try
            {
                statement.ElementAt(1).ToObject<string>();
            }
            catch
            {
                throw new InvalidJsonInputException("Invalid Jtoken passed to Adapter: query/command cannot be converted into string");
            }
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid Jtoken passed to Adapter: wrong number of elements JSON array or statement");
        }
    }
    public static void ValidateStatementElements(JToken jtoken, int i)
    {
        try
        {
            for (int j = 1; j <= i; j++)
                jtoken.ElementAt(j);
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid" +
                jtoken.ElementAt(0).ToObject<string>() +
                "passed to Adapter: wrong number of elements");
        }
    }
    public static void ValidateBoard(JToken board, int size = 19)
    {
        string[][] newBoard;
        try
        {
            newBoard = board.ToObject<string[][]>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid board passed to Adapter: cannot be converted into a string[][]");
        }

        if (newBoard.Length != size)
            throw new InvalidJsonInputException("Invalid board passed to Adapter: number of rows != " + size);

        foreach (string[] row in newBoard)
        {
            if (row.Length != size)
                throw new InvalidJsonInputException("Invalid board passed to Adapter: number of elements in row != " + size);
            foreach (string element in row)
                if (element != "W" && element != "B" && element != " ")
                    throw new InvalidJsonInputException("Invalid board passed to Adapter: invlaid elements in board");
        }
    }
    public static void ValidateStone(JToken stone)
    {
        string newStone;
        try
        {
            newStone = stone.ToObject<string>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid stone passed to Adapter: cannot be converted into a string");
        }
        if (newStone != "W" && newStone != "B")
            throw new InvalidJsonInputException("Invalid stone passed to Adapter: not a valid stone (\"W\" or \"B\")");
    }
    public static void ValidateMaybeStone(JToken maybeStone)
    {
        string newMaybeStone;
        try
        {
            newMaybeStone = maybeStone.ToObject<string>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid MaybeStone passed to Adapter: cannot be converted into a string");
        }
        if (newMaybeStone != "W" && newMaybeStone != "B" && newMaybeStone != " ")
            throw new InvalidJsonInputException("Invalid MaybeStone passed to Adapter: not a valid MaybeStone (\"W\" or \"B\" or \" \")");
    }
    public static void ValidatePoint(JToken point, int size = 19)
    {
        string strPoint;
        int[] newPoint;
        try
        {
            strPoint = point.ToObject<string>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid point passed to Adapter: cannot be converted into a string");
        }
        try
        {
            newPoint = ParsingHelper.ParsePoint(strPoint);
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid point passed to Adapter: cannot be parsed");
        }
        try
        {
            if (0 <= newPoint[0] && newPoint[0] <= size - 1 && 0 <= newPoint[1] && newPoint[1] <= size - 1)
                return;
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid point passed to Adapter: does not contain two valid points");
        }
        throw new InvalidJsonInputException("Invalid point passed to Adapter: coordinates are not > 0 and < 20");

    }

    /*
     * New Validation Methods for RuleChecker
     */
    //Doesn't check if Board, Stone, or Move is valid
    //Does check that an object resembling Board or [Stone, Move] exists
    public static void ValidateJTokenRuleChecker(JToken jtoken)
    {
        try //check if jtoken is a board
        {
            jtoken.ToObject<string[][]>();
        }
        catch //otherwise, check if jtoken is [Stone, Move]
        {
            try
            {
                jtoken.ElementAt(0);
                jtoken.ElementAt(1);
            }
            catch
            {
                throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: cannot be converted to Board or [Stone, Move]");
            }
            try
            {
                jtoken.ElementAt(2);
                throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: cannot be converted to Board or [Stone, Move]");
            }
            catch { }
        }
    }
    public static void ValidatePass(JToken move)
    {
        try
        {
            string pass = move.ToObject<string>();
            if (pass != "pass")
                throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: pass is not written correctly");
        }
        catch { }
    }
    public static void ValidatePlay(JToken play)
    {
        try
        {
            string point = play.ElementAt(0).ToObject<string>();
            JArray boards = play.ElementAt(1).ToObject<JArray>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: Move does not contain a valid play (or pass)");
        }
    }
    public static void ValidateBoards(JToken boards)
    {
        try
        {
            string[][][] b = boards.ToObject<string[][][]>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid boards passed to Adapter: boards is not an array of string arrays");
        }

        if (boards.Count() > 3 || boards.Count() < 0)
            throw new InvalidJsonInputException("Invalid boards passed to Adapter: invalid number of boards");

        try
        {
            string[][][] b = boards.ToObject<string[][][]>();
            int size = b[0].Length;
            foreach (string[][] i in b)
                ValidateBoard(JToken.Parse(JsonConvert.SerializeObject(i)), size);
        }
        catch (InvalidJsonInputException)
        {
            throw new InvalidJsonInputException("Invalid boards passed to Adapter: boards contains an invalid board");
        }

    }

    /*
     * New Validation Methods for Player
     */
    public static void ValidateJTokenPlayer(JToken jtoken)
    {
        try //check if there is a string in first element
        {
            jtoken.ElementAt(0).ToObject<string>();
        }
        catch
        {
            throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: First element of JArray is not a string (or does not exist)");
        }
        try //check if there is a second element
        {
            jtoken.ElementAt(1);
        }
        catch //if there isn't, make sure jtoken is ["register"]
        {
            if (jtoken.ElementAt(0).ToObject<string>() != "register")
                throw new InvalidJsonInputException("Invalid jtoken passed to Adapter: JArray only contains one element, but it isn't \"register\"");
        }
    }

}
