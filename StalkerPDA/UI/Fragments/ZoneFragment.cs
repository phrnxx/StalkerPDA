using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace StalkerPDA.UI.Fragments
{
    public class ZoneFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            view.SetBackgroundColor(Color.Transparent);
            view.SetPadding(16, 16, 16, 16);

            var title = new TextView(Activity) { Text = "АРХІВ ДАНИХ", TextSize = 20 };
            title.SetTextColor(Color.ParseColor("#43A047"));
            title.SetPadding(0, 10, 0, 30);
            view.AddView(title);

            var listView = new ListView(Activity);
            var categories = new List<string> { "Локації", "Мутанти", "Аномалії", "Артефакти", "Зброя" };

            var adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, categories);
            listView.Adapter = adapter;

            listView.ItemClick += (s, e) =>
            {
                string category = categories[e.Position];
                var nextFragment = new LoreListFragment(category);

                var transaction = FragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.main_fragment_container, nextFragment);
                transaction.AddToBackStack(null);
                transaction.Commit();
            };

            view.AddView(listView);
            return view;
        }
    }
}