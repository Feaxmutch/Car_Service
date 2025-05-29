
namespace CarService
{
    public static class UserUtilits
    {
        private static Random _random = new();

        public static bool GetRandomBoolean(float trueChance)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(trueChance);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(trueChance, 1);

            return _random.NextSingle() <= trueChance;
        }

        public static T GetRandomObject<T>(List<T> objects)
        {
            ArgumentNullException.ThrowIfNull(objects);

            return objects[_random.Next(0, objects.Count)];
        }

        public static void WriteWithColor(string mesage, ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(mesage);
            Console.ForegroundColor = defaultColor;
        }

        public static float ConvertToPercent(float multiplyer)
        {
            return multiplyer * 100;
        }

        public static float Round(float number, int decimals)
        {
            return Convert.ToSingle(Math.Round(Convert.ToDecimal(number), decimals));
        }

        public static void HandleIndexExeptions<T>(IEnumerable<T> collection, int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, collection.Count());
        }
    }
}
