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
                    await Task.Delay(TICK_RATE_MS);
                    GenerateRandomEvent();
                }
                else
                {
                    await Task.Delay(TICK_RATE_MS);
                }
            }
        }

        private void GenerateRandomEvent()
        {
            Stalker activeStalker = LoreDatabase.GetRandomActiveCharacter();
            string phrase = LoreDatabase.GetPhraseForStalker(activeStalker);
            SendMessage(activeStalker, phrase);

            if (_rng.Next(0, 100) < 40)
            {
                CreateReplyChain(activeStalker, phrase);
            }
        }

        private void CreateReplyChain(Stalker firstSpeaker, string triggerPhrase)
        {
            var possibleRepliers = LoreDatabase.GetPossibleRepliers(firstSpeaker.Faction);

            if (possibleRepliers.Count == 0) return;

            int chainLength = _rng.Next(1, 4);
            int delaySeconds = _rng.Next(5, 12);
            Stalker previousSpeaker = firstSpeaker;
            string previousPhrase = triggerPhrase;

            for (int i = 0; i < chainLength; i++)
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
                delaySeconds += _rng.Next(4, 10);
            }

            if (_rng.Next(0, 100) < 20)
            {
                Stalker gabela = LoreDatabase.GetCharacterByName("Габела");
                string closePhrase = LoreDatabase.GetContextualReply(gabela, firstSpeaker, triggerPhrase);
                _pendingReplies.Add(new PendingReply
                {
                    ExecuteTime = DateTime.Now.AddSeconds(delaySeconds + 5),
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