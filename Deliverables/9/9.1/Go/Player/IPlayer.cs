namespace PlayerSpace
{
    interface IPlayer
    {
        string Register(string name);

        void ReceiveStones(string stone);

        string MakeAMove(string[][][] boards);

        string GetStone();

        string GetName();
    }
}
