using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RuleCheckerSpace
{
    /* 
     * Wrapper for the RuleChecker Class
     * Provides checks to make sure other classes interact correctly with the RuleChecker
     */
    public static class RuleCheckerWrapper
    {
        public static JObject Score(string[][] board)
        {
            ValidationMethods.ValidateBoard(board, board.Length);
            return RuleChecker.Score(board);
        }

        public static bool Pass()
        {
            return RuleChecker.Pass();
        }

        public static bool Play(string stone, string point, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, boards[0].Length);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.Play(stone, point, boards);
        }

        public static bool CheckHistory(string stone, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckHistory(stone, boards);
        }

        public static bool CheckMove(string stone, string point, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, boards[0].Length);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckMove(stone, point, boards);
        }

        public static bool CheckStartWithEmptyBoard(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckStartWithEmptyBoard(boards);
        }

        public static bool CheckBlackPlaysFirst(string stone, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckBlackPlaysFirst(stone, boards);
        }

        public static bool CheckStoneChanges(string stone, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckStoneChanges(stone, boards);
        }

        public static bool CheckPiecesWithoutLiberties(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckPiecesWithoutLiberties(boards);
        }

        public static bool CheckSuicide(string stone, string point, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, boards[0].Length);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckSuicide(stone, point, boards);
        }

        public static bool CheckKOForPlay(string stone, string point, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, boards[0].Length);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckKOForPlay(stone, point, boards);
        }

        public static bool CheckKOInHistory(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckKOInHistory(boards);
        }

        public static bool CheckPass(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckPass(boards);
        }

        public static List<List<List<string>>> GetPointsLists(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.GetPointsLists(boards);
        }
    }
}
