using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace StalkerPDA.UI.Adapters
{
    public class ChatAdapter : BaseAdapter<string>
    {
        private readonly Activity _context;
        private readonly List<string> _messages;

        public ChatAdapter(Activity context, List<string> messages)
        {
            _context = context;
            _messages = messages;
        }

        public override string this[int position] => _messages[position];
        public override int Count => _messages.Count;
        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.From(_context).Inflate(Resource.Layout.item_message, null);

            var message = _messages[position];
            var parts = message.Split(new[] { '\n' }, 2);

            var headerTextView = view.FindViewById<TextView>(Resource.Id.message_header);
            var bodyTextView = view.FindViewById<TextView>(Resource.Id.message_body);

            if (parts.Length == 2)
            {
                headerTextView.Text = parts[0];
                bodyTextView.Text = parts[1];
            }
            else
            {
                headerTextView.Text = "SYSTEM MESSAGE";
                bodyTextView.Text = message;
            }

            if (message.Contains("Габела"))
            {
                headerTextView.SetTextColor(Color.ParseColor("#A0E8FF"));
                bodyTextView.SetTextColor(Color.ParseColor("#A0E8FF"));
            }
            else
            {
                headerTextView.SetTextColor(Color.ParseColor("#4A6070"));
                bodyTextView.SetTextColor(Color.ParseColor("#E0F0FF"));
            }

            return view;
        }
    }
}