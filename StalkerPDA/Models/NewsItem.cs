using System;

namespace StalkerPDA.Models
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateString { get; set; }
        public string ArticleUrl { get; set; }
        public bool IsEmergency { get; set; }
        public bool IsExpanded { get; set; }

        public NewsItem(string title, string description, string date, string url = "", bool isEmergency = false)
        {
            Title = title;
            Description = description;
            DateString = date;
            ArticleUrl = url;
            IsEmergency = isEmergency;
            IsExpanded = false;
        }
    }
}