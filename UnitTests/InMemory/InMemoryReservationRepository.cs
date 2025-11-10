using Project.DataModels;

namespace UnitTests.InMemory;

public class InMemoryReservationRepository
{
    private readonly List<ReservationModel> _reservations = new();
    private int _nextId = 1;

    public void Add(ReservationModel reservation)
    {
        if (reservation.ID == 0)
        {
            reservation.ID = _nextId++;
        }
        else
        {
            if (reservation.ID >= _nextId)
            {
                _nextId = reservation.ID + 1;
            }
        }
        _reservations.Add(reservation);
    }

    public ReservationModel? GetById(int id)
    {
        return _reservations.FirstOrDefault(r => r.ID == id);
    }

    public List<ReservationModel> GetAll()
    {
        return _reservations.ToList();
    }

    public List<ReservationModel> GetByDate(string date)
    {
        return _reservations
            .Where(r => r.StartAt.StartsWith(date))
            .ToList();
    }

    public List<int> GetReservedTableIds(string date)
    {
        return _reservations
            .Where(r => r.StartAt.StartsWith(date))
            .Select(r => r.TableId)
            .ToList();
    }

    public void Update(ReservationModel reservation)
    {
        var index = _reservations.FindIndex(r => r.ID == reservation.ID);
        if (index >= 0)
        {
            _reservations[index] = reservation;
        }
    }

    public void Delete(int id)
    {
        var reservation = _reservations.FirstOrDefault(r => r.ID == id);
        if (reservation != null)
        {
            _reservations.Remove(reservation);
        }
    }

    public void Clear()
    {
        _reservations.Clear();
        _nextId = 1;
    }
}
