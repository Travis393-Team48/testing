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
    /* 
     * Interfaces between the Referee and Json Inputs
     * Provides a method called JsonCommand which is how Json Inputs interact with Referee
     * Json Commands must be in the format [string], where the string is either a name or a move
     * Adds JSON data to jTokenList (if input is valid) 
     * Holds an RefereeWrapper object
     */
    public class RefereeAdapter
    {
        RefereeWrapper _referee;
        bool _isSet_player1;
        bool _isSet_player2;

        public RefereeAdapter(PlayerWrapper player1, PlayerWrapper player2, int size = 19)
        {
            _referee = new RefereeWrapper(player1, player2, size);
        }

        public void JsonCommand(JToken jtoken, ref List<JToken> jTokenList)
        {
            //Register Players
            if (!_isSet_player1)
            {
                _isSet_player1 = true;

                jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>()))));
            }
            else if (!_isSet_player2)
            {
                _isSet_player2 = true;

                jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(_referee.Register(jtoken.ToObject<string>()))));
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

                    jTokenList.Add(JToken.Parse(JsonConvert.SerializeObject(names.ToArray())));

                    throw new RefereeException("Game has ended");
                }
            }
        }

        //Version of JsonCommand that writes to console instead of adding to a JToken List
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
                PrintBoardHistory();
            }
            //Play out the match
            else
            {
                string play = jtoken.ToObject<string>();
                try
                {
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

                    Console.WriteLine(JsonConvert.SerializeObject(names.ToArray()));

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
