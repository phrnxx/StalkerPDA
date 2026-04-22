using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StalkerPDA.Models;

namespace StalkerPDA.Services
{
    public class PendingReply
    {
        public DateTime ExecuteTime { get; set; }
        public Stalker Author { get; set; }
        public string Text { get; set; }
    }

    public class ALifeSimulator
    {
        public event EventHandler<PdaMessage> OnMessageGenerated;

        public double CurrentRadiationLevel { get; set; } = 0.12;

        private bool _isRunning = false;
        private Random _rng = new Random();
        private List<PendingReply> _pendingReplies = new List<PendingReply>();
        private const int TICK_RATE_MS = 6000;

        public void StartSimulation()
        {
            if (_isRunning) return;
            _isRunning = true;
            Task.Run(async () => await SimulationLoop());
        }

        public void StopSimulation()
        {
            _isRunning = false;
        }

        private async Task SimulationLoop()
        {
            while (_isRunning)
            {
                DateTime now = DateTime.Now;
                var dueReplies = _pendingReplies.Where(r => r.ExecuteTime <= now).ToList();

                foreach (var reply in dueReplies)
                {
                    SendMessage(reply.Author, reply.Text);
                    _pendingReplies.Remove(reply);
                    await Task.Delay(1500);
                }

                if (_pendingReplies.Count == 0 && _rng.Next(0, 100) < 60)
                {
                    GenerateRandomEvent();
                }

                await Task.Delay(TICK_RATE_MS);
            }
        }

        private void GenerateRandomEvent()
        {
            Stalker activeStalker = LoreDatabase.GetRandomActiveCharacter();
            string phrase = "";

            int currentHour = DateTime.Now.Hour;
            int eventRoll = _rng.Next(0, 100);

            if (CurrentRadiationLevel >= 0.50 && eventRoll < 30)
            {
                phrase = GetRadiationEventPhrase();
            }
            else if (eventRoll >= 30 && eventRoll < 60)
            {
                phrase = GetTimeOfDayPhrase(currentHour);
            }
            else
            {
                ProceduralALife generator = new ProceduralALife();
                phrase = generator.GenerateMessage();
            }

            SendMessage(activeStalker, phrase);

            if (_rng.Next(0, 100) < 70)
            {
                CreateReplyChain(activeStalker, phrase);
            }
        }

        private string GetTimeOfDayPhrase(int hour)
        {
            if (hour >= 22 || hour <= 4)
            {
                string[] nightMessages = {
                    "Ніч темна... ПНБ барахлить, видимість нуль. Сидіть біля вогнищ.",
                    "Чули виття з боку Темної Долини? Краще туди до ранку не сунутися.",
                    "Увага всім: вночі активність кровососів зросла. Будьте обережні на відкритих місцях."
                };
                return nightMessages[_rng.Next(nightMessages.Length)];
            }
            else if (hour >= 5 && hour <= 10)
            {
                string[] morningMessages = {
                    "Ранок, сталкери. Туман розсіюється, вдалого полювання за артефактами.",
                    "Хто йде на Кордон? Шукаю напарника на ранкову ходку.",
                    "Тільки-но сонце встало, а військові вже патруль вислали. Пильнуйте."
                };
                return morningMessages[_rng.Next(morningMessages.Length)];
            }
            else if (hour >= 18 && hour <= 21)
            {
                string[] eveningMessages = {
                    "Сонце сідає. Час повертатися на базу і рахувати хабар.",
                    "Хто буде на '100 Рентген' ввечері? Я пригощаю, знайшов цілу 'Сніжинку'!",
                    "Тіні довшають, аномалії гірше видно. Згортаємо активність."
                };
                return eveningMessages[_rng.Next(eveningMessages.Length)];
            }
            else
            {
                string[] dayMessages = {
                    "Спека сьогодні... Аномалії ледь видно через марево.",
                    "Патруль біля мосту, будьте обережні при переході.",
                    "Знайшов 'Медузу', міняю на патрони 5.45. Чекаю на Смітнику."
                };
                return dayMessages[_rng.Next(dayMessages.Length)];
            }
        }

        private string GetRadiationEventPhrase()
        {
            string[] radMessages = {
                $"Дозиметр тріщить як скажений. Фон піднявся до {CurrentRadiationLevel} мР/год! Одягайте респіратори.",
                "Знову радіоактивна хмара з боку Смітника йде. Шукайте укриття, брати.",
                "Радіаційний фон скаче... Або артефакт потужний поруч, або зараз щось буде.",
                "Антиради закінчуються, а фон все росте. Хто має зайві 'шприци'?"
            };
            return radMessages[_rng.Next(radMessages.Length)];
        }

        private void CreateReplyChain(Stalker firstSpeaker, string triggerPhrase)
        {
            int replyCount = _rng.Next(1, 5);
            int delaySeconds = _rng.Next(3, 8);

            List<Stalker> possibleRepliers = LoreDatabase.Characters
                .Where(c => c.Name != firstSpeaker.Name && c.Name != "Габела")
                .ToList();

            Stalker previousSpeaker = firstSpeaker;
            string previousPhrase = triggerPhrase;

            for (int i = 0; i < replyCount; i++)
            {
                var candidates = possibleRepliers
                    .Where(r => r.Name != previousSpeaker.Name)
                    .ToList();
                if (candidates.Count == 0) candidates = possibleRepliers;

                Stalker replier = candidates[_rng.Next(candidates.Count)];
                string replyText = LoreDatabase.GetContextualReply(replier, firstSpeaker, previousPhrase);

                _pendingReplies.Add(new PendingReply
                {
                    ExecuteTime = DateTime.Now.AddSeconds(delaySeconds),
                    Author = replier,
                    Text = replyText
                });

                previousSpeaker = replier;
                previousPhrase = replyText;
                delaySeconds += _rng.Next(4, 12);
            }

            if (_rng.Next(0, 100) < 15)
            {
                Stalker gabela = LoreDatabase.GetCharacterByName("Габела");
                string closePhrase = LoreDatabase.GetContextualReply(gabela, firstSpeaker, triggerPhrase);
                _pendingReplies.Add(new PendingReply
                {
                    ExecuteTime = DateTime.Now.AddSeconds(delaySeconds + 3),
                    Author = gabela,
                    Text = closePhrase
                });
            }
        }

        private void SendMessage(Stalker author, string text)
        {
            var msg = new PdaMessage(author.Name, author.Faction, text);
            OnMessageGenerated?.Invoke(this, msg);
        }
    }
}