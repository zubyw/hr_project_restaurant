using System;
using System.Collections.Generic;

public class ThemesLogic
{
    private readonly ThemeAccess access = new ThemeAccess();
    private static readonly string[] AllowedCourses = { "Starter", "Main", "Dessert" };

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

        if (!IsValidCourse(course))
        {
            throw new Exception("Invalid course");
        }

        if (Exists(name))
        {
            throw new Exception("Theme exists");
        }

        ThemeModel theme = new ThemeModel();
        theme.Name = name;
        theme.Course = course;
        access.AddTheme(theme, timeSlot);
    }

    public void UpdateTheme(int id, string name, string course, int active)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid id");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("Name empty");
        }

        if (!IsValidCourse(course))
        {
            throw new Exception("Invalid course");
        }

        ThemeModel theme = access.GetById(id);
        if (theme == null)
        {
            throw new Exception("Not found");
        }

        theme.Name = name;
        theme.Course = course;
        theme.IsActive = active;
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

    private bool IsValidCourse(string course)
    {
        foreach (string c in AllowedCourses)
        {
            if (c.Equals(course, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
