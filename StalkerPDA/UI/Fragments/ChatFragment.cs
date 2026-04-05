using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Models;

namespace StalkerPDA.UI.Fragments
{
    public class ChatFragment : Fragment
    {
        private ListView _listView;
        private NetworkAdapter _adapter;
        private List<string> _localMessages;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_network, container, false);
            _listView = view.FindViewById<ListView>(Resource.Id.network_list_view);

            _localMessages = new List<string>(MainActivity.GlobalNetworkMessages);
            _adapter = new NetworkAdapter(Activity, _localMessages);
            _listView.Adapter = _adapter;

            MainActivity.OnNetworkMessageReceived += OnNewMessage;

            ScrollToBottom();

            return view;
        }

        private void OnNewMessage(string newMsg)
        {
            if (Activity == null || _listView == null || _adapter == null) return;

            Activity.RunOnUiThread(() => {
                _adapter.Add(newMsg);
                ScrollToBottom();
            });
        }

        private void ScrollToBottom()
        {
            if (_adapter.Count > 0)
            {
                _listView.SmoothScrollToPosition(_adapter.Count - 1);
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MainActivity.OnNetworkMessageReceived -= OnNewMessage;
        }
    }

    public class NetworkAdapter : ArrayAdapter<string>
    {
        public NetworkAdapter(Activity context, IList<string> objects)
            : base(context, Android.Resource.Layout.SimpleListItem1, objects) { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);

            textView.SetTextColor(Color.White);
            textView.TextSize = 14f;
            textView.SetMaxLines(10);
            textView.SetPadding(20, 15, 20, 15);

            return view;
        }
    }
}