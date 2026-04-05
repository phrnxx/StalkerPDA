using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Models;

namespace StalkerPDA.UI.Fragments
{
    public class LoreDetailFragment : Fragment
    {
        private LoreItem _item;

        public LoreDetailFragment(LoreItem item)
        {
            _item = item;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var scroll = new ScrollView(Activity);
            scroll.SetBackgroundColor(Color.Transparent);

            var view = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            view.SetPadding(30, 30, 30, 30);

            var btnBack = new Button(Activity) { Text = "< ПОВЕРНУТИСЯ" };
            btnBack.SetTextColor(Color.ParseColor("#00BFFF"));
            btnBack.SetBackgroundColor(Color.Transparent);
            btnBack.Gravity = GravityFlags.Left;
            btnBack.Click += (s, e) => FragmentManager.PopBackStack();
            view.AddView(btnBack);

            var title = new TextView(Activity) { Text = _item.Name, TextSize = 24 };
            title.SetTextColor(Color.ParseColor("#00BFFF"));
            title.SetTypeface(null, TypefaceStyle.Bold);
            title.SetPadding(0, 0, 0, 20);
            view.AddView(title);

            if (!string.IsNullOrEmpty(_item.ImageResourceName))
            {
                int resId = Activity.Resources.GetIdentifier(_item.ImageResourceName, "drawable", Activity.PackageName);
                if (resId != 0)
                {
                    var image = new ImageView(Activity);
                    image.SetImageResource(resId);
                    image.SetAdjustViewBounds(true);
                    image.SetPadding(0, 0, 0, 30);
                    view.AddView(image);
                }
            }

            var desc = new TextView(Activity) { Text = _item.Description, TextSize = 16 };
            desc.SetTextColor(Color.White);
            view.AddView(desc);

            scroll.AddView(view);
            return scroll;
        }
    }
}