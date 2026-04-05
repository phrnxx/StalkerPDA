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
        private bool _isRunning = false;
        private Random _rng = new Random();
        private List<PendingReply> _pendingReplies = new List<PendingReply>();
        private const int TICK_RATE_MS = 5000;

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
                    await Task.Delay(1000);
                }

                if (_pendingReplies.Count == 0)
                {
                    GenerateRandomEvent();
                }

                await Task.Delay(TICK_RATE_MS);
            }
        }

        private void GenerateRandomEvent()
        {
            Stalker activeStalker = LoreDatabase.GetRandomActiveCharacter();
            string phrase = LoreDatabase.GetPhraseForStalker(activeStalker);
            SendMessage(activeStalker, phrase);

            if (_rng.Next(0, 100) < 40)
            {
                CreateReplyChain(activeStalker);
            }
        }

        private void CreateReplyChain(Stalker firstSpeaker)
        {
            Stalker replier;
            string replyText = "";

            if (firstSpeaker.Faction == "Вільні" || firstSpeaker.Faction == "Іскра")
            {
                replier = LoreDatabase.GetCharacterByName("Габела");
                replyText = $"Не слухай нікого, {firstSpeaker.Name}. Заходь до мене, є свіжа ковбаса.";
            }
            else
            {
                replier = LoreDatabase.GetCharacterByName("Ріхтер");
                replyText = "Згоден з тобою на всі сто.";
            }

            _pendingReplies.Add(new PendingReply
            {
                ExecuteTime = DateTime.Now.AddSeconds(_rng.Next(5, 12)),
                Author = replier,
                Text = replyText
            });
        }

        private void SendMessage(Stalker author, string text)
        {
            var msg = new PdaMessage(author.Name, author.Faction, text);
            OnMessageGenerated?.Invoke(this, msg);
        }
    }
}