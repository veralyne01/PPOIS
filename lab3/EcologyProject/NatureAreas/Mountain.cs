namespace EcologyProject;

public class Mountain : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Mountain;

    public Mountain(double size, string name) : base(size, name, new[]
    {
        "Бурый медведь", "Волк", "Лиса", "Рысь", "Благородный олень", "Горный козёл",
        "Снежный барс", "Сурок альпийский", "Архар", "Серна", "Як", "Снежная полёвка",
        "Беркут", "Бородач", "Альпийская галка", "Улар", "Горный шмель",
        "Альпийская бабочка", "Горная гадюка", "Скальная ящерица", "Альпийский тритон",
        "Саламандра", "Эдельвейс", "Горечавка", "Альпийский мак", "Горная сосна",
        "Можжевельник", "Мхи", "Лишайники", "Форель"
    })
    {
        Pollution.Add(PollutionType.Air);
        Pollution.Add(PollutionType.Soil);
    }
}
