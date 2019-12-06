using System.Collections.Generic;
using CustomExceptions;
using PlayerSpace;


namespace RefereeSpace
{
    /* 
     * Wrapper for the Referee Class
     * Provides checks to make sure other classes interact correctly with the Referee
     * Also checks thaat register is called twice before using Referee
     */
    class RefereeWrapper
    {
        Referee _referee;
        int _players_set;

        public RefereeWrapper(PlayerWrapper player1, PlayerWrapper player2, int size = 19)
        {
            if (size < 1 || size > 19)
                throw new WrapperException("Invalid size passed to RefereeWrapper");
            _referee = new Referee(player1, player2, size);
            _players_set = 0;
        }

        public string AssignPlayer()
        {
            _players_set++;
            if (_players_set > 2)
                throw new WrapperException("Protocols of interaction violation in RefereeWrapper: Cannot assign more than two players");
            return _referee.AssignPlayer();
        }

        /* Passes the game 
         * throws and exception if game should end and updates _victors (sorted by lexigraphical order)
         */
        public void Pass()
        {
            if (_players_set != 2)
                throw new WrapperException("Protocols of interaction violation in RefereeWrapper: AssignPlayer not called twice before Pass");
            _referee.Pass();
        }

        /*
         * Simulates a move being made and updates _board_history, _pass_count, and current_player
         * If the move is illegal, throws an exception adn updates _victors
         */
        public void Play(string point)
        {
            if (_players_set != 2)
                throw new WrapperException("Protocols of interaction violation in RefereeWrapper: AssignPlayer not called twice before Play");
            ValidationMethods.ValidatePoint(point);
            _referee.Play(point);
        }

        public string[][][] GetBoardHistory()
        {
            return _referee.GetBoardHistory();
        }

        public List<PlayerWrapper> GetVictors()
        {
            return _referee.GetVictors();
        }

        public List<string> RefereeGame(string name1, string name2)
        {
            bool out_has_cheater;
            return _referee.RefereeGame(name1, name2, out out_has_cheater);
        }

        public List<string> RefereeGame(string name1, string name2, out bool has_cheater)
        {
            return _referee.RefereeGame(name1, name2, out has_cheater);
        }
    }
}
