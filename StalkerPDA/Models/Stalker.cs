namespace StalkerPDA.Models
{
    public class Stalker
    {
        public string Name { get; set; }
        public string Faction { get; set; }

        public Stalker(string name, string faction)
        {
            Name = name;
            Faction = faction;
        }
    }
}