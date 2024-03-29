﻿using CustomExceptions;
using System.Net.Sockets;

namespace PlayerSpace
{
    /* 
     * Wrapper for the Player Class
     * Provides checks to make sure other classes interact correctly with the player
     * Holds either a Player or PlayerProxy Object
     * 
     * Protocals of Interaction:
     *  Register must be called before use of other functions
     *  ReceiveStones must be called before MakeAMove
     *  GetStones must be called after ReceiveStones
     */
    public class PlayerWrapper : IPlayer
    {
        private IPlayer _player;
        private bool _register_flag;
        private bool _receive_stones_flag;

        public PlayerWrapper(Socket socket)
        {
            _player = new PlayerProxy(socket);
        }

        public PlayerWrapper(string aiType, int n = 1)
        {
            ValidationMethods.ValidateAIType(aiType);
            ValidationMethods.ValidateN(n);
            _player = new Player(aiType, n);
        }

        //Use specifically for illegal players
        public PlayerWrapper(string aiType, string configuration)
        {
            if (aiType == "illegal")
            {
                _player = new PlayerIllegal(configuration);
            }
            else
                throw new WrapperException("Wrapper Exception in PlayerWrapper: Invalid aiType when creating an illegal client");
        }

        public string Register(string name)
        {
            if (_register_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Register called twice");
            _register_flag = true;
            return _player.Register(name);
        }

        public void ReceiveStones(string stone)
        {
            ValidationMethods.ValidateStone(stone);
            if (!_register_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Register not called before ReceiveStones");
            if (_receive_stones_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Receive stone called twice on a non-touornament player");
            _player.ReceiveStones(stone);
            _receive_stones_flag = true;
        }

        public string MakeAMove(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            if (!_register_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Register not called before MakeAMove");
            if (!_receive_stones_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: ReceiveStones not called before MakeAMove");
            return _player.MakeAMove(boards);
        }

        public string GetStone()
        {
            if (!_receive_stones_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: ReceiveStones not called before GetStone");
            return _player.GetStone();
        }

        public string GetName()
        {
            if (!_register_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Register not called before GetName");
            return _player.GetName();
        }

        public string EndGame()
        {
            string ok = _player.EndGame();
            if (ok != "OK")
                throw new WrapperException("Player did not return \"OK\" in PlayerWrapper");
	        _receive_stones_flag = false;
            return ok;
        }
    }
}
