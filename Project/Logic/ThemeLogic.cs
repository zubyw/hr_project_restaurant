using System;
using System.Collections.Generic;
using Project.DataAccess;

public class ThemesLogic
{
    private readonly ThemeAccess access = new ThemeAccess();

    private readonly DishAccess _dishaccess = new DishAccess();

    public List<ThemeModel> GetAll()
    {
        return access.GetAllThemes();
    }

    public ThemeModel GetById(int id)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        ThemeModel theme = access.GetById(id);
        if (theme == null)
        {
            throw new Exception("Not found");
        }

        return theme;
    }

    public void WriteTheme(ThemeModel theme)
    {
        access.Write(theme);
    }

    public void UpdateTheme(ThemeModel theme)
    {
        access.Update(theme);
    }


    public void Activate(int id)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        access.ActivateTheme(id);
    }

    public void Deactivate(int id)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        access.DeactivateTheme(id);
    }

    public void DeleteThemeCompletely(ThemeModel theme)
    {
        access.DeleteThemeCompletely(theme);
    }

    private bool Exists(string name)
    {
        List<ThemeModel> list = access.GetAllThemes();
        foreach (ThemeModel t in list)
        {
            if (t.Name != null && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private void EnsureValidThemeMonth(DateTime monthDate)
    {
        if (monthDate.Year < 2025)
        {
            throw new Exception("Theme year must be 2025 or later.");
        }

        DateTime firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
        DateTime now = DateTime.Today;
        DateTime firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1);

        if (firstDayOfMonth < firstDayOfCurrentMonth)
        {
            throw new Exception("Theme month cannot be in the past.");
        }
    }

    public Dictionary<string, int> GetAllActiveDatesAndThemes()
    {
        DateTime datetoday = DateTime.Today;
        return access.GetFutureThemesByMonth(datetoday);
    }

    public bool DoesThemeExist(string dishname)
        {
            return access.GetThemeByName(dishname);
        }
    
    public List<string> GetAvailableMonths()
    {
        List<string> timeslots = access.GetThemeCalendarTakenMonths();
        
        List<string> takenMonths = timeslots
            .Select(ts => DateTime.Parse(ts))
            .Where(d => d >= DateTime.Today)
            .OrderBy(d => d)
            .Select(d => d.ToString("MM-yyyy"))
            .ToList();

        List<string> allMonths = new List<string>();

        DateTime now = DateTime.Now;

        int year = now.Year;
        int month = now.Month + 1;
        if (month == 13)
        {
            month = 1;
            year++;
        }

        for (int i = 0; i < 36; i++)
        {
            allMonths.Add($"{month:00}-{year}");

            month++;
            if (month == 13)
            {
                month = 1;
                year++;
            }
        }
        
        List<string> availableMonths = allMonths
        .Where(m => !takenMonths.Contains(m))
        .ToList();

        return availableMonths;
    }

    public void LinkMonthsToTheme(List<string> months, ThemeModel theme)
    {
        if (months.Count() == 0) return;

        foreach (string m in months)
        {
            access.LinkMonthToTheme(m, theme);
        }
    }

    public List<DishModel> GetAllAvailableDishes(ThemeModel theme)
        {
            List<DishModel> dishesInTheme = _dishaccess.GetByTheme(theme);
            List<DishModel> allDishes = _dishaccess.GetAllDishes();

            List<DishModel> availableDishes = allDishes
            .Where(d => !dishesInTheme.Any(t => t.ID == d.ID))
            .ToList();
            return availableDishes;
        }

    public void AddDishesToTheme(List<DishModel> listdishes, ThemeModel theme)
    {
        if (listdishes is null) return;
        foreach (DishModel d in listdishes)
        {
            _dishaccess.LinkDishToTheme(d, theme);
        }
        return;
    }

    public List<DishModel> GetAllDishesInTheme(ThemeModel theme)
        {
            return _dishaccess.GetByTheme(theme);   
        }
    
    public void DeleteDishonTheme(DishModel dish, ThemeModel theme)
    {
        access.DeleteDishonTheme(theme, dish);
    }

}
