using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Services;

namespace StalkerPDA.UI.Fragments
{
    public class ConsciousnessFragment : Fragment
    {
        private ListView _chatListView;
        private EditText _chatInput;
        private Button _btnSend;
        private List<string> _messages;
        private ConsciousnessChatAdapter _adapter;
        private GeminiService _aiService;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_consciousness, container, false);

            _chatListView = view.FindViewById<ListView>(Resource.Id.consciousness_list_view);
            _chatInput = view.FindViewById<EditText>(Resource.Id.consciousness_input);
            _btnSend = view.FindViewById<Button>(Resource.Id.btn_consciousness_send);

            _messages = new List<string>();
            _adapter = new ConsciousnessChatAdapter(Activity, _messages);
            _chatListView.Adapter = _adapter;

            _aiService = new GeminiService();

            _btnSend.Click += async (s, e) => {
                string text = _chatInput.Text.Trim();
                if (string.IsNullOrEmpty(text)) return;

                _chatInput.Text = "";
                _btnSend.Enabled = false;

                AddMessage($"[ВИ]: {text}");

                try
                {
                    string response = await _aiService.SendConsciousnessMessageAsync(text);
                    AddMessage($"[О-СВІДОМІСТЬ]: {response}");
                }
                catch (Exception ex)
                {
                    AddMessage($"[ПОМИЛКА]: {ex.Message}");
                }
                finally
                {
                    if (Activity != null)
                        Activity.RunOnUiThread(() => _btnSend.Enabled = true);
                }
            };

            return view;
        }

        private void AddMessage(string msg)
        {
            if (Activity == null) return;
            Activity.RunOnUiThread(() => {
                _messages.Add(msg);
                _adapter.NotifyDataSetChanged();
                _chatListView.SmoothScrollToPosition(_messages.Count - 1);
            });
        }
    }

    public class ConsciousnessChatAdapter : BaseAdapter<string>
    {
        private readonly Activity _context;
        private readonly List<string> _items;

        public ConsciousnessChatAdapter(Activity context, List<string> items)
        {
            _context = context;
            _items = items;
        }

        public override string this[int position] => _items[position];
        public override int Count => _items.Count;
        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView textView = (convertView as TextView) ?? new TextView(_context);

            textView.Text = _items[position];
            textView.SetPadding(25, 15, 25, 15);
            textView.TextSize = 16f;

            if (_items[position].StartsWith("[ВИ]"))
            {
                textView.SetTextColor(Color.ParseColor("#f2e8d9")); 
                textView.SetBackgroundColor(Color.ParseColor("#1A1A1A"));
            }
            else
            {
                textView.SetTextColor(Color.ParseColor("#00BFFF")); 
                textView.SetBackgroundColor(Color.Transparent);
            }

            return textView;
        }
    }
}