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
            ValidationMethods.ValidateBoard(board);
            return RuleChecker.Score(board);
        }

        public static bool Pass()
        {
            return RuleChecker.Pass();
        }

        public static bool Play(string stone, string point, string[][][] boards)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point);
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
            ValidationMethods.ValidatePoint(point);
            ValidationMethods.ValidateBoards(boards);
            return RuleChecker.CheckMove(stone, point, boards);
        }
    }
}
