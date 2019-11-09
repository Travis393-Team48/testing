using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSpace
{
    interface IPlayer
    {
        string Register(string name, string aiType, int n);

        void ReceiveStones(string stone);

        string MakeAMove(string[][][] boards);

        string GetStone();

        string GetName();
    }
}
