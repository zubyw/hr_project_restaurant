using System;
using System.Collections.Generic;

public class ThemesLogic
{
    private readonly ThemeAccess _access = new ThemeAccess();

    public List<ThemeModel> GetAll()
    {
        return _access.GetAllThemes();
    }

    public ThemeModel GetById(int id)
    {
        if (id <= 0)
        {
            throw new Exception("Invalid ID.");
        }

        ThemeModel theme = _access.GetById(id);
        if (theme == null)
        {
            throw new Exception("Theme not found.");
        }

        return theme;
    }

    public int? GetActiveThemeId()
    {
        return _access.GetActiveThemeID();
    }
}
