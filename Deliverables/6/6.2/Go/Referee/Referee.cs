using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerSpace;
using BoardSpace;
using CustomExceptions;
using RuleCheckerSpace;
using Newtonsoft.Json.Linq;

namespace RefereeSpace
{
    /*
     * 
     */
    class Referee
    {
        PlayerWrapper _player1;
        PlayerWrapper _player2;
        PlayerWrapper _current_player;
        int _pass_count;
        List<BoardWrapper> _board_history;
        List<PlayerWrapper> _victors;

        public Referee(int size = 19)
        {
            _board_history = new List<BoardWrapper>();
            _victors = new List<PlayerWrapper>();
            _board_history.Add(new BoardWrapper(null, size));
        }

        public string Register(string name, string aiType = "human")
        {
            if (_player1 == null)
            {
                _player1 = new PlayerWrapper(name, aiType);
                _player1.ReceiveStones("B");
                _current_player = _player1;
                return "B";
            }
            else if (_player2 == null)
            {
                _player2 = new PlayerWrapper(name, aiType);
                _player2.ReceiveStones("W");
                return "W";
            }
            throw new InvalidOperationException("Invalid call to Register in Referee: Cannot register more than two players");
        }

        /* Passes the game, updating _pass_count and _current_player
         * throws and exception if game should end and updates _victors (sorted by lexigraphical order)
         */
        public void Pass()
        {
            _pass_count++;
            _current_player = _current_player == _player1 ? _player2 : _player1; 
            if (_pass_count >= 2)
            {
                JObject scores = RuleCheckerWrapper.Score(_board_history[0].GetBoard());
                if (scores["B"].ToObject<int>() == scores["W"].ToObject<int>())
                {
                    _victors.Add(_player1);
                    _victors.Add(_player2);
                    _victors.Sort(CustomComparators.ComparePlayers);
                }
                else if (scores["B"].ToObject<int>() > scores["W"].ToObject<int>())
                    _victors.Add(_player1);
                else
                    _victors.Add(_player2);

                throw new RefereeException("Game has ended");
            }
        }
        
        /*
         * Simulates a move being made and updates _board_history, _pass_count, and current_player
         * If the move is illegal, throws an exception adn updates _victors
         */
        public void Play(string point)
        {
            try
            {
                RuleCheckerWrapper.Play(_current_player.GetStone(), point, GetBoardHistory());
            }
            catch (RuleCheckerException)
            {
                if (_current_player == _player1)
                    _victors.Add(_player2);
                else
                    _victors.Add(_player1);
                throw new RefereeException("Invalid Play in Referee: an illegal move has been made");
            }

            //update _board_history
            BoardWrapper b = new BoardWrapper(_board_history[0].GetBoard(), _board_history[0].GetSize());
            b.PlaceStone(_current_player.GetStone(), point);
            _board_history.Insert(0, b);
            if (_board_history.Count == 4)
                _board_history.RemoveAt(3);

            _pass_count = 0;
            _current_player = _current_player == _player1 ? _player2 : _player1; 
        }

        public string[][][] GetBoardHistory()
        {
            string[][][] bh = new string[_board_history.Count][][];
            for (int i = 0; i < _board_history.Count; i++)
                bh[i] = _board_history[i].GetBoard();
            return bh;
        }

        public void SetBoardHistory(string[][][] bh)
        {
            foreach (string[][] board in bh)
                _board_history.Add(new BoardWrapper(board, board.Length));
        }

        public List<PlayerWrapper> GetVictors()
        {
            return _victors;
        }
    }
}
