namespace StalkerPDA.Models
{
    public class LoreItem
    {
        public string Name { get; set; }
        public string ImageResourceName { get; set; } 
        public string Description { get; set; }

        public LoreItem(string name, string imageResourceName, string description)
        {
            Name = name;
            ImageResourceName = imageResourceName;
            Description = description;
        }
    }
}