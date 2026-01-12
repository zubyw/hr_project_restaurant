public static class UserMakeReservationLogic
{
    public static int MinPeople { get; } = 1;
    public static int MaxPeople { get; } = 6;
    public static string DateFormat { get; } = "dd-MM-yyyy"; // Europees formaat

    public static TableAccess _TableAcces = new TableAccess();
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
}
