using System;
using System.Collections.Generic;
using System.Linq;
using Project.DataModels;
using Project.Logic;

namespace Project.Presentation
{
    public static class FloorPlanView
    {
        public static TableModel? SelectTableFromFloorPlan(List<TableModel> allTables, List<int> reservedTableIds, int guestCount)
        {
            TableModel[][] floorPlan = FloorPlanLogic.BuildFloorPlan(allTables);

            int row = 0;
            int col = 0;

            if (!IsSelectable(floorPlan[row][col], reservedTableIds, guestCount))
            {
                (row, col) = FindFirstSelectable(
                    floorPlan, reservedTableIds, guestCount);
            }

            while (true)
            {
                ShowReadOnlyFloorPlan(guestCount);

                DisplayFloorPlan(floorPlan, reservedTableIds, guestCount, row, col);

                Console.WriteLine();
                Console.WriteLine("[(2p)  ] Available | [(4p) X] Reserved | [(6p) -] Wrong size | [(2p) >] Selected");
                Console.WriteLine("Use ←/→/↑/↓ to move, Enter to select, Esc to cancel");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    return null;

                var result = FloorPlanLogic.HandleKey(key, floorPlan, reservedTableIds, guestCount, row, col);

                row = result.newRow;
                col = result.newCol;

                if (result.selected != null)
                    return result.selected;
            }
        }

        private static (int row, int col) FindFirstSelectable(TableModel[][] floorPlan, List<int> reservedTableIds, int guestCount)
        {
            for (int r = 0; r < floorPlan.Length; r++)
                for (int c = 0; c < floorPlan[r].Length; c++)
                    if (IsSelectable(floorPlan[r][c], reservedTableIds, guestCount)) return (r, c);

            return (0, 0);
        }

        private static bool IsSelectable(
            TableModel table,
            List<int> reservedTableIds,
            int guestCount)
        {
            return IsRightSize(table, guestCount) &&
                   !reservedTableIds.Contains(table.ID);
        }

        private static void ShowReadOnlyFloorPlan(int guestCount)
        {
            Console.Clear();
            Console.WriteLine($"Party Size: {guestCount}");
            Console.WriteLine();
        }

        private static void DisplayFloorPlan(TableModel[][] floorPlan, List<int> reservedTableIds, int guestCount, int selectedRow,int selectedCol)
        {
            for (int r = 0; r < floorPlan.Length; r++)
            {
                for (int c = 0; c < floorPlan[r].Length; c++)
                {
                    bool isSelected = r == selectedRow && c == selectedCol;
                    PrintTableSymbol(floorPlan[r][c], reservedTableIds, guestCount, isSelected);
                }
                Console.WriteLine();
            }
        }

        private static void PrintTableSymbol(TableModel table, List<int> reservedTableIds, int guestCount, bool isSelected)
        {
            string capacity = $"{table.TableCapacity}p";
            string status;

            if (!IsRightSize(table, guestCount))
            {
                status = "-";
                if (isSelected) Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (reservedTableIds.Contains(table.ID))
            {
                status = "X";
                if (isSelected) Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (isSelected)
            {
                status = ">";
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                status = " ";
            }

            Console.Write($"[({capacity}) {status}]  ");
            Console.ResetColor();
        }
        private static bool IsRightSize(TableModel table, int guestCount)
        {
            if (guestCount <= 2) return table.TableCapacity == 2;
            if (guestCount <= 4) return table.TableCapacity == 4;
            return table.TableCapacity == 6;
        }
    }
}
