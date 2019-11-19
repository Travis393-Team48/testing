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
         * Holds an PlayerWrapper object which can hold either a Player or PlayerProxy
         */

        private PlayerWrapper _player;

        public PlayerAdapter(string aiType, int n = 1)
        {
            _player = new PlayerWrapper(aiType, n);
        }

        public PlayerAdapter(int port)
        {
            _player = new PlayerWrapper(port);
        }

        public JToken JsonCommand(JToken jtoken, string name = "no name")
        {
            JsonValidation.ValidateJTokenPlayer(jtoken);

            try
            {
                switch (jtoken.ElementAt(0).ToObject<string>())
                {
                    case "register":
                        try
                        {
                            name = _player.Register(name);
                            return JToken.Parse(JsonConvert.SerializeObject(name));
                        }
                        catch(JsonSerializationException e)
                        {
                            throw new InvalidJsonInputException(e.Message);
                        }
                    case "receive-stones":
                        try
                        {
                            _player.ReceiveStones(jtoken.ElementAt(1).ToObject<string>());
                        }
                        catch (JsonSerializationException e)
                        {
                            throw new InvalidJsonInputException(e.Message);
                        }
                        catch (ArgumentException e)
                        {
                            throw new InvalidJsonInputException(e.Message);
                        }
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
                        catch (PlayerProxyException e)
                        {
                            return JToken.Parse(JsonConvert.SerializeObject(e.Message));
                        }
                        catch (JsonSerializationException e)
                        {
                            throw new InvalidJsonInputException(e.Message);
                        }
                }
            }

            catch (WrapperException e)
            {
                throw new InvalidJsonInputException("A wrapper exception occurred", e);
            }
            throw new InvalidJsonInputException("Unrecognized JSONCommand passed to PlayerWrapper");

        }
    }
}
