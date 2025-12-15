using System;
using System.Collections.Generic;
using System.Linq;
using Project.DataModels;

namespace Project.Presentation
{
    public static class FloorPlanView
    {
        private static readonly Dictionary<int, (int Row, int Col)> TablePositions = new Dictionary<int, (int, int)>
        {
            { 1, (2, 3) }, { 2, (2, 10) }, { 3, (2, 17) }, { 4, (2, 24) },
            { 5, (7, 3) }, { 6, (7, 12) }, { 7, (7, 21) }, { 8, (7, 30) },
            { 9, (7, 39) }, { 10, (7, 48) }, { 11, (12, 3) }, { 12, (12, 14) },
            { 13, (12, 25) }, { 14, (12, 36) }
        };

        /// <summary>
        /// Shows a read-only floorplan overview.
        /// </summary>
        public static void ShowReadOnlyFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            Console.Clear();
            Console.WriteLine();
            ColorConsole.WriteTitle("╔═══════════════════════════════════════════════╗");
            ColorConsole.WriteTitle("║         RESTAURANT FLOOR PLAN                 ║");
            ColorConsole.WriteTitle("╚═══════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Party Size: {guestCount} guests");
            Console.WriteLine();

            DisplayLegend();
            Console.WriteLine();
            DisplayFloorPlan(allTables, reservedTableIds, guestCount, -1); // -1 = geen selectie
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        /// <summary>
        /// Lets the user select a table (skips reserved and wrong-size tables)
        /// </summary>
        public static TableModel? SelectTableFromFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            int selectedIndex = allTables.FindIndex(t => !reservedTableIds.Contains(t.ID) && IsRightSize(t, guestCount));
            if (selectedIndex == -1) return null;

            bool selecting = true;
            while (selecting)
            {
                Console.Clear();
                Console.WriteLine();
                ColorConsole.WriteTitle("╔═══════════════════════════════════════════════╗");
                ColorConsole.WriteTitle("║         RESTAURANT FLOOR PLAN                 ║");
                ColorConsole.WriteTitle("╚═══════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine($"  Party Size: {guestCount} guests");
                Console.WriteLine();

                DisplayLegend();
                Console.WriteLine();

                DisplayFloorPlan(allTables, reservedTableIds, guestCount, selectedIndex);
                Console.WriteLine("\nUse ↑/↓ to navigate, Enter to select, Esc to cancel.");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        selectedIndex = FindPreviousSelectable(allTables, reservedTableIds, guestCount, selectedIndex);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        selectedIndex = FindNextSelectable(allTables, reservedTableIds, guestCount, selectedIndex);
                        break;
                    case ConsoleKey.Enter:
                        selecting = false;
                        break;
                    case ConsoleKey.Escape:
                        return null;
                }
            }

            return allTables[selectedIndex];
        }

        private static int FindNextSelectable(List<TableModel> allTables, List<int> reserved, int guestCount, int currentIndex)
        {
            int start = currentIndex;
            do
            {
                currentIndex = (currentIndex + 1) % allTables.Count;
            } while ((reserved.Contains(allTables[currentIndex].ID) || !IsRightSize(allTables[currentIndex], guestCount)) && currentIndex != start);
            return currentIndex;
        }

        private static int FindPreviousSelectable(List<TableModel> allTables, List<int> reserved, int guestCount, int currentIndex)
        {
            int start = currentIndex;
            do
            {
                currentIndex = (currentIndex - 1 + allTables.Count) % allTables.Count;
            } while ((reserved.Contains(allTables[currentIndex].ID) || !IsRightSize(allTables[currentIndex], guestCount)) && currentIndex != start);
            return currentIndex;
        }

        private static bool IsRightSize(TableModel table, int guestCount)
        {
            int requiredTableSize = guestCount <= 2 ? 2 : guestCount <= 4 ? 4 : 6;
            return table.TableCapacity == requiredTableSize;
        }

        private static void DisplayLegend()
        {
            Console.Write("  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("█ Available  ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("█ Wrong Size  ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("█ Reserved  ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("█ Selected");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DisplayFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount, int selectedIndex)
        {
            const int floorWidth = 60;
            const int floorHeight = 25;

            string[,] grid = new string[floorHeight, floorWidth];
            ConsoleColor[,] colorGrid = new ConsoleColor[floorHeight, floorWidth];

            for (int i = 0; i < floorHeight; i++)
                for (int j = 0; j < floorWidth; j++)
                {
                    grid[i, j] = " ";
                    colorGrid[i, j] = ConsoleColor.Black;
                }

            for (int i = 0; i < allTables.Count; i++)
            {
                var table = allTables[i];
                if (!TablePositions.ContainsKey(table.TableNumber)) continue;
                var (row, col) = TablePositions[table.TableNumber];
                int boxWidth = table.TableCapacity + 2;
                int boxHeight = 3;
                if (row < 0 || row >= floorHeight - boxHeight || col < 0 || col >= floorWidth - boxWidth) continue;

                ConsoleColor tableColor = reservedTableIds.Contains(table.ID) ? ConsoleColor.Red :
                                          !IsRightSize(table, guestCount) ? ConsoleColor.DarkGray :
                                          (i == selectedIndex ? ConsoleColor.Cyan : ConsoleColor.White);

                string capacity = $"{table.TableCapacity}p";

                grid[row, col] = "┌";
                for (int x = 1; x < boxWidth - 1; x++) grid[row, col + x] = "─";
                grid[row, col + boxWidth - 1] = "┐";

                grid[row + 1, col] = "│";
                int pad = (boxWidth - 2 - capacity.Length) / 2;
                for (int x = 1; x < boxWidth - 1; x++)
                    grid[row + 1, col + x] = (x == pad + 1 && capacity.Length >= 2) ? capacity[0].ToString() :
                                             (x == pad + 2 && capacity.Length >= 2) ? capacity[1].ToString() : " ";
                grid[row + 1, col + boxWidth - 1] = "│";

                grid[row + 2, col] = "└";
                for (int x = 1; x < boxWidth - 1; x++) grid[row + 2, col + x] = "─";
                grid[row + 2, col + boxWidth - 1] = "┘";

                for (int r = 0; r < boxHeight; r++)
                    for (int c = 0; c < boxWidth; c++)
                        colorGrid[row + r, col + c] = tableColor;
            }

            for (int r = 0; r < floorHeight; r++)
            {
                Console.Write("  ");
                for (int c = 0; c < floorWidth; c++)
                {
                    Console.ForegroundColor = colorGrid[r, c];
                    Console.Write(grid[r, c]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}
