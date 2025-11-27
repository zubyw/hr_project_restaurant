public class ThemeModel
{

    public int ID { get; set; }
    public required string Name { get; set; }
    public required string  Course { get; set; }
    public int IsActive { get; set; } = 1;


    public override string ToString()
    {
        return $"Name: {Name}\nDescription: {Course}";
    }
}