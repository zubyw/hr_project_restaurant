using System;

public static class ColorConsole
{
    // Color methods for different purposes
    public static void WriteTitle(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteSuccess(string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteError(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteWarning(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteInfo(string text)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteHighlight(string text)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteMenuOption(string text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WritePrompt(string text)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteLogo(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void WriteSubtitle(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void ClearScreen()
    {
        Console.Clear();
    }
}
