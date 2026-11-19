using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    enum Country
    {
        Belarus,
        Russia,
        Ukraine,
        Poland,
        Germany,
        France,
        Italy,
        Spain,
        Portugal,
        Netherlands,
        Belgium,
        Switzerland,
        Austria,
        CzechRepublic,
        Slovakia,
        Hungary,
        Lithuania,
        Latvia,
        Estonia,
        Finland,
        Sweden,
        Norway,
        Denmark,
        UnitedKingdom,
        UnitedStates,
        Canada,
        China,
        Japan,
        SouthKorea,
        India,
        Turkey,
        Brazil,
        Argentina,
        Australia,
        NewZealand,
        Egypt,
        SouthAfrica,
        Thailand,
        Vietnam,
        Indonesia,
        Malaysia
    }

    enum OperationType
    {
        Purchase,
        Accrual
    }

    public enum Nominals
    {
        OnePound = 1,
        TwoPounds = 2,
        FivePounds = 5,
        TenPounds = 10,
        TwentyPounds = 20,
        FiftyPounds = 50,
        OneDollar = 100,
        FiveDollars = 500,
        TenDollars = 1000,
        TwentyDollars = 2000,
        FiftyDollars = 5000,
        OneHundrendDollars = 10000,
        TwoHundredDollars = 20000
    }
    enum SupplyOperations
    {
        WriteOff,
        Arrival
    }
}
