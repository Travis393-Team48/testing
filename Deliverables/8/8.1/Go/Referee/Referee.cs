﻿using System;
using System.Collections.Generic;
using PlayerSpace;
using BoardSpace;
using CustomExceptions;
using RuleCheckerSpace;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace RefereeSpace
{
    /* Referee class
     * Expects two players, and calling Register will call Register and Receive Stones for thw two players
     * Hold two players and referees a game between them
     * Throws a RefereeException if a player makes an illegal move
     * Throws an InvalidOperationException if Register is somehow called more than twice (shouldn't ever happen as wrapper checks for this)
     */
    class Referee
    {
        int _players_set;
        PlayerWrapper _player1;
        PlayerWrapper _player2;
        PlayerWrapper _current_player;
        int _pass_count;
        List<BoardWrapper> _board_history;
        List<PlayerWrapper> _victors;
        int _size;

        public Referee(PlayerWrapper player1, PlayerWrapper player2, int size)
        {
            _board_history = new List<BoardWrapper>();
            _victors = new List<PlayerWrapper>();
            _board_history.Add(new BoardWrapper(null, size));
            _player1 = player1;
            _player2 = player2;
            _size = size;
        }

        public string Register(string name)
        {
            if (_players_set == 0)
            {
                _player1.Register(name);
                _player1.ReceiveStones("B");
                _current_player = _player1;
                _players_set++;
                return "B";
            }
            else if (_players_set == 1)
            {
                _player2.Register(name);
                _player2.ReceiveStones("W");
                _players_set++;
                return "W";
            }
            throw new InvalidOperationException("Invalid call to Register in Referee: Cannot register more than two players");
        }

        /* Passes the game, updating _pass_count, _current_player, and _board_history
         * throws and exception if game should end and updates _victors (sorted by lexigraphical order)
         */
        public void Pass()
        {
            _pass_count++;
            _current_player = _current_player == _player1 ? _player2 : _player1;

            //update _board_history
            _board_history.Insert(0, _board_history[0]);
            if (_board_history.Count == 4)
                _board_history.RemoveAt(3);

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
            b.RemoveDeadStones(_current_player.GetStone());
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

        /*
         * Simulates a game between two players
         * Returns a list off victors
         * Players can be local or remote
         * If a remote player sends illegal data or disconnects, the other player is declared victor
         */
        public List<string> RefereeGame()
        {
            while (true)
            {
                try
                {
                    string next_move = _current_player.MakeAMove(GetBoardHistory());
                    if (next_move == "pass")
                       Pass();
                    else
                    {
                        //maybe this function shouldn't be in referee... need to check if this is a valid point b/c no wrappeer
                        ValidationMethods.ValidatePoint(next_move, _size);
                        Play(next_move);
                    }
                    //Don't Update current player!!! already updated in Pass() and Play()
                    //_current_player = _current_player == _player1 ? _player2 : _player1;
                }
                catch (RefereeException)
                {
                    List<PlayerWrapper> victors = GetVictors();
                    List<string> names = new List<string>();
                    foreach (PlayerWrapper victor in victors)
                        names.Add(victor.GetName());

                    return names;
                }
                catch (Exception e)
                {
                    if (e is JsonSerializationException || e is ArgumentException || e is SocketException || e is WrapperException || e is JsonReaderException)
                    {
                        List<string> names = new List<string>();
                        if (_current_player == _player1)
                            names.Add(_player2.GetName());
                        else
                            names.Add(_player1.GetName());
                        return names;
                    }
                    else
                        throw;
                }

            }
        }
    }
}
