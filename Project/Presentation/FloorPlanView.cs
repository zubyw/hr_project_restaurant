using System;
using System.Collections.Generic;
using Project.DataModels;

namespace Project.Presentation
{
    public static class FloorPlanView
    {
        public static void ShowReadOnlyFloorPlan(int guestCount)
        {
            Console.Clear();
            Console.WriteLine();
            ColorConsole.WriteTitle("╔═══════════════════════════════════════════════╗");
            ColorConsole.WriteTitle("║         RESTAURANT FLOOR PLAN                 ║");
            ColorConsole.WriteTitle("╚═══════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Party Size: {guestCount} guests");
            Console.WriteLine();

        }

        public static TableModel? SelectTableFromFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            int selectedIndex = FindFirstSelectableTableIndex(allTables, reservedTableIds, guestCount);

            bool chosen = false;

            while (!chosen)
            {
                ShowReadOnlyFloorPlan(guestCount);               
                Console.WriteLine();

                DisplayFloorPlan(allTables, reservedTableIds, guestCount, selectedIndex);

                Console.WriteLine();
                DisplayLegend();
                Console.WriteLine("Use ↑/↓ to move, Enter to select, Esc to cancel");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = FindPreviousSelectable(allTables, selectedIndex);
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = FindNextSelectable(allTables, selectedIndex);
                else if (key == ConsoleKey.Enter)
                {
                    var table = allTables[selectedIndex];
                    if (!reservedTableIds.Contains(table.ID) && IsRightSize(table, guestCount))
                        chosen = true;
                }
                else if (key == ConsoleKey.Escape)
                    return null;
            }

            return allTables[selectedIndex];
        }


        private static int FindFirstSelectableTableIndex(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            for (int i = 0; i < allTables.Count; i++)
            {
                if (IsRightSize(allTables[i], guestCount) && !reservedTableIds.Contains(allTables[i].ID))
                {
                    return i;
                }
            }
            return 0;
        }

        private static int FindPreviousSelectable(List<TableModel> allTables, int currentIndex)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = allTables.Count - 1;
            return currentIndex;
        }

        private static int FindNextSelectable(List<TableModel> allTables, int currentIndex)
        {
            currentIndex++;
            if (currentIndex >= allTables.Count) currentIndex = 0;
            return currentIndex;
        }

        private static void DisplayFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount, int selectedIndex)
        {
            PrintRow(allTables, reservedTableIds, 2, guestCount, selectedIndex);
            PrintRow(allTables, reservedTableIds, 4, guestCount, selectedIndex);
            PrintRow(allTables, reservedTableIds, 6, guestCount, selectedIndex);
        }

        private static void PrintRow(List<TableModel> allTables, List<int> reservedTableIds, int size, int guestCount, int selectedIndex)
        {
            Console.Write("");
            for (int i = 0; i < allTables.Count; i++)
            {
                var table = allTables[i];
                if (table.TableCapacity != size) continue;

                bool isSelected = i == selectedIndex;
                PrintTableSymbol(table, reservedTableIds, guestCount, isSelected);
            }
            Console.WriteLine();
        }

        private static void PrintTableSymbol(TableModel table, List<int> reservedTableIds, int guestCount, bool isSelected)
        {
            if (!IsRightSize(table, guestCount))
            {
                if (isSelected) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[-]  ");
            }
            else if (reservedTableIds.Contains(table.ID))
            {
                if (isSelected) Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[X]  ");
            }
            else
            {
                if (isSelected) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[>]  ");
            }

            Console.ResetColor();
        }


        private static bool IsRightSize(TableModel table, int guestCount)
        {
            if (guestCount <= 2) return table.TableCapacity == 2;
            if (guestCount <= 4) return table.TableCapacity == 4;
            return table.TableCapacity == 6;
        }

        private static void DisplayLegend()
        {
            Console.WriteLine("[ ] Available  [X] Reserved  [-] Wrong size  [>] Selected");
        }
    }
}
