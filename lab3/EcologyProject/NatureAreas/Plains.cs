namespace EcologyProject;

public class Plains : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Plains;

    public Plains(double size, string name) : base(size, name, new[]
    {
        "Волк", "Лиса", "Обыкновенная лиса", "Заяц", "Заяц-русак", "Суслик",
        "Сурок", "Сайгак", "Бизон", "Косуля", "Корсак", "Степной хорёк", "Полёвка",
        "Степной орёл", "Жаворонок", "Перепел", "Дрофа", "Серая куропатка",
        "Журавль-красавка", "Саранча", "Кузнечик", "Богомол", "Шмель земляной",
        "Степная гадюка", "Прыткая ящерица", "Желтобрюхий полоз", "Тарантул",
        "Каракурт", "Ковыль", "Типчак", "Полынь", "Клевер", "Ромашка", "Василёк", "Мак"
    })
    {
        Pollution.Add(PollutionType.Air);
        Pollution.Add(PollutionType.Soil);
    }
}
