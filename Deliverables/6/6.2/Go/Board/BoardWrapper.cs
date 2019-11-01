using System;
using System.Collections.Generic;

namespace BoardSpace
{
    /* 
     * Wrapper for the Board Class
     * Provides checks to make sure other classes interact correctly with the board
     */
    public class BoardWrapper
    {
        private Board _board;
        private int _size;

        public BoardWrapper(string[][] newBoard = null, int size = 19)
        {
            if (newBoard != null)
                ValidationMethods.ValidateBoard(newBoard, size);
            _size = size;
            _board = new Board(newBoard, size);
        }

        public bool Occupied(string point)
        {
            ValidationMethods.ValidatePoint(point, _size);
            return _board.Occupied(point);
        }

        public bool Occupies(string stone, string point)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, _size);
            return _board.Occupies(stone, point);
        }

        public bool Reachable(string point, string maybeStone)
        {
            ValidationMethods.ValidatePoint(point, _size);
            ValidationMethods.ValidateMaybeStone(maybeStone);
            return _board.Reachable(point, maybeStone);
        }

        public string[][] PlaceStone(string stone, string point)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, _size);
            return _board.PlaceStone(stone, point);
        }

        public string[][] RemoveStone(string stone, string point)
        {
            ValidationMethods.ValidateStone(stone);
            ValidationMethods.ValidatePoint(point, _size);
            return _board.RemoveStone(stone, point);
        }

        public List<string> GetPoints(string maybeStone)
        {
            ValidationMethods.ValidateMaybeStone(maybeStone);
            return _board.GetPoints(maybeStone);
        }

        public List<string> GetAdjacentLiberties(string point)
        {
            ValidationMethods.ValidatePoint(point, _size);
            return _board.GetAdjacentLiberties(point);
        }

        public int GetSize()
        {
            return _board.GetSize();
        }
    }
}
