using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using StalkerPDA.Models;

namespace StalkerPDA.UI.Adapters
{
    public class ChatAdapter : BaseAdapter<PdaMessage>
    {
        private readonly Context _context;
        private readonly List<PdaMessage> _messages;

        public ChatAdapter(Context context, List<PdaMessage> messages)
        {
            _context = context;
            _messages = messages;
        }

        public override PdaMessage this[int position] => _messages[position];
        public override int Count => _messages.Count;
        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.FromContext(_context).Inflate(Resource.Layout.item_message, parent, false);

            var headerTextView = view.FindViewById<TextView>(Resource.Id.message_header);
            var bodyTextView = view.FindViewById<TextView>(Resource.Id.message_body);

            var message = _messages[position];

            int pdaId = Math.Abs(message.Author.GetHashCode()) % 90000 + 10000;

            headerTextView.Text = $"{message.Author} [PDA #{pdaId}] {message.Faction} - {message.FormattedTime}";
            bodyTextView.Text = message.Text;

            if (message.IsEasterEgg)
            {
                headerTextView.SetTextColor(Color.ParseColor("#39ff14"));
                bodyTextView.SetTextColor(Color.ParseColor("#39ff14"));
            }
            else
            {
                headerTextView.SetTextColor(Color.ParseColor("#f2e8d9"));
                bodyTextView.SetTextColor(Color.ParseColor("#f2e8d9"));
            }

            return view;
        }
    }
}