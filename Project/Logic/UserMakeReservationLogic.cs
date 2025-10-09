public static class UserMakeReservationLogic
{
    public static int MinPeople { get; } = 1;
    public static int MaxPeople { get; } = 6;
    public static string DateFormat { get; } = "yyyy-MM-dd";

    public static bool CheckValidDate(string date)
    {
        // Check if date has 10 chars
        if (date.Length != 10)
        {
            return false;
        }

        // Check if this format is used ...-...-...
        List<string> parts = date.Split('-').ToList();
        if (parts.Count != 3)
        {
            return false;
        }

        // Check if there are numbers given inbetween the '-'
        if (!int.TryParse(parts[0], out int year))
        {
            return false;
        }
        if (!int.TryParse(parts[1], out int month))
        {
            return false;
        }
        if (!int.TryParse(parts[2], out int day))
        {
            return false;
        }

        // check if it all given numbers is an valid date
        DateTime validDate;
        try
        {
            validDate = new DateTime(year, month, day);
        }
        catch
        {
            return false;
        }

        // Check if reservation is not in the past 
        if (validDate < DateTime.Today)
        {
            return false;
        }
        return true;
    }

    public static bool CheckAmountPeople(string AmountPeople)
    {
        if (int.TryParse(AmountPeople, out int intAmountPeople))
        {
            if (intAmountPeople > 1 && intAmountPeople < 6)
            {
                return true;
            }
        }
        return false;
    }

    // Check if the given daytime is valid.
    public static bool CheckValidDayTime(string daytime)
    {
        List<string> validTimes = new List<string> { "17:00", "17:30", "18:00", "18:30", "19:00", "19:30" };


        foreach (string validTime in validTimes)
        {
            if (validTime == daytime)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetTableSize(string amountpeople)
    {
        int.TryParse(amountpeople, out int intAmountPeople);
        if ((intAmountPeople == 1) || (intAmountPeople == 2))
        {
            return 2;
        }
        else if ((intAmountPeople == 3) || (intAmountPeople == 4))
        {
            return 4;
        }
        else if ((intAmountPeople == 5) || (intAmountPeople == 6))
        {
            return 6;
        }
        return 0;
    }
}