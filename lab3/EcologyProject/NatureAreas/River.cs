namespace EcologyProject;

public class River : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.River;

    public River(double size, string name) : base(size, name, new[]
    {
        "Бобр", "Выдра", "Ондатра", "Водяная полёвка", "Зимородок", "Серая цапля",
        "Утка-кряква", "Щука", "Окунь", "Голавль", "Язь", "Жерех", "Форель", "Сом",
        "Судак", "Озёрная лягушка", "Лягушка травяная", "Тритон", "Уж обыкновенный",
        "Болотная черепаха", "Речной рак", "Беззубка", "Перловица", "Катушка",
        "Подёнка", "Ручейник", "Стрекоза", "Водомерка", "Камыш", "Тростник", "Рогоз",
        "Спирогира"
    })
    {
        Pollution.Add(PollutionType.Water);
        Pollution.Add(PollutionType.Soil);
    }
}
