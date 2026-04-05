using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using StalkerPDA.Models;
using StalkerPDA.Services;
using StalkerPDA.UI.Adapters;

namespace StalkerPDA.UI.Fragments
{
    public class NewsFragment : Fragment
    {
        private ListView _listView;
        private NewsAdapter _adapter;
        private NewsScraper _scraper;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_news, container, false);
            _listView = view.FindViewById<ListView>(Resource.Id.news_list_view);
            _scraper = new NewsScraper();

            LoadNews();

            return view;
        }

        private async void LoadNews()
        {
            List<NewsItem> news = await _scraper.FetchLatestNewsAsync();

            if (Activity != null)
            {
                Activity.RunOnUiThread(() =>
                {
                    _adapter = new NewsAdapter(Activity, news);
                    _listView.Adapter = _adapter;
                });
            }
        }
    }
}