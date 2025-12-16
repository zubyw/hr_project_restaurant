public class DishModel
{

    public int ID { get; set; }
    public int ThemeId { get; set; } 
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    
    // Allergen properties
    public List<int> AllergenIds { get; set; } = new List<int>();
    public List<string> AllergenNames { get; set; } = new List<string>();

    // Drink propperties
    public int? DrinkId { get; set; }


    public override string ToString()
    {
        string allergenInfo = AllergenNames.Count > 0 
            ? $"\nAllergens: {string.Join(", ", AllergenNames)}" 
            : "";
        return $"Name: {Name}\nPrice: {Price}\nDescription: {Description}\nType: {Type}{allergenInfo}";
    }

}