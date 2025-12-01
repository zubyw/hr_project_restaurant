namespace Project.DataModels
{
    public class AllergenModel
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}
