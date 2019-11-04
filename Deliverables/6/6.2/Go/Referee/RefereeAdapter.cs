using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;
using PlayerSpace;

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

        public void JsonCommand(JToken jtoken, ref List<JToken> jTokenList)
        {
            //Register Players
            if (!_isSet_player1)
            {
                //Console.WriteLine(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>())));
                _isSet_player1 = true;

                jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>()))));
            }
            else if (!_isSet_player2)
            {
                //Console.WriteLine(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>())));
                _isSet_player2 = true;

                jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>()))));

                //PrintBoardHistory();
            }
            //Play out the match
            else
            {
                string play = jtoken.ToObject<string>();
                try
                {
                    jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(_referee.GetBoardHistory())));

                    if (play == "pass")
                        _referee.Pass();
                    else
                        _referee.Play(play);
                }
                catch (RefereeException)
                {
                    List<PlayerWrapper> victors = _referee.GetVictors();
                    List<string> names = new List<string>();
                    foreach (PlayerWrapper victor in victors)
                        names.Add(victor.GetName());

                    //Console.WriteLine(JsonConvert.SerializeObject(names.ToArray()));
                    jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(names.ToArray())));

                    throw new RefereeException("Game has ended");
                }
            }
        }

        public void PrintBoardHistory()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_referee.GetBoardHistory()));
        }
    }
}
