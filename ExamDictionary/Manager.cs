using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dictionary
{
    public class DictionaryFactory
    {
        public BaseDictionary CreateDictionary(string type)
        {
            return new CustomDictionary(type);
        }
    }

    public class DictionaryManager
    {
        private BaseDictionary dictionary;

        public DictionaryManager(BaseDictionary dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public void AddWord(string activeDictionaryType) => dictionary.AddWord(activeDictionaryType);
        public void DeleteWord(string activeDictionaryType) => dictionary.DeleteWord(activeDictionaryType);
        public void DeleteTranslation(string activeDictionaryType) => dictionary.DeleteTranslation(activeDictionaryType);
        public void ReplaceWord(string activeDictionaryType) => dictionary.ReplaceWord(activeDictionaryType);
        public void ReplaceTranslation(string activeDictionaryType) => dictionary.ReplaceTranslation(activeDictionaryType);
        public void SearchTranslations(string word) => dictionary.SearchTranslations(word);
        public void TranslatePhrase() =>  dictionary.TranslatePhrase();
    }

    public class FileStorage : IStorage
    {
        private const string dictionaryListFile = "dictionary_list.json";

        public void Save(Dictionary<string, List<string>> newWords, string dictionaryType)
        {
            if (newWords == null || string.IsNullOrWhiteSpace(dictionaryType))
            {
                throw new ArgumentException("Словник або його тип не може бути пустим");
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var existingDictionary = Load(dictionaryType);

            foreach (var kvp in newWords)
            {
                if (existingDictionary.TryGetValue(kvp.Key, out List<string>? existingTranslations))
                {
                    foreach (var translation in kvp.Value)
                    {
                        if (!existingTranslations.Contains(translation))
                        {
                            existingTranslations.Add(translation);
                        }
                    }
                }
                else
                {
                    existingDictionary[kvp.Key] = new List<string>(kvp.Value);
                }
            }

            string jsonString = JsonSerializer.Serialize(existingDictionary, options);
            string fileName = $"{dictionaryType}.json"; 
            File.WriteAllText(fileName, jsonString);
            Console.WriteLine($"Зберігаю файл: {fileName}");
        }

        public Dictionary<string, List<string>> Load(string dictionaryType)
        {
            if (File.Exists(dictionaryType))
            {
                string jsonString = File.ReadAllText(dictionaryType);
                return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString) ?? new Dictionary<string, List<string>>();
            }

            return new Dictionary<string, List<string>>();
        }


        public void SaveDictionaryList(List<string> dictionaryTypes)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(dictionaryTypes, options);
            File.WriteAllText(dictionaryListFile, jsonString);
        }

        public List<string> LoadDictionaryList()
        {
            if (File.Exists(dictionaryListFile))
            {
                string jsonString = File.ReadAllText(dictionaryListFile);
                return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            }
            return new List<string>();
        }
    }
}
