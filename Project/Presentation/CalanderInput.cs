using System;
using Project.Logic;
public static class CalanderInput
{
    public static string Calander()
    {
        DateTime today = DateTime.Today;

        int year = today.Year;
        int month = today.Month;
        int day = today.Day;

        int level = 0;

        return RunCalander(today, ref year, ref month, ref day, ref level);
    }

    static string RunCalander(DateTime today, ref int year, ref int month, ref int day, ref int level)
    {
        Console.Clear();
        Console.WriteLine("Use arrows and ENTER to confirm date\n");

        Console.WriteLine((level == 0 ? "> " : "  ") + $"Jaar:  {year}");
        Console.WriteLine((level == 1 ? "> " : "  ") + $"Maand: {month}");
        Console.WriteLine((level == 2 ? "> " : "  ") + $"Dag:   {day}");

        var key = Console.ReadKey(true).Key;

        CalanderLogic.ChangeLevel(ref level, key);

        int oldYear = year;
        int oldMonth = month;

        if (key == ConsoleKey.LeftArrow) CalanderLogic.MoveLeft(ref year, ref month, ref day, level, today);
        if (key == ConsoleKey.RightArrow) CalanderLogic.MoveRight(ref year, ref month, ref day, level, today);

        CalanderLogic.GetMaximumDaysInMonth(ref day, year, month);

        if (key == ConsoleKey.Enter)
        {
            Console.Clear();
            Console.WriteLine($"Geselecteerde datum: {day}-{month}-{year}");
            Thread.Sleep(1500);
            return $"{day}-{month}-{year}";
        }

        return RunCalander(today, ref year, ref month, ref day, ref level);
    }
}
