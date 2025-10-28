public class DishModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }


    public DishModel(Int64 id, string email, string password, string fullname)
    public DishModel(string n, decimal p, string d, string t)
    {
        Name = n;
        Price = p;
        Description = d;
        Type = t;
    }


}