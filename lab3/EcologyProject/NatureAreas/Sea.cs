namespace EcologyProject;

public class Sea : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Sea;

    public Sea(double size, string name) : base(size, name, new[]
    {
        "Дельфин-белобочка", "Синий кит", "Косатка", "Тюлень", "Морж", "Альбатрос",
        "Чайка", "Баклан", "Пингвин", "Буревестник", "Акула", "Тунец", "Треска",
        "Камбала", "Скумбрия", "Морской конёк", "Зелёная морская черепаха",
        "Кожистая черепаха", "Краб камчатский", "Омар", "Креветка", "Лангуст",
        "Осьминог", "Кальмар", "Мидия", "Устрица", "Морской гребешок", "Медуза",
        "Актиния", "Коралл", "Ламинария", "Фукус", "Ульва", "Зостера"
    })
    {
        Pollution.Add(PollutionType.Water);
    }
}
