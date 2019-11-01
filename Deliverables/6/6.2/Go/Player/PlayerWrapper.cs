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
    public class PlayerWrapper
    {
        /* 
         * Interfaces between the Player and Json Inputs
         * Provides a method called JsonCommand which is how Json Inputs interact with Player
         * Also provides checks to make sure inputs are correct
         * Also checks that register and receive-stones are called before make-a-move
         * Json Commands must be in the format ["register"], ["receive-stones", Stone], and ["make-a-move", Boards]
         * Returns JSON data as a JToken (if input is valid) 
         * Holds an Player object
         */

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
    }
}
