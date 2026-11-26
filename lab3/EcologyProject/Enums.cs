using System;
using System.Collections.Generic;
using System.Text;

namespace EcologyProject
{
    public enum MammalTypes
    {
        predator, //хищники
        rodent, //грызуны
        primate, //приматы
        chiroptera, //рукокрылые
        insectivorous, //насекомоядные
        artiodactyl, //парнокопытные
        perissodactyl//непарнокопытные
    }

    public enum BirdTypes
    {
        passeriformes, //воробьинообразные
        woody, //древесные
        waterfaul, //водоплавающие
        predator, //хищники
        nonflying //нелетающие

    }
    public enum FishTypes
    {
        freshwater, //пресноводные
        marine, //морские
        anadromous //проходные
    }

    public enum FactoryType
    {
        TPP, //thermal power plant - ТЭС
        HPS, //hydroelectric power station - ГЭС
        NPP //nuclear power plant - АЭС
    }

    public enum PollutionType
    {
        Air,
        Water,
        Soil
    }
}
