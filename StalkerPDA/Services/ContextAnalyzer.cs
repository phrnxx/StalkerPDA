using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StalkerPDA.Models;
using Android.Util;

namespace StalkerPDA.Services
{
    public class ContextAnalyzer
    {
        public event Action<PdaMessage> OnTraderNotification;
        private bool _isMonitoring = false;
        private HashSet<string> _knownQuests = new HashSet<string>();
        private GoogleTasksAPI _tasksApi;
        private bool _isFirstRun = true;
        private Random _rand = new Random();

        public ContextAnalyzer()
        {
            _tasksApi = new GoogleTasksAPI();
        }

        public void StartMonitoring()
        {
            if (_isMonitoring) return;
            _isMonitoring = true;
            Task.Run(async () => await MonitoringLoop());
        }

        public void StopMonitoring()
        {
            _isMonitoring = false;
        }

        private string GetSidorovichPhrase(string questName)
        {
            string[] phrases = {
                $"Мічений, якого біса ти прохолоджуєшся? У тебе висить контракт: «{questName}». Рухай батонами!",
                $"Я тобі гроші плачу не за те, щоб ти штани протирав. У тебе висить завдання: «{questName}».",
                $"Коротше, Мічений. Завдання «{questName}» ще актуальне. Чекаю результатів.",
                $"Є робота. Твоє завдання «{questName}» досі висить. Зробиш - отримаєш хабар, не зробиш - підеш на корм псам."
            };
            return phrases[_rand.Next(phrases.Length)];
        }

        private async Task MonitoringLoop()
        {
            await Task.Delay(5000);

            while (_isMonitoring)
            {
                try
                {
                    var activeQuests = await _tasksApi.GetActiveQuestsAsync();
                    if (activeQuests != null)
                    {
                        foreach (var quest in activeQuests)
                        {
                            if (string.IsNullOrEmpty(quest) ||
                                quest.Contains("Немає активних квестів") ||
                                quest.Contains("ПОМИЛКА") ||
                                quest.Contains("Отримання даних"))
                                continue;

                            if (_isFirstRun)
                            {
                                _knownQuests.Add(quest);
                            }
                            else if (!_knownQuests.Contains(quest))
                            {
                                _knownQuests.Add(quest);
                                Log.Debug("PDA_LOG", $"[СИСТЕМА]: Новий квест '{quest}', Сидорович на зв'язку...");

                                string warning = GetSidorovichPhrase(quest);
                                var msg = new PdaMessage("Сидорович", "Торговці", warning);
                                OnTraderNotification?.Invoke(msg);

                                await Task.Delay(5000);
                            }
                        }
                        _isFirstRun = false;
                    }
                }
                catch { }

                await Task.Delay(60000);
            }
        }
    }
}