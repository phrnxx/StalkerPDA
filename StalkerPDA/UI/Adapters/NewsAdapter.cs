using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using StalkerPDA.Models;

namespace StalkerPDA.UI.Adapters
{
    public class NewsAdapter : BaseAdapter<NewsItem>
    {
        private readonly Context _context;
        private readonly List<NewsItem> _newsList;

        public NewsAdapter(Context context, List<NewsItem> newsList)
        {
            _context = context;
            _newsList = newsList;
        }

        public override NewsItem this[int position] => _newsList[position];

        public override int Count => _newsList.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.FromContext(_context).Inflate(Resource.Layout.item_news, parent, false);

            var dateTextView = view.FindViewById<TextView>(Resource.Id.news_date);
            var titleTextView = view.FindViewById<TextView>(Resource.Id.news_title);
            var descTextView = view.FindViewById<TextView>(Resource.Id.news_description);

            var newsItem = _newsList[position];

            dateTextView.Text = newsItem.DateString;
            titleTextView.Text = newsItem.Title;
            descTextView.Text = newsItem.Description;

            descTextView.Visibility = newsItem.IsExpanded ? ViewStates.Visible : ViewStates.Gone;

            if (newsItem.IsEmergency)
            {
                titleTextView.SetTextColor(Color.ParseColor("#8B0000"));
                descTextView.SetTextColor(Color.ParseColor("#8B0000"));
            }
            else
            {
                titleTextView.SetTextColor(Color.ParseColor("#f2e8d9"));
                descTextView.SetTextColor(Color.ParseColor("#f2e8d9"));
            }

            view.Click -= OnItemClick;
            view.Click += OnItemClick;
            view.Tag = position;

            return view;
        }

        private void OnItemClick(object sender, System.EventArgs e)
        {
            var view = sender as View;
            int position = (int)view.Tag;
            _newsList[position].IsExpanded = !_newsList[position].IsExpanded;
            NotifyDataSetChanged();
        }
    }
}