using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public static class UserMakeReservationLogic
{
    public static int MinPeople { get; } = 1;
    public static int MaxPeople { get; } = 6;
    public static string DateFormat { get; } = "dd-MM-yyyy"; // Europees formaat

    public static TableAcces _TableAcces = new TableAcces();

    public static bool CheckValidDate(string date)
    {
        // Check if date has 10 chars: dd-MM-yyyy
        if (date.Length != 10)
        {
            return false;
        }

        // Split op '-' -> dd-MM-yyyy
        List<string> parts = date.Split('-').ToList();
        if (parts.Count != 3)
        {
            return false;
        }

        // Dag
        if (!int.TryParse(parts[0], out int day))
        {
            return false;
        }

        // Maand
        if (!int.TryParse(parts[1], out int month))
        {
            return false;
        }

        // Jaar
        if (!int.TryParse(parts[2], out int year))
        {
            return false;
        }

        // Controleer of het een geldige datum is
        DateTime validDate;
        try
        {
            validDate = new DateTime(year, month, day);
        }
        catch
        {
            return false;
        }

        // Controleer of de datum niet in het verleden ligt
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
            if (intAmountPeople >= 1 && intAmountPeople <= 6)
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
        return 2;
    }

    public static TableModel? GetAvailableTable(string reservationDate, int tablesize)
    {
        List<TableModel> allTables = _TableAcces.GetAllTables();

        List<int> reservedTableIds = _TableAcces.GetNonAvailableOnDate(reservationDate, tablesize);

        List<TableModel> availableTables = allTables.Where(t => !reservedTableIds.Contains(t.ID)).ToList();

        // Handle the case where no tables are available
        if (!availableTables.Any())
        {
            Console.WriteLine("No available tables found.");
            return null;
        }

        // Find a table that fits the tablesize
        return availableTables.FirstOrDefault(t => t.TableCapacity >= tablesize);
    }
}

