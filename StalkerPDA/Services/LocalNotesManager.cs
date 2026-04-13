using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StalkerPDA.Models;

namespace StalkerPDA.Services
{
    public static class LocalNotesManager
    {
        private static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pda_local_notes.json");

        public static List<PdaQuest> GetNotes()
        {
            if (!File.Exists(FilePath)) return new List<PdaQuest>();
            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<PdaQuest>>(json) ?? new List<PdaQuest>();
            }
            catch { return new List<PdaQuest>(); }
        }

        public static void AddNote(string text)
        {
            var notes = GetNotes();
            notes.Add(new PdaQuest
            {
                Id = Guid.NewGuid().ToString(),
                Title = text,
                Deadline = "ОСОБИСТЕ",
                IsGoogleTask = false
            });

            File.WriteAllText(FilePath, JsonSerializer.Serialize(notes));
        }

        public static void RemoveNote(string id)
        {
            var notes = GetNotes();
            notes.RemoveAll(n => n.Id == id);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(notes));
        }
    }
}