public class Drink : IIdentifier
{
    public int ID { get; set; }
    public string Name { get; set; }
    public double AlcoholPercentage { get; set; }
    public decimal Price { get; set; }

    public Drink() 
    {
    }

    public Drink(int id, string name, double alcoholPercentage, decimal price)
    {
        ID = id;
        Name = name;
        AlcoholPercentage = alcoholPercentage;
        Price = price;
    }
}
