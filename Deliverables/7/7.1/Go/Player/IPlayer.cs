using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSpace
{
    interface IPlayer
    {
        void ReceiveStones(string stone);

        string MakeAMove(string[][][] boards);

        string GetStone();

        string GetName();
    }
}
