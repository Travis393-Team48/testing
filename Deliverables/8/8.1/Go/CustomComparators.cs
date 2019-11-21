using System;
using PlayerSpace;

public static class CustomComparators
{
    public static int ComparePlayers(PlayerWrapper playerA, PlayerWrapper playerB)
    {
        string a = playerA.GetName();
        string b = playerB.GetName();

        int aL = a.Length;
        int bL = b.Length;
        for (int i = 0; i < Math.Min(aL, bL); i++)
        {
            if (char.IsUpper(a[i]) && char.IsLower(b[i])) //A < b
                return -1;
            else if (char.IsLower(a[i]) && char.IsUpper(b[i])) //a > B
                return 1;
            else //if characters are the same case or one character isn't a letter
            {
                return a[i].CompareTo(b[i]);
            }
        }

        return (aL > bL) ? 1 : -1;
    }
}
