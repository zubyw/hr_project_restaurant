public class Drink
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double AlcoholPercentage { get; set; }
    public decimal Price { get; set; }

    public Drink() 
    {
    }

    public Drink(int id, string name, double alcoholPercentage, decimal price)
    {
        Id = id;
        Name = name;
        AlcoholPercentage = alcoholPercentage;
        Price = price;
    }
}
