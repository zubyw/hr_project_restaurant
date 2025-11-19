public class DishModel
{

    public int ID { get; set; }
    public int ThemeId { get; set; } 
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }

}