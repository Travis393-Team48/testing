using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;

static class ValidationMethods
{
    public static void ValidateBoard(string[][] board, int size = 19)
    {
        if (board.Length != size)
            throw new WrapperException("Invalid board passed to Wrapper: number of rows != " + size);

        foreach (string[] row in board)
        {
            if (row.Length != size)
                throw new WrapperException("Invalid board passed to Wrapper: number of elements in row != " + size);
            foreach (string element in row)
                if (element != "W" && element != "B" && element != " ")
                    throw new WrapperException("Invalid board passed to Wrapper: invlaid elements in board");
        }
    }
    
    public static void ValidateStone(string stone)
    {
        if (stone != "W" && stone != "B")
            throw new WrapperException("Invalid stone passed to Wrapper: not a valid stone (\"W\" or \"B\")");
    }

    public static void ValidateMaybeStone(string stone)
    {
        if (stone != "W" && stone != "B" && stone != " ")
            throw new InvalidJsonInputException("Invalid MaybeStone passed to Wrapper: not a valid MaybeStone (\"W\" or \"B\" or \" \")");
    }

    public static void ValidatePoint(string point, int size = 19)
    {
        int[] newPoint;
        try
        {
            newPoint = ParsingHelper.ParsePoint(point);
        }
        catch
        {
            throw new WrapperException("Invalid point passed to Wrapper: cannot be parsed" + point);
        }
        try
        {
            if (0 <= newPoint[0] && newPoint[0] <= size - 1 && 0 <= newPoint[1] && newPoint[1] <= size - 1)
                return;
        }
        catch
        {
            throw new WrapperException("Invalid point passed to Wrapper: does not contain two valid points");
        }
        throw new WrapperException("Invalid point passed to Wrapper: coordinates are not > 0 and < " + size);

    }

    public static void ValidateBoards(string[][][] boards)
    {
        if (boards.Count() > 3 || boards.Count() < 0)
            throw new WrapperException("Invalid boards passed to Wrapper: invalid number of boards");

        int size = boards[0].Length;
        try
        {
            foreach (string[][] b in boards)
                ValidateBoard(b, size);
        }
        catch (WrapperException)
        {
            throw new WrapperException("Invalid boards passed to Wrapper: boards contains an invalid board");
        }
    }

    public static void ValidateAIType(string aiType)
    {
        if (aiType != "dumb" && aiType != "less dumb" && aiType != "human" &&  aiType != "illegal")
            throw new WrapperException("Invalid aiType passed to Wrapper: " + aiType);
    }

    public static void ValidateN(int n)
    {
        if (n < 0)
            throw new WrapperException("Invalid n passed to Wrapper: n cannot be less than 0");
    }
}
