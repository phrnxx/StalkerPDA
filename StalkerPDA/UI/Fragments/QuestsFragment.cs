using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using StalkerPDA.Services;

namespace StalkerPDA.UI.Fragments
{
    public class QuestsFragment : Fragment
    {
        private ListView _questsListView;
        private TextView _monolithWarning;
        private TextView _weatherStatus;
        private QuestAdapter _adapter;
        private List<string> _quests;
        private GoogleTasksAPI _tasksApi;
        private BatteryMonitor _batteryMonitor;
        private WeatherAPI _weatherApi;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_quests, container, false);

            _questsListView = view.FindViewById<ListView>(Resource.Id.quests_list_view);
            _monolithWarning = view.FindViewById<TextView>(Resource.Id.monolith_warning);
            _weatherStatus = view.FindViewById<TextView>(Resource.Id.weather_status);

            _quests = new List<string>();
            _adapter = new QuestAdapter(Activity, _quests);
            _questsListView.Adapter = _adapter;

            _tasksApi = new GoogleTasksAPI();
            _batteryMonitor = new BatteryMonitor();
            _weatherApi = new WeatherAPI();

            _adapter.Add("Отримання даних із сервера...");

            CheckBatteryLevel();
            LoadQuestsAsync();
            LoadWeatherAsync();

            return view;
        }

        private void CheckBatteryLevel()
        {
            try
            {
                int battery = _batteryMonitor.GetBatteryLevel();
                if (battery > 0 && battery <= 20)
                {
                    _monolithWarning.Visibility = ViewStates.Visible;
                }
                else
                {
                    _monolithWarning.Visibility = ViewStates.Gone;
                }
            }
            catch { }
        }

        private void LoadQuestsAsync()
        {
            Task.Run(async () =>
            {
                try
                {
                    var activeQuests = await _tasksApi.GetActiveQuestsAsync();
                    UpdateQuestsUI(activeQuests);
                }
                catch (Exception ex)
                {
                    UpdateQuestsUI(null, $"Помилка: {ex.Message}");
                }
            });
        }

        private void UpdateQuestsUI(List<string> quests, string errorMessage = null)
        {
            if (Activity == null) return;

            Activity.RunOnUiThread(() =>
            {
                _adapter.Clear();

                if (errorMessage != null)
                {
                    _adapter.Add(errorMessage);
                }
                else if (quests != null && quests.Count > 0)
                {
                    foreach (var q in quests)
                    {
                        _adapter.Add(q);
                    }
                }
                else
                {
                    _adapter.Add("Немає активних контрактів.");
                }

                _adapter.NotifyDataSetChanged();
            });
        }

        private async void LoadWeatherAsync()
        {
            try
            {
                string weather = await _weatherApi.GetWeatherAsync();
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    _weatherStatus.Text = weather;
                });
            }
            catch
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    _weatherStatus.Text = "Погода: Немає зв'язку з Сферою";
                });
            }
        }
    }

    public class QuestAdapter : ArrayAdapter<string>
    {
        public QuestAdapter(Activity context, IList<string> objects)
            : base(context, Android.Resource.Layout.SimpleListItem1, objects) { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);

            textView.SetTextColor(Color.White);
            textView.TextSize = 16f;
            textView.SetPadding(20, 20, 20, 20);

            return view;
        }
    }
}