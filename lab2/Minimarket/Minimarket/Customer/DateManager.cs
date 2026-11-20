namespace Minimarket
{
    public class DateManager
    {
        private static DateOnly minDate = new DateOnly(2010, 1, 1);
        private static DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);
        public static DateOnly GenerateRandomDate()
        {
            int daysPassed = Random.Shared.Next(0, 6167);
            DateOnly date = minDate.AddDays(daysPassed);
            if (date > todayDate)
            {
                throw new Exception("Invalid date!");
            }
            return date;
        }
    }
}
