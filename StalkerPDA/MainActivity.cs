using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.UI.Fragments;
using StalkerPDA.Services;
using StalkerPDA.Models;

namespace StalkerPDA
{
    [Activity(Label = "P.D.A.", Theme = "@style/Theme.StalkerPDA", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ContextAnalyzer _contextAnalyzer;
        private Timer _statusTimer;
        private TextView _tvBattery;
        private TextView _tvSignal;
        private TextView _tvClock;
        private Button _lastSelectedButton;

        public static ALifeSimulator SharedSimulator;
        public static List<string> GlobalNetworkMessages = new List<string>();
        public static event Action<string> OnNetworkMessageReceived;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);

            InitBackgroundServices();

            _tvBattery = FindViewById<TextView>(Resource.Id.sidebar_battery);
            _tvSignal = FindViewById<TextView>(Resource.Id.sidebar_signal);
            _tvClock = FindViewById<TextView>(Resource.Id.sidebar_clock);

            _statusTimer = new Timer((e) => { RunOnUiThread(UpdateStatusIndicators); }, null, 0, 30000);

            var btnQuests = FindViewById<Button>(Resource.Id.tab_quests);
            var btnMap = FindViewById<Button>(Resource.Id.tab_map);
            var btnNetwork = FindViewById<Button>(Resource.Id.tab_network);
            var btnNews = FindViewById<Button>(Resource.Id.tab_news);
            var btnConsciousness = FindViewById<Button>(Resource.Id.tab_consciousness);
            var btnDatabase = FindViewById<Button>(Resource.Id.tab_database);

            btnQuests.Click += (s, e) => { LoadFragmentWithSound(new QuestsFragment()); HighlightTab(btnQuests); };
            btnMap.Click += (s, e) => { LoadFragmentWithSound(new MapFragment()); HighlightTab(btnMap); };
            btnNetwork.Click += (s, e) => { LoadFragmentWithSound(new ChatFragment()); HighlightTab(btnNetwork); };
            btnNews.Click += (s, e) => { LoadFragmentWithSound(new NewsFragment()); HighlightTab(btnNews); };
            btnConsciousness.Click += (s, e) => { LoadFragmentWithSound(new ConsciousnessFragment()); HighlightTab(btnConsciousness); };
            btnDatabase.Click += (s, e) => { LoadFragmentWithSound(new ZoneFragment()); HighlightTab(btnDatabase); };

            LoadFragmentWithSound(new QuestsFragment());
            HighlightTab(btnQuests);
        }

        private void UpdateStatusIndicators()
        {
            _tvClock.Text = DateTime.Now.ToString("HH:mm");

            var filter = new Android.Content.IntentFilter(Android.Content.Intent.ActionBatteryChanged);
            var batteryIntent = RegisterReceiver(null, filter);
            int level = batteryIntent?.GetIntExtra(BatteryManager.ExtraLevel, -1) ?? -1;
            int scale = batteryIntent?.GetIntExtra(BatteryManager.ExtraScale, -1) ?? -1;

            if (level >= 0 && scale > 0)
            {
                int batteryPct = (int)((level / (float)scale) * 100);
                _tvBattery.Text = $"{batteryPct}%";
                _tvBattery.SetTextColor(batteryPct <= 15 ? Color.ParseColor("#C84040") : Color.ParseColor("#C87030"));
            }

            FetchRadiationLevelAsync();
        }

        private async void FetchRadiationLevelAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    string html = await client.GetStringAsync("https://www.saveecobot.com/radiation/dnipropetrovska-oblast/kryvyi-rih");
                    var match = Regex.Match(html, @"(\d+[\.,]\d+)\s*(мкЗв/год|мкР/год|μSv/h)", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string radValue = match.Groups[1].Value;
                        RunOnUiThread(() => _tvSignal.Text = radValue);
                    }
                }
            }
            catch { RunOnUiThread(() => _tvSignal.Text = "ERR"); }
        }

        private void HighlightTab(Button clickedButton)
        {
            if (_lastSelectedButton != null)
            {
                _lastSelectedButton.Selected = false;
                _lastSelectedButton.SetTextColor(Color.ParseColor("#D4C090"));
            }
            clickedButton.Selected = true;
            clickedButton.SetTextColor(Color.ParseColor("#C8A040"));
            _lastSelectedButton = clickedButton;
        }

        private void LoadFragmentWithSound(Fragment fragment)
        {
            FragmentManager.BeginTransaction().Replace(Resource.Id.main_fragment_container, fragment).Commit();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
            }
        }

        private void InitBackgroundServices()
        {
            if (SharedSimulator == null)
            {
                SharedSimulator = new ALifeSimulator();
                SharedSimulator.OnMessageGenerated += (s, msg) => {
                    var formatted = FormatPdaMessage(msg);
                    RunOnUiThread(() => { GlobalNetworkMessages.Add(formatted); OnNetworkMessageReceived?.Invoke(formatted); });
                };
                SharedSimulator.StartSimulation();
            }

            if (_contextAnalyzer == null)
            {
                _contextAnalyzer = new ContextAnalyzer();
                _contextAnalyzer.StartMonitoring();
            }
        }

        public static string FormatPdaMessage(PdaMessage msg)
        {
            var pdaId = new Random(msg.Author.GetHashCode()).Next(10000, 99999);
            return $"{msg.Author} [PDA #{pdaId}] - {DateTime.Now:HH:mm}\n{msg.Text}";
        }
    }
}