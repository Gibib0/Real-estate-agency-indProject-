using System;

namespace RealEstateAgency
{
    public static class ConsoleHelpers
    {
        public static string GetString(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine();
        }

        public static int GetInt(string prompt)
        {
            int value;
            while (true)
            {
                var input = GetString(prompt);
                if (int.TryParse(input, out value))
                {
                    return value;
                }
                Console.WriteLine("Wrong format. Enter integer number.");
            }
        }

        public static decimal GetDecimal(string prompt)
        {
            decimal value;
            while (true)
            {
                var input = GetString(prompt);
                if (decimal.TryParse(input, out value))
                {
                    return value;
                }
                Console.WriteLine("Wrong format. Enter number (for example, 50000.5).");
            }
        }

        public static decimal? GetNullableDecimal(string prompt)
        {
            while (true)
            {
                var input = GetString(prompt);
                if (string.IsNullOrWhiteSpace(input))
                {
                    return null;
                }
                if (decimal.TryParse(input, out decimal value))
                {
                    return value;
                }
                Console.WriteLine("Wrong format. Enter a number or press Enter to skip.");
            }
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}