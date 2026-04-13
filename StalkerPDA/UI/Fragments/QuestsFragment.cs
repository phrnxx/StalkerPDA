using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using StalkerPDA.Models;
using StalkerPDA.Services;
using StalkerPDA.UI.Adapters;

namespace StalkerPDA.UI.Fragments
{
    public class QuestsFragment : Fragment
    {
        private ListView _questsListView;
        private TextView _monolithWarning;
        private TextView _weatherStatus;
        private EditText _etNewNote;
        private Button _btnAddNote;
        private QuestAdapter _adapter;
        private List<PdaQuest> _quests;
        private GoogleTasksAPI _tasksApi;
        private BatteryMonitor _batteryMonitor;
        private WeatherAPI _weatherApi;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_quests, container, false);

            _questsListView = view.FindViewById<ListView>(Resource.Id.quests_list_view);
            _monolithWarning = view.FindViewById<TextView>(Resource.Id.monolith_warning);
            _weatherStatus = view.FindViewById<TextView>(Resource.Id.weather_status);
            _etNewNote = view.FindViewById<EditText>(Resource.Id.et_new_note);
            _btnAddNote = view.FindViewById<Button>(Resource.Id.btn_add_note);

            _quests = new List<PdaQuest>();
            _adapter = new QuestAdapter(Activity, _quests);
            _questsListView.Adapter = _adapter;

            _tasksApi = new GoogleTasksAPI();
            _batteryMonitor = new BatteryMonitor();
            _weatherApi = new WeatherAPI();

            _btnAddNote.Click += (s, e) =>
            {
                string text = _etNewNote.Text.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    LocalNotesManager.AddNote(text);
                    _etNewNote.Text = "";
                    LoadQuestsAsync();
                }
            };

            _questsListView.ItemLongClick += OnQuestLongClick;

            CheckBatteryLevel();
            LoadQuestsAsync();
            LoadWeatherAsync();

            return view;
        }

        private void OnQuestLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var quest = _quests[e.Position];

            if (quest.Id == "error" || quest.Id == "empty") return;

            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("БАЗА ДАНИХ");

            if (quest.IsGoogleTask)
            {
                builder.SetMessage("Це завдання з Мережі (Google Tasks). Позначте його як виконане на своєму терміналі, щоб воно зникло з ПДА.");
                builder.SetPositiveButton("ЗРОЗУМІЛО", (s, args) => { });
            }
            else
            {
                builder.SetMessage($"Видалити запис: '{quest.Title}'?");
                builder.SetPositiveButton("ТАК", (s, args) =>
                {
                    LocalNotesManager.RemoveNote(quest.Id);
                    LoadQuestsAsync();
                });
                builder.SetNegativeButton("НІ", (s, args) => { });
            }

            builder.Create().Show();
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
                var allQuests = new List<PdaQuest>();

                allQuests.AddRange(LocalNotesManager.GetNotes());

                try
                {
                    var activeTasks = await _tasksApi.GetActiveQuestsAsync();
                    if (activeTasks != null)
                    {
                        foreach (var task in activeTasks)
                        {
                            allQuests.Add(new PdaQuest
                            {
                                Id = Guid.NewGuid().ToString(),
                                Title = task,
                                Deadline = "МЕРЕЖА",
                                IsGoogleTask = true
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    allQuests.Add(new PdaQuest
                    {
                        Id = "error",
                        Title = $"Помилка: {ex.Message}",
                        Deadline = "СИСТЕМА",
                        IsGoogleTask = true
                    });
                }

                if (Activity == null) return;

                Activity.RunOnUiThread(() =>
                {
                    _quests.Clear();
                    if (allQuests.Count == 0)
                    {
                        _quests.Add(new PdaQuest
                        {
                            Id = "empty",
                            Title = "Немає активних контрактів.",
                            Deadline = "СИСТЕМА",
                            IsGoogleTask = false
                        });
                    }
                    else
                    {
                        _quests.AddRange(allQuests);
                    }
                    _adapter.NotifyDataSetChanged();
                });
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
}