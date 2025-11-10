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
            { 1, (2, 3) },
            { 2, (2, 10) },
            { 3, (2, 17) },
            { 4, (2, 24) },
            { 5, (7, 3) },
            { 6, (7, 12) },
            { 7, (7, 21) },
            { 8, (7, 30) },
            { 9, (7, 39) },
            { 10, (7, 48) },
            { 11, (12, 3) },
            { 12, (12, 14) },
            { 13, (12, 25) },
            { 14, (12, 36) }
        };

        public static TableModel? SelectTableFromFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            // Start with the first selectable table
            var firstSelectable = allTables
                .OrderBy(t => t.TableNumber)
                .FirstOrDefault(t => IsTableSelectable(t, reservedTableIds, guestCount));
            int selectedTableNumber = firstSelectable?.TableNumber ?? 0;
            ConsoleKey key;

            do
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

                DisplayFloorPlan(allTables, reservedTableIds, guestCount, selectedTableNumber);
                
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow || 
                    key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
                {
                    selectedTableNumber = NavigateFloorPlan(selectedTableNumber, key, allTables, reservedTableIds, guestCount);
                }
                else if (key == ConsoleKey.Enter && selectedTableNumber > 0)
                {
                    var selectedTable = allTables.FirstOrDefault(t => t.TableNumber == selectedTableNumber);
                    if (selectedTable != null && IsTableSelectable(selectedTable, reservedTableIds, guestCount))
                    {
                        return selectedTable;
                    }
                    else if (selectedTable != null)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        ColorConsole.WriteError("Cannot select this table - it's either reserved or wrong size!");
                        System.Threading.Thread.Sleep(1500);
                    }
                }

            } while (key != ConsoleKey.Escape);

            return null;
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

        private static void DisplayFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount, int selectedTableNumber)
        {
            const int floorWidth = 60;
            const int floorHeight = 25;

            string[,] grid = new string[floorHeight, floorWidth];
            ConsoleColor[,] colorGrid = new ConsoleColor[floorHeight, floorWidth];

            for (int i = 0; i < floorHeight; i++)
            {
                for (int j = 0; j < floorWidth; j++)
                {
                    grid[i, j] = " ";
                    colorGrid[i, j] = ConsoleColor.Black;
                }
            }

            foreach (var table in allTables)
            {
                if (TablePositions.ContainsKey(table.TableNumber))
                {
                    var (row, col) = TablePositions[table.TableNumber];
                    
                    int boxWidth = table.TableCapacity + 2;
                    int boxHeight = 3;
                    
                    if (row >= 0 && row < floorHeight - boxHeight && col >= 0 && col < floorWidth - boxWidth)
                    {
                        ConsoleColor tableColor = GetTableColor(table, reservedTableIds, guestCount, selectedTableNumber);
                        string capacity = $"{table.TableCapacity}p";

                        grid[row, col] = "┌";
                        for (int i = 1; i < boxWidth - 1; i++)
                        {
                            grid[row, col + i] = "─";
                        }
                        grid[row, col + boxWidth - 1] = "┐";

                        grid[row + 1, col] = "│";
                        int paddingLeft = (boxWidth - 2 - capacity.Length) / 2;
                        for (int i = 1; i < boxWidth - 1; i++)
                        {
                            if (i == paddingLeft + 1 && capacity.Length >= 2)
                            {
                                grid[row + 1, col + i] = capacity[0].ToString();
                            }
                            else if (i == paddingLeft + 2 && capacity.Length >= 2)
                            {
                                grid[row + 1, col + i] = capacity[1].ToString();
                            }
                            else
                            {
                                grid[row + 1, col + i] = " ";
                            }
                        }
                        grid[row + 1, col + boxWidth - 1] = "│";

                        grid[row + 2, col] = "└";
                        for (int i = 1; i < boxWidth - 1; i++)
                        {
                            grid[row + 2, col + i] = "─";
                        }
                        grid[row + 2, col + boxWidth - 1] = "┘";

                        for (int i = 0; i < boxHeight; i++)
                        {
                            for (int j = 0; j < boxWidth; j++)
                            {
                                colorGrid[row + i, col + j] = tableColor;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < floorHeight; i++)
            {
                Console.Write("  ");
                for (int j = 0; j < floorWidth; j++)
                {
                    Console.ForegroundColor = colorGrid[i, j];
                    Console.Write(grid[i, j]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private static ConsoleColor GetTableColor(TableModel table, List<int> reservedTableIds, int guestCount, int selectedTableNumber)
        {
            bool isReserved = reservedTableIds.Contains(table.ID);
            // Round up guest count to table size: 1-2 -> 2, 3-4 -> 4, 5-6 -> 6
            int requiredTableSize = guestCount <= 2 ? 2 : guestCount <= 4 ? 4 : 6;
            bool isRightSize = table.TableCapacity == requiredTableSize;
            bool isSelected = table.TableNumber == selectedTableNumber;

            if (isSelected && isRightSize && !isReserved)
            {
                return ConsoleColor.Cyan;
            }
            else if (isReserved)
            {
                return ConsoleColor.Red;
            }
            else if (!isRightSize)
            {
                return ConsoleColor.DarkGray;
            }
            else
            {
                return ConsoleColor.White;
            }
        }

        private static bool IsTableSelectable(TableModel table, List<int> reservedTableIds, int guestCount)
        {
            bool isReserved = reservedTableIds.Contains(table.ID);
            // Round up guest count to table size: 1-2 -> 2, 3-4 -> 4, 5-6 -> 6
            int requiredTableSize = guestCount <= 2 ? 2 : guestCount <= 4 ? 4 : 6;
            bool isRightSize = table.TableCapacity == requiredTableSize;
            
            return !isReserved && isRightSize;
        }

        private static int NavigateFloorPlan(int currentTableNumber, ConsoleKey key, List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            if (currentTableNumber == 0)
            {
                var firstSelectable = allTables
                    .OrderBy(t => t.TableNumber)
                    .FirstOrDefault(t => IsTableSelectable(t, reservedTableIds, guestCount));
                return firstSelectable?.TableNumber ?? allTables.First().TableNumber;
            }

            var currentPos = TablePositions.ContainsKey(currentTableNumber) 
                ? TablePositions[currentTableNumber] 
                : (0, 0);

            int targetRow = currentPos.Item1;
            int targetCol = currentPos.Item2;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    targetRow -= 5;
                    break;
                case ConsoleKey.DownArrow:
                    targetRow += 5;
                    break;
                case ConsoleKey.LeftArrow:
                    targetCol -= 10;
                    break;
                case ConsoleKey.RightArrow:
                    targetCol += 10;
                    break;
            }

            // Get selectable tables only
            var selectableTables = allTables
                .Where(t => IsTableSelectable(t, reservedTableIds, guestCount))
                .Select(t => t.TableNumber)
                .ToList();

            if (!selectableTables.Any())
            {
                return currentTableNumber;
            }

            var closestTable = TablePositions
                .Where(kvp => selectableTables.Contains(kvp.Key)) // Only consider selectable tables
                .OrderBy(kvp => Math.Abs(kvp.Value.Item1 - targetRow) + Math.Abs(kvp.Value.Item2 - targetCol))
                .Where(kvp => key switch
                {
                    ConsoleKey.UpArrow => kvp.Value.Item1 < currentPos.Item1,
                    ConsoleKey.DownArrow => kvp.Value.Item1 > currentPos.Item1,
                    ConsoleKey.LeftArrow => kvp.Value.Item2 < currentPos.Item2,
                    ConsoleKey.RightArrow => kvp.Value.Item2 > currentPos.Item2,
                    _ => true
                })
                .Select(kvp => kvp.Key)
                .FirstOrDefault();

            if (closestTable != 0)
            {
                return closestTable;
            }

            return currentTableNumber;
        }
    }
}
