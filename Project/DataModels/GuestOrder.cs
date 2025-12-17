public class GuestOrder
{
    public DishModel? MainDish { get; set; }
    public Drink? Drink { get; set; }

    public decimal TotalPrice
    {
        get
        {
            decimal total = 0;
            if (MainDish != null) total += MainDish.Price;
            if (Drink != null) total += Drink.Price;
            return total;
        }
    }
}
