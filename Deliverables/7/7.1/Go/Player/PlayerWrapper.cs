using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;

namespace PlayerSpace
{
    /* 
     * Wrapper for the Player Class
     * Provides checks to make sure other classes interact correctly with the player
     */
    public class PlayerWrapper : IPlayer
    {
        private IPlayer _player;
        private bool _register_flag;
        private bool _receive_stones_flag;

        public PlayerWrapper(bool remote, int port = 0)
        {
            if (remote)
                _player = new PlayerProxy(port);
            else
                _player = new Player();
        }

        public string Register(string name, string aiType, int n = 1)
        {
            ValidationMethods.ValidateAIType(aiType);
            ValidationMethods.ValidateN(n);
            _register_flag = true;
            return _player.Register(name, aiType, n);
        }

        public void ReceiveStones(string stone)
        {
            ValidationMethods.ValidateStone(stone);
            if (!_register_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: Register not called before ReceiveStones");
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
    }
}
