namespace EcologyProject;

public class Lake : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Lake;

    public Lake(double size, string name) : base(size, name, new[]
    {
        "Бобр", "Выдра", "Ондатра", "Водяная полёвка", "Утка-кряква", "Лебедь-шипун",
        "Серая цапля", "Поганка", "Камышница", "Щука", "Карась", "Окунь", "Плотва",
        "Лещ", "Линь", "Карп", "Судак", "Озёрная лягушка", "Прудовая лягушка",
        "Тритон", "Болотная черепаха", "Уж обыкновенный", "Речной рак", "Прудовик",
        "Беззубка", "Катушка", "Стрекоза", "Водомерка", "Жук-плавунец", "Комар",
        "Кувшинка", "Кубышка", "Ряска", "Камыш", "Тростник", "Рогоз", "Спирогира", "Хлорелла"
    })
    {
        Pollution.Add(PollutionType.Water);
        Pollution.Add(PollutionType.Soil);
    }
}
