using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuleCheckerSpace;
using CustomExceptions;
using BoardSpace;

namespace PlayerSpace
{
    /*
     * Simulates an aiPlayer that return invalid points during make a move depending on configuration
     * After doing stuff depnding on the configuration, wil default to a "smart" player
     * Configurations:
     * "always return 1-1"
     * "return 19-19 once"
     * "return 99-99 once"
     * "return characters once"
     * "return numbers once"
     * "return array once"
     * "pass forever"
     * "return empty"
     */
    class PlayerIllegal : IPlayer
    {
        PlayerWrapper _player;
        string _stone;
        string _configuration;
        bool _once;

        public PlayerIllegal(string configuration)
        {
            _configuration = configuration;
            _player = new PlayerWrapper("smart", 1);
        }

        public string Register(string name)
        {
            return _player.Register(name);
        }

        public void ReceiveStones(string stone)
        {
            _player.ReceiveStones(stone);
            _stone = stone;
        }

        public string MakeAMove(string[][][] boards)
        {
            try
            {
                RuleChecker.CheckHistory(_stone, boards);
            }
            catch(RuleCheckerException)
            {
                throw new PlayerException("This history makes no sense!");
            }

            switch (_configuration)
            {
                case "always return 1-1":
                    return "1-1";
                case "return 19-19 once":
                    if (_once == false)
                    {
                        _once = true;
                        return "19-19";
                    }
                    goto default;
                case "return 99-99 once":
                    if (_once == false)
                    {
                        _once = true;
                        return "99-99";
                    }
                    goto default;
                case "return characters once":
                    if (_once == false)
                    {
                        _once = true;
                        return "illegal move";
                    }
                    goto default;
                case "return numbers once":
                    if (_once == false)
                    {
                        _once = true;
                        return "1234";
                    }
                    goto default;
                case "return array once":
                    if (_once == false)
                    {
                        _once = true;
                        return "[\"illlegal move\"]";
                    }
                    goto default;
				//not illegal, but will let the other player win
				case "pass forever":
					return "pass";
				case "return empty":
					if (_once == false)
					{
						_once = true;
						return "";
					}
					goto default;
                default:
                    return _player.MakeAMove(boards);
            }

            throw new Exception("Illegal player somehow failed to return a move");
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
