using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExceptions;
using PlayerSpace;


namespace RefereeSpace
{
    /* 
     * Wrapper for the Referee Class
     * Provides checks to make sure other classes interact correctly with the Referee
     */
    class RefereeWrapper
    {
        Referee _referee;
        int _players_set;

        public RefereeWrapper(int size = 19)
        {
            if (size < 1 || size > 19)
                throw new WrapperException("Invalid size passed to RefereeWrapper");
            _referee = new Referee(size);
            _players_set = 0;
        }

        public string Register(string name, string aiType = "human")
        {
            ValidationMethods.ValidateAIType(aiType);
            _players_set++;
            return _referee.Register(name, aiType);
        }

        /* Passes the game 
         * throws and exception if game should end and updates _victors (sorted by lexigraphical order)
         */
        public void Pass()
        {
            if (_players_set != 2)
                throw new WrapperException("Protocols of interaction violation in RefereeWrapper: Register not called twice before Pass");
            _referee.Pass();
        }

        /*
         * Simulates a move being made and updates _board_history, _pass_count, and current_player
         * If the move is illegal, throws an exception adn updates _victors
         */
        public void Play(string point)
        {
            if (_players_set != 2)
                throw new WrapperException("Protocols of interaction violation in RefereeWrapper: Register not called twice before Play");
            ValidationMethods.ValidatePoint(point);
            _referee.Play(point);
        }

        public string[][][] GetBoardHistory()
        {
            return _referee.GetBoardHistory();
        }

        public void SetBoardHistory(string[][][] bh)
        {
            ValidationMethods.ValidateBoards(bh);
            _referee.SetBoardHistory(bh);
        }

        public List<PlayerWrapper> GetVictors()
        {
            return _referee.GetVictors();
        }
    }
}
