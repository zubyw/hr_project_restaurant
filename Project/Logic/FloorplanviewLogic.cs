using System;

namespace Project.Logic
{
    public static class FloorPlanLogic
    {
        public static (int newRow, int newCol, TableModel? selected) HandleKey(ConsoleKey key, TableModel[][] floorPlan, List<int> reservedTableIds, int guestCount, int row, int col)
        {
            int newRow = row;
            int newCol = col;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (col > 0)
                        newCol--;
                    break;

                case ConsoleKey.RightArrow:
                    if (col < floorPlan[row].Length - 1)
                        newCol++;
                    break;

                case ConsoleKey.UpArrow:
                    if (row > 0 && col < floorPlan[row - 1].Length)
                        newRow--;
                    break;

                case ConsoleKey.DownArrow:
                    if (row < floorPlan.Length - 1 &&
                        col < floorPlan[row + 1].Length)
                        newRow++;
                    break;

                case ConsoleKey.Enter:
                    var table = floorPlan[row][col];
                    if (IsSelectable(table, reservedTableIds, guestCount))
                        return (row, col, table);
                    break;
            }

            return (newRow, newCol, null);
        }

        private static bool IsSelectable(TableModel table, List<int> reservedTableIds, int guestCount)
        {
            return IsRightSize(table, guestCount) && !reservedTableIds.Contains(table.ID);
        }

        private static bool IsRightSize(TableModel table, int guestCount)
        {
            if (guestCount <= 2) return table.TableCapacity == 2;
            if (guestCount <= 4) return table.TableCapacity == 4;
            return table.TableCapacity == 6;
        }

        public static TableModel[][] BuildFloorPlan(List<TableModel> allTables)
        {
            var t = allTables.ToList();

            return new[]
            {
                new[] { t[8], t[0], t[5], t[1] },           
                new[] { t[3], t[6], t[11], t[9], t[10], t[7] },
                new[] { t[4], t[13], t[2], t[12] }
            };
        }
        public static bool HasAnySelectableTable(TableModel[][] floorPlan, List<int> reservedTableIds, int guestCount)
        {
            for (int r = 0; r < floorPlan.Length; r++)
                for (int c = 0; c < floorPlan[r].Length; c++)
                    if (IsSelectable(floorPlan[r][c], reservedTableIds, guestCount))
                        return true;

            return false;
        }
    }
}
