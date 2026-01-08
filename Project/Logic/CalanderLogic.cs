namespace Project.Logic
{
    public class CalanderLogic
    {
        public static void MoveLeft(ref int year, ref int month, ref int day, int level, DateTime today)
        {
            if (level == 0) // year
            {
                if (year > today.Year + 1)
                {
                    year--;
                    month = 1;
                    day = 1;
                }
                else
                {
                    year = today.Year;
                    month = today.Month;
                    day = today.Day;
                }
            }
            else if (level == 1) // Month 
            {
                if (year == today.Year && month == today.Month) return;

                month--;
                if (year == today.Year && month <= today.Month)
                {
                    month = today.Month;
                    day = today.Day;
                }
                if (month < 1) month = 12;
            }
        else if (level == 2) // day
        {
            int newDay = day - 1;

            if (year == today.Year && month == today.Month && newDay < today.Day)
                newDay = today.Day;
            if (newDay < 1)
                newDay = DateTime.DaysInMonth(year, month);

            if (IsFutureDate(year, month, newDay, today))
                day = newDay;
        }

        }

        public static void MoveRight(ref int year, ref int month, ref int day, int level, DateTime today)
        {
            if (level == 0)
            {
                year++;
                month = 1;
                day = 1;
            }
            if (level == 1)
            {
                month++;

                if (month > 12) month = 1;

                if (year == today.Year && month < today.Month)
                    month = today.Month;
                if (year == today.Year && month == today.Month)
                    day = today.Day;
                else
                    day = 1;
            }
            if (level == 2)
            {
                int maxDay = DateTime.DaysInMonth(year, month);
                int newDay = day + 1;
                if (newDay > maxDay)
                {
                    newDay = 1;
                }
                if (IsFutureDate(year, month, newDay, today))
                {
                    day = newDay;
                }
            }
        }

        public static void ChangeLevel(ref int level, ConsoleKey key)
        {
            if (key == ConsoleKey.UpArrow) level = Math.Max(0, level - 1);
            if (key == ConsoleKey.DownArrow) level = Math.Min(2, level + 1);
        }

        public static void GetMaximumDaysInMonth(ref int day, int year, int month)
        {
            int maxday = DateTime.DaysInMonth(year, month);
            day = Math.Max(1, Math.Min(day, maxday));
        }

        public static bool IsFutureDate(int year, int month, int day, DateTime today)
        {
            return new DateTime(year, month, day) >= today;
        }
    }
}