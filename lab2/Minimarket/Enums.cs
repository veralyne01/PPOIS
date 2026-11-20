using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public enum Country
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

    public enum OperationType
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

    public enum Status
    {
        LookingForProducts,
        InLineAtCheckout,
        Paying
    }

    enum CustomerName
    {
        Alexander, Maria, Dmitry, Anna, Sergey,
        Elena, Ivan, Olga, Andrey, Natalia,
        Pavel, Tatiana, Nikolai, Irina, Vladimir,
        Svetlana, Mikhail, Ekaterina, Artem, Yulia,
        Maxim, Victoria, Denis, Alina, Kirill,
        Daria, Egor, Polina, Vitaly, Margarita,
        Oleg, Ksenia, Roman, Valeria, Stanislav,
        Alexandra, Yaroslav, Veronika, Gleb, Sofia
    }

    static class RandomHelper
    {
        public static T GetRandom<T>() where T : struct, Enum
        {
            T[] values = Enum.GetValues<T>();
            return values[Random.Shared.Next(values.Length)];
        }
    }
}
