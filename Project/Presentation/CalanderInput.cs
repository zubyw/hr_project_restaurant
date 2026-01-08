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
        string leftArrowYear = year > today.Year ? "< " : "  ";
        string leftArrowMonth = (month == today.Month) &&  (year == today.Year) ? "  " : "< ";
        string leftArrowDay = (day == today.Day) && (year == today.Year) && (month == today.Month) ? "  " : "< ";
        string rightArrow = " >";

        Console.Clear();
        Console.WriteLine("Use arrows ←/→ to decrease/increase | ↑/↓ for changing between Year/Month/Day \nENTER to confirm date\n");

        Console.WriteLine((level == 0 ? leftArrowYear : " ") + $"Year:  {year}" + (level == 0 ? rightArrow : " "));
        Console.WriteLine((level == 1 ? leftArrowMonth : " ") + $"Month:  {month}" + (level == 1 ? rightArrow : " "));
        Console.WriteLine((level == 2 ? leftArrowDay : " ") + $"Day:  {day}" + (level == 2 ? rightArrow : " "));

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
            Console.WriteLine($"Selected date: {day:00}-{month:00}-{year}");
            Thread.Sleep(1500);
            return $"{day:00}-{month:00}-{year}";
        }

        return RunCalander(today, ref year, ref month, ref day, ref level);
    }
}
