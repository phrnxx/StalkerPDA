using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views; // Обов'язково для роботи зі шторкою екрана
using Android.Widget;
using StalkerPDA.UI.Fragments;
using StalkerPDA.Services;
using StalkerPDA.Models;

namespace StalkerPDA
{
    [Activity(Label = "P.D.A.", Theme = "@android:style/Theme.Black.NoTitleBar", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ContextAnalyzer _contextAnalyzer;
        public static ALifeSimulator SharedSimulator;
        public static List<string> GlobalNetworkMessages = new List<string>();
        public static event Action<string> OnNetworkMessageReceived;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Примусовий ландшафтний режим
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.activity_main);

            InitBackgroundServices();

            // Знаходимо наші нові тактичні кнопки
            var btnQuests = FindViewById<Button>(Resource.Id.tab_quests);
            var btnMap = FindViewById<Button>(Resource.Id.tab_map);
            var btnNetwork = FindViewById<Button>(Resource.Id.tab_network);
            var btnConsciousness = FindViewById<Button>(Resource.Id.tab_consciousness);
            var btnDatabase = FindViewById<Button>(Resource.Id.tab_database);

            // Підключаємо перемикання зі звуком кліку
            btnQuests.Click += (s, e) => LoadFragmentWithSound(new QuestsFragment());
            btnMap.Click += (s, e) => LoadFragmentWithSound(new MapFragment());
            btnNetwork.Click += (s, e) => LoadFragmentWithSound(new ChatFragment());
            btnConsciousness.Click += (s, e) => LoadFragmentWithSound(new ConsciousnessFragment());
            btnDatabase.Click += (s, e) => LoadFragmentWithSound(new ZoneFragment());

            // Завантажуємо першу вкладку при старті (без звуку, щоб не клацало при запуску)
            if (savedInstanceState == null) LoadFragmentWithSound(new QuestsFragment(), playSound: false);
        }

        // ==========================================
        // МАГІЯ ЗНИЩЕННЯ ШТОРКИ (Immersive Mode)
        // ==========================================
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                // Вмикаємо жорсткий повноекранний режим. 
                // Телефон тепер виглядає як суцільний пристрій без системних іконок Android.
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.ImmersiveSticky |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.LayoutStable);
            }
        }

        private void LoadFragmentWithSound(Fragment fragment, bool playSound = true)
        {
            if (playSound) SoundManager.PlayClick(this);
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.main_fragment_container, fragment);
            transaction.Commit();
        }

        private void InitBackgroundServices()
        {
            if (SharedSimulator == null)
            {
                SharedSimulator = new ALifeSimulator();
                SharedSimulator.OnMessageGenerated += (s, msg) => {
                    var formatted = FormatPdaMessage(msg);
                    RunOnUiThread(() => {
                        GlobalNetworkMessages.Add(formatted);
                        OnNetworkMessageReceived?.Invoke(formatted);
                    });
                };
                SharedSimulator.StartSimulation();
            }

            if (_contextAnalyzer == null)
            {
                _contextAnalyzer = new ContextAnalyzer();
                _contextAnalyzer.OnTraderNotification += ShowIncomingMessage;
                _contextAnalyzer.StartMonitoring();
            }
        }

        public static string FormatPdaMessage(PdaMessage msg)
        {
            var pdaId = new Random(msg.Author.GetHashCode()).Next(10000, 99999);
            var time = DateTime.Now.ToString("HH:mm");
            return $"{msg.Author} [PDA #{pdaId}] - {time}\n{msg.Text}";
        }

        private void ShowIncomingMessage(PdaMessage msg)
        {
            RunOnUiThread(() =>
            {
                // Звук нового квесту/повідомлення
                SoundManager.PlayNotification(this);

                var builder = new AlertDialog.Builder(this);
                builder.SetTitle($"СИГНАЛ: {msg.Author}");
                builder.SetMessage(msg.Text);
                builder.SetPositiveButton("ПРИНЯТИ", (s, e) => { });
                builder.Create().Show();
            });
        }
    }
}