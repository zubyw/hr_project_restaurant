using System;
using System.Collections.Generic;

public class ThemesLogic
{
    private readonly ThemeAccess access = new ThemeAccess();

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

    public void CreateTheme(string name, string course, DateTime timeSlot)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Name empty");
        }

        EnsureValidThemeMonth(timeSlot);

        if (Exists(name))
        {
            throw new Exception("Theme exists");
        }

        ThemeModel theme = new ThemeModel
        {
            Name = name,
            Course = course,
            IsActive = 1
        };

        access.AddTheme(theme, timeSlot);
    }

    public void UpdateTheme(int id, string name, string course, int active, DateTime timeSlot)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Name empty");
        }


        EnsureValidThemeMonth(timeSlot);


        ThemeModel existing = access.GetById(id);
        if (existing == null)
        {
            throw new Exception("Not found");
        }

        existing.Name = name;
        existing.Course = course;
        existing.IsActive = active;

        access.Update(existing);
        access.UpdateThemeCalendar(id, existing.Name, existing.Course, timeSlot);
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

    public void DeleteThemeCompletely(int id)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        access.DeleteThemeCompletely(id);
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
}
