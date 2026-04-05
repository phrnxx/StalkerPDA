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

            var title = new TextView(Activity) { Text = "ЕНЦИКЛОПЕДІЯ ЗОНИ", TextSize = 20 };
            title.SetTextColor(Color.ParseColor("#00BFFF"));
            title.SetPadding(20, 20, 20, 20);
            view.AddView(title);

            var listView = new ListView(Activity);
            var categories = new List<string> { "Локації", "Мутанти", "Аномалії", "Артефакти", "Зброя" };
            var adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, categories);
            listView.Adapter = adapter;

            listView.ItemClick += (s, e) =>
            {
                string category = categories[e.Position];
                var transaction = FragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.fragment_container, new LoreListFragment(category));
                transaction.AddToBackStack(null);
                transaction.Commit();
            };

            view.AddView(listView);
            return view;
        }
    }
}