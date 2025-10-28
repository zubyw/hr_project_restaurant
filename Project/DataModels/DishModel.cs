public class DishModel
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }


    public DishModel(Int64 id, string email, string password, string fullname)
    {
        Id = id;
        EmailAddress = email;
        Password = password;
        FullName = fullname;
    }
}

