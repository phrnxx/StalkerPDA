using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Models;

namespace StalkerPDA.UI.Adapters
{
    public class QuestAdapter : BaseAdapter<PdaQuest>
    {
        private readonly Activity _context;
        private readonly List<PdaQuest> _quests;

        public QuestAdapter(Activity context, List<PdaQuest> quests)
        {
            _context = context;
            _quests = quests;
        }

        public override PdaQuest this[int position] => _quests[position];

        public override int Count => _quests.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.From(_context).Inflate(Resource.Layout.item_quest, null);

            var quest = _quests[position];

            view.FindViewById<TextView>(Resource.Id.quest_title).Text = quest.Title;
            view.FindViewById<TextView>(Resource.Id.quest_deadline).Text = quest.Deadline;

            var stripe = view.FindViewById<View>(Resource.Id.quest_priority_stripe);

            // Кольори Моноліту замість золотих
            if (quest.IsGoogleTask)
            {
                stripe.SetBackgroundColor(Color.ParseColor("#A0E8FF")); // Яскравий блакитний для Мережі
            }
            else
            {
                stripe.SetBackgroundColor(Color.ParseColor("#4A6070")); // Тьмяний синій для особистих нотаток
            }

            return view;
        }
    }
}