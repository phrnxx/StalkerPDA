using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using StalkerPDA.UI.Fragments;
using StalkerPDA.Services;
using StalkerPDA.Models;

namespace StalkerPDA
{
    [Activity(Label = "P.D.A.", Theme = "@android:style/Theme.Black.NoTitleBar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Button _btnQuests, _btnNetwork, _btnNews, _btnZone, _btnConsciousness;
        private ContextAnalyzer _contextAnalyzer;

        public static ALifeSimulator SharedSimulator;
        public static List<string> GlobalNetworkMessages = new List<string>();
        public static event Action<string> OnNetworkMessageReceived;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (SharedSimulator == null)
            {
                SharedSimulator = new ALifeSimulator();

                for (int i = 0; i < 3; i++)
                {
                    var stalker = LoreDatabase.GetRandomActiveCharacter();
                    var msg = new PdaMessage(stalker.Name, stalker.Faction, LoreDatabase.GetPhraseForStalker(stalker));
                    GlobalNetworkMessages.Add(FormatPdaMessage(msg));
                }

                SharedSimulator.OnMessageGenerated += (s, msg) => {
                    var formatted = FormatPdaMessage(msg);
                    RunOnUiThread(() => {
                        GlobalNetworkMessages.Add(formatted);
                        OnNetworkMessageReceived?.Invoke(formatted);
                    });
                };

                SharedSimulator.StartSimulation();
            }

            _contextAnalyzer = new ContextAnalyzer();
            _contextAnalyzer.OnTraderNotification += ShowIncomingMessage;
            _contextAnalyzer.StartMonitoring();

            _btnQuests = FindViewById<Button>(Resource.Id.btn_quests);
            _btnNetwork = FindViewById<Button>(Resource.Id.btn_network);
            _btnNews = FindViewById<Button>(Resource.Id.btn_news);
            _btnZone = FindViewById<Button>(Resource.Id.btn_zone);
            _btnConsciousness = FindViewById<Button>(Resource.Id.btn_consciousness);

            _btnQuests.Click += (s, e) => { LoadFragment(new QuestsFragment()); UpdateTabStyles(_btnQuests); };
            _btnNetwork.Click += (s, e) => { LoadFragment(new ChatFragment()); UpdateTabStyles(_btnNetwork); };
            _btnNews.Click += (s, e) => { LoadFragment(new NewsFragment()); UpdateTabStyles(_btnNews); };
            _btnZone.Click += (s, e) => { LoadFragment(new ZoneFragment()); UpdateTabStyles(_btnZone); };
            _btnConsciousness.Click += (s, e) => { LoadFragment(new ConsciousnessFragment()); UpdateTabStyles(_btnConsciousness); };

            LoadFragment(new QuestsFragment());
            UpdateTabStyles(_btnQuests);
        }

        public static string FormatPdaMessage(PdaMessage msg)
        {
            var pdaId = new Random(msg.Author.GetHashCode()).Next(10000, 99999);
            var time = DateTime.Now.ToString("HH:mm");
            var date = DateTime.Now.ToString("dd.MM.yyyy");
            return $"{msg.Author} [PDA #{pdaId}] {msg.Faction} - {time}\n{date}\n{msg.Text}";
        }

        private void ShowIncomingMessage(PdaMessage msg)
        {
            RunOnUiThread(() =>
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle($"ВХІДНИЙ СИГНАЛ: {msg.Author}");
                builder.SetMessage(msg.Text);
                builder.SetPositiveButton("ПРИЙНЯТИ", (s, e) => { });
                builder.Create().Show();
            });
        }

        private void LoadFragment(Fragment fragment)
        {
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.fragment_container, fragment);
            transaction.Commit();
        }

        private void UpdateTabStyles(Button activeButton)
        {
            var dimColor = Color.ParseColor("#1A3A5A");
            _btnQuests.SetTextColor(dimColor);
            _btnNetwork.SetTextColor(dimColor);
            _btnNews.SetTextColor(dimColor);
            _btnZone.SetTextColor(dimColor);
            _btnConsciousness.SetTextColor(dimColor);
            activeButton.SetTextColor(Color.ParseColor("#00BFFF"));
        }
    }
}