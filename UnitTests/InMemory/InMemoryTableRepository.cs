using Project.DataModels;

namespace UnitTests.InMemory;

public class InMemoryTableRepository
{
    private readonly List<TableModel> _tables = new();
    private int _nextId = 1;

    public void Add(TableModel table)
    {
        if (table.ID == 0)
        {
            table.ID = _nextId++;
        }
        else
        {
            if (table.ID >= _nextId)
            {
                _nextId = table.ID + 1;
            }
        }
        _tables.Add(table);
    }

    public List<TableModel> GetAll()
    {
        return _tables.ToList();
    }

    public TableModel? GetById(int id)
    {
        return _tables.FirstOrDefault(t => t.ID == id);
    }

    public TableModel? GetByTableNumber(int tableNumber)
    {
        return _tables.FirstOrDefault(t => t.TableNumber == tableNumber);
    }

    public void Clear()
    {
        _tables.Clear();
        _nextId = 1;
    }
}
