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
    public class PlayerWrapper
    {
        private Player _player;
        private bool _receive_stones_flag;

        public PlayerWrapper(string name, string aiType)
        {
            ValidationMethods.ValidateAIType(aiType);
            _player = new Player(name, aiType);
        }

        public PlayerWrapper(string name, string aiType, int n)
        {
            ValidationMethods.ValidateAIType(aiType);
            ValidationMethods.ValidateN(n);
            _player = new Player(name, aiType, n);
        }

        public void ReceiveStones(string stone)
        {
            ValidationMethods.ValidateStone(stone);
            _player.ReceiveStones(stone);
            _receive_stones_flag = true;
        }

        public string MakeAMove(string[][][] boards)
        {
            ValidationMethods.ValidateBoards(boards);
            if (!_receive_stones_flag)
                throw new WrapperException("Protocols of interaction violation in PlayerWrapper: ReceiveStones not called before MakeAMove");
            return _player.MakeAMove(boards);
        }

        public string GetStone()
        {
            return _player.GetStone();
        }

        public string GetName()
        {
            return _player.GetName();
        }
    }
}
