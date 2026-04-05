using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Models;
using StalkerPDA.Services;

namespace StalkerPDA.UI.Fragments
{
    public class LoreListFragment : Fragment
    {
        private string _category;
        private List<LoreItem> _items;

        public LoreListFragment(string category)
        {
            _category = category;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            view.SetBackgroundColor(Color.Transparent);

            var btnBack = new Button(Activity) { Text = "< ПОВЕРНУТИСЯ" };
            btnBack.SetTextColor(Color.ParseColor("#00BFFF"));
            btnBack.SetBackgroundColor(Color.Transparent);
            btnBack.Gravity = GravityFlags.Left;
            btnBack.Click += (s, e) => FragmentManager.PopBackStack();
            view.AddView(btnBack);

            var title = new TextView(Activity) { Text = _category.ToUpper(), TextSize = 20 };
            title.SetTextColor(Color.ParseColor("#00BFFF"));
            title.SetPadding(20, 20, 20, 20);
            view.AddView(title);

            if (_category == "Локації") _items = LoreDatabase.GetLocations();
            else if (_category == "Мутанти") _items = LoreDatabase.GetMutants();
            else if (_category == "Аномалії") _items = LoreDatabase.GetAnomalies();
            else if (_category == "Артефакти") _items = LoreDatabase.GetArtifacts();
            else if (_category == "Зброя") _items = LoreDatabase.GetWeapons();
            else _items = new List<LoreItem>();

            var listView = new ListView(Activity);
            var names = _items.Select(i => i.Name).ToList();
            var adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, names);
            listView.Adapter = adapter;

            listView.ItemClick += (s, e) =>
            {
                var item = _items[e.Position];
                var transaction = FragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.fragment_container, new LoreDetailFragment(item));
                transaction.AddToBackStack(null);
                transaction.Commit();
            };

            view.AddView(listView);
            return view;
        }
    }
}