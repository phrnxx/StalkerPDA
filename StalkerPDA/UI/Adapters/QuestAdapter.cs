using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace StalkerPDA.UI.Adapters
{
    public class QuestAdapter : BaseAdapter<string>
    {
        private readonly Context _context;
        private readonly List<string> _quests;

        public QuestAdapter(Context context, List<string> quests)
        {
            _context = context;
            _quests = quests;
        }

        public override string this[int position] => _quests[position];

        public override int Count => _quests.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.FromContext(_context).Inflate(Resource.Layout.item_quest, parent, false);

            var titleTextView = view.FindViewById<TextView>(Resource.Id.quest_title);
            titleTextView.Text = _quests[position];

            return view;
        }
    }
}