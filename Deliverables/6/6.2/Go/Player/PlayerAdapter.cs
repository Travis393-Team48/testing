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
    public class PlayerAdapter
    {
        /* 
         * Interfaces between the Player and Json Inputs
         * Provides a method called JsonCommand which is how Json Inputs interact with Player
         * Json Commands must be in the format ["register"], ["receive-stones", Stone], and ["make-a-move", Boards]
         * Returns JSON data as a JToken (if input is valid) 
         * Holds an Player object
         */

        private PlayerWrapper _player;

        public JToken JsonCommand(JToken jtoken, string name = "no name", string AIType = "dumb", int n = 1)
        {
            JsonValidation.ValidateJTokenPlayer(jtoken);

            switch (jtoken.ElementAt(0).ToObject<string>())
            {
                case "register":
                    if (AIType == "less dumb")
                        _player = new PlayerWrapper(name, AIType, n);
                    else
                        _player = new PlayerWrapper(name, AIType);
                    return JToken.Parse(JsonConvert.SerializeObject(name));
                case "receive-stones":
                    _player.ReceiveStones(jtoken.ElementAt(1).ToObject<string>());
                    return JToken.Parse(JsonConvert.SerializeObject(null));
                case "make-a-move":
                    try
                    {
                        return JToken.Parse(JsonConvert.SerializeObject(_player.MakeAMove(
                            jtoken.ElementAt(1).ToObject<string[][][]>())));
                    }
                    catch (PlayerException e)
                    {
                        return JToken.Parse(JsonConvert.SerializeObject(e.Message));
                    }
            }

            throw new InvalidJsonInputException("Unrecognized JSONCommand passed to PlayerWrapper");

        }
    }
}
