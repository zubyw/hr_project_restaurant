public class ThemeModel
{

    public int ID { get; set; }
    public required string Name { get; set; }
    public required string  Course { get; set; }
    private int IsActive { get; set; } = 1;

}