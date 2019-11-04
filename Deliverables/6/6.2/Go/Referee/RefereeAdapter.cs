using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;

namespace RefereeSpace
{
    public class RefereeAdapter
    {
        RefereeWrapper _referee;
        bool _isSet_player1;
        bool _isSet_player2;

        public RefereeAdapter(int size = 19)
        {
            _referee = new RefereeWrapper(size);
        }

        public void JsonCommand(JToken jtoken)
        {
            //Register Players
            if (!_isSet_player1)
            {
                Console.WriteLine(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>())));
                _isSet_player1 = true;
            }
            else if (!_isSet_player2)
            {
                Console.WriteLine(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>())));
                _isSet_player2 = true;
            }
            //Play out the match
            else
            {
                string play = jtoken.ToObject<string>();
                try
                {
                    Console.WriteLine(JsonConvert.SerializeObject(_referee.GetBoardHistory()));

                    if (play == "pass")
                        _referee.Pass();
                    else
                        _referee.Play(play);
                }
                catch (RefereeException)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(_referee.GetVictors()));
                    throw new RefereeException("Game has ended");
                }
            }
        }
    }
}
