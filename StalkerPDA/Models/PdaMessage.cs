using System;

namespace StalkerPDA.Models
{
    public class PdaMessage
    {
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
        public string Faction { get; set; }
        public string Text { get; set; }
        public bool IsEasterEgg { get; set; } 

        public string FormattedTime => Timestamp.ToString("HH:mm dd.MM.yyyy");

        public PdaMessage(string author, string faction, string text, bool isEasterEgg = false)
        {
            Timestamp = DateTime.Now;
            Author = author;
            Faction = faction;
            Text = text;
            IsEasterEgg = isEasterEgg;
        }
    }
}