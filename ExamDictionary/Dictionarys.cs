using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary
{
    public abstract class BaseDictionary : IAddable, IDeletable, IReplaceable, ISearchable
    {
        protected Dictionary<string, List<string>> Words = new();
        public abstract void AddWord(string activeDictionaryType);
        public abstract void DeleteWord(string activeDictionaryType);
        public abstract void DeleteTranslation(string activeDictionaryType);
        public abstract void ReplaceWord(string activeDictionaryType);
        public abstract void ReplaceTranslation(string activeDictionaryType);
        public abstract void SearchTranslations(string word);
        public abstract void TranslatePhrase();

        public virtual Dictionary<string, List<string>> ToDictionary()
        {
            return new Dictionary<string, List<string>>();
        }
    }

    public class CustomDictionary : BaseDictionary
    {
        private string dictionaryType;

        public CustomDictionary(string dictionaryType)
        {
            this.dictionaryType = dictionaryType;
            FileStorage storage = new FileStorage();
            Words = storage.Load($"{dictionaryType}.json");

        }
        public override void AddWord(string activeDictionaryType)
        {
            string word = GetInput("Введіть слово: ", "Слово не може бути порожнім або складатися лише з цифр.");
            word = word.ToLower();

            string translation = GetInput("Введіть переклад: ", "Переклад не може бути порожнім або складатися лише з цифр.");
            var translations = translation
                .Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .ToList();

            if (!Words.TryGetValue(word, out List<string>? existingTranslations))
            {
                Words[word] = new List<string>();
                existingTranslations = Words[word];
            }

            var addedTranslations = translations.Except(existingTranslations).ToList();
            var duplicateTranslations = translations.Intersect(existingTranslations).ToList();

            existingTranslations.AddRange(addedTranslations);

            if (addedTranslations.Any())
            {
                Console.WriteLine($"До слова \"{word}\" додано унікальні переклади: {string.Join(", ", addedTranslations)}");
            }

            if (duplicateTranslations.Any())
            {
                Console.WriteLine($"Переклади \"{string.Join(", ", duplicateTranslations)}\" вже існують для слова \"{word}\"");
            }

            new FileStorage().Save(Words, activeDictionaryType);
            Console.WriteLine("Словник успішно збережений у файл.");
        }

        private static string GetInput(string text, string errorMessage)
        {
            string? input;
            do
            {
                Console.Write(text);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.All(char.IsDigit))
                {
                    Console.WriteLine(errorMessage);
                }
            } while (string.IsNullOrWhiteSpace(input) || input.All(char.IsDigit));

            return input;
        }

        public override void DeleteWord(string activeDictionaryType)
        {
            string word = GetInput("Введіть слово для видалення: ", "Слово не може бути порожнім або складатися лише з цифр.");
            word = word.Trim().ToLower();

            if (Words.Remove(word))
            {
                Console.Write($"Ви впевнені, що хочете видалити слово \"{word}\"? (Так/Ні): ");
                string? confirmation = Console.ReadLine()?.Trim().ToLower();

                if (confirmation == "так" || confirmation == "yes")
                {
                    new FileStorage().Save(Words, activeDictionaryType);
                    Console.WriteLine($"Слово \"{word}\" успішно видалено.");
                }
                else
                {
                    Console.WriteLine("Видалення скасовано.");
                }
            }
            else
            {
                Console.WriteLine($"Слово \"{word}\" не знайдено у словнику.");
            }
        }

        public override void DeleteTranslation(string activeDictionaryType)
        {
            string word = GetInput("Введіть слово: ", "Слово не може бути порожнім або складатися лише з цифр. Введіть слово ще раз:");
            if (!Words.TryGetValue(word.Trim().ToLower(), out List<string>? translations) || !translations.Any())
            {
                Console.WriteLine($"Слово \"{word}\" не знайдено у словнику.");
                return;
            }

            Console.WriteLine($"Доступні переклади для слова \"{word}\": {string.Join(", ", translations)}");

            string translation = GetInput("Введіть переклад для видалення: ", "Переклад не може бути порожнім або складатися лише з цифр. Введіть переклад ще раз:");

            if (translations.Count == 1)
            {
                Console.WriteLine($"Неможливо видалити переклад \"{translation}\" для слова \"{word}\", оскільки це єдиний переклад. Слово повинно мати принаймні один переклад.");
            }
            else
            {
                Console.Write($"Ви впевнені, що хочете видалити переклад \"{translation}\" для слова \"{word}\"? (Так/Ні): ");
                string? confirmation = Console.ReadLine()?.Trim().ToLower();

                if (confirmation == "так" || confirmation == "yes")
                {
                    if (translations.Remove(translation.Trim().ToLower()))
                    {
                        Console.WriteLine($"Переклад \"{translation}\" для слова \"{word}\" успішно видалено.");
                    }
                    else
                    {
                        Console.WriteLine($"Переклад \"{translation}\" не знайдено для слова \"{word}\".");
                    }
                }
                else
                {
                    Console.WriteLine("Видалення скасовано.");
                }
            }

            new FileStorage().Save(Words, activeDictionaryType);
            Console.WriteLine("Словник успішно збережений у файл.");
        }

        
        public override void ReplaceWord(string activeDictionaryType)
        {
            string oldWord = GetInput("Введіть старе слово: ", "Слово не може бути порожнім або складатися лише з цифр. Введіть старе слово ще раз:");

            if (!Words.ContainsKey(oldWord))
            {
                Console.WriteLine("Слово не знайдено");
                return;
            }
            var translations = Words[oldWord];

            string newWord = GetInput("Введіть нове слово: ", "Нове слово не може бути порожнім або складатися лише з цифр. Введіть нове слово ще раз:");
            if (Words.ContainsKey(newWord))
            {
                Console.WriteLine($"Слово \"{newWord}\" вже існує у словнику. Введіть інше нове слово.");
                return;
            }

            Words.Remove(oldWord);
            Words[newWord] = translations;

            Console.WriteLine($"Слово \"{oldWord}\" змінено на \"{newWord}\".");

            new FileStorage().Save(Words, activeDictionaryType);
            Console.WriteLine("Словник успішно збережений у файл.");
        }


        public override void ReplaceTranslation(string activeDictionaryType)
        {
            string word = GetInput("Введіть слово: ", "Слово не може бути порожнім. Будь ласка, введіть слово ще раз.");

            if (!Words.ContainsKey(word))
            {
                Console.WriteLine("Слово не знайдено");
                return;
            }

            List<string> translations = Words[word];
            Console.WriteLine($"Список перекладів для слова \"{word}\": {string.Join(", ", translations)}");

            Console.WriteLine("Ви хочете замінити (1) всі переклади або (2) конкретний переклад? (введіть 1 або 2):");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть новий переклад для всіх варіантів: ");
                    string? newTranslationsInput = Console.ReadLine();

                    if (!string.IsNullOrEmpty(newTranslationsInput))
                    {
                        Words[word] = newTranslationsInput
                            .Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .ToList();

                        Console.WriteLine($"Усі переклади для слова \"{word}\" успішно змінено.");
                    }
                    else
                    {
                        Console.WriteLine("Помилка: введено порожній переклад.");
                    }
                    break;

                case "2":
                    Console.Write("Введіть старий варіант перекладу для заміни: ");
                    string? oldTranslation = Console.ReadLine();

                    if (!string.IsNullOrEmpty(oldTranslation) && translations.Contains(oldTranslation))
                    {
                        Console.Write("Введіть новий варіант перекладу: ");
                        string? newTranslation = Console.ReadLine();

                        if (!string.IsNullOrEmpty(newTranslation))
                        {
                            translations[translations.IndexOf(oldTranslation)] = newTranslation;
                            Console.WriteLine($"Варіант перекладу \"{oldTranslation}\" успішно змінено на \"{newTranslation}\".");
                        }
                        else
                        {
                            Console.WriteLine("Новий варіант перекладу не може бути порожнім.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Старий варіант перекладу не знайдено або введено порожній рядок.");
                    }
                    break;

                default:
                    Console.WriteLine("Невірний вибір. Будь ласка, введіть 1 або 2.");
                    return;
            }

            new FileStorage().Save(Words, activeDictionaryType);
            Console.WriteLine("Словник успішно збережений у файл.");
        }


        public override void SearchTranslations(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine("Слово не може бути порожнім. Будь ласка, введіть слово ще раз.");
                return;
            }

            word = word.Trim().ToLower();

            if (Words.TryGetValue(word, out var translations))
            {
                Console.WriteLine($"Слово: {word}");
                Console.WriteLine("Переклади:");
                foreach (var translation in translations)
                {
                    Console.WriteLine($"- {translation}");
                }

                Console.Write("\nБажаєте зберегти результат у файл? (Так/Ні): ");
                string? response = Console.ReadLine()?.Trim().ToLower();

                if (response == "так" || response == "yes")
                {
                    Console.Write("Введіть назву файлу для збереження: ");
                    string? fileName = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        var fileStorage = new FileStorage();
                        var existingDictionary = fileStorage.Load($"{fileName}.json");

                        if (existingDictionary.ContainsKey(word))
                        {
                            var existingTranslations = existingDictionary[word];
                            foreach (var translation in translations)
                            {
                                if (!existingTranslations.Contains(translation))
                                {
                                    existingTranslations.Add(translation);
                                }
                            }
                        }
                        else
                        {
                            existingDictionary[word] = new List<string>(translations);
                        }

                        fileStorage.Save(existingDictionary, fileName);
                        Console.WriteLine("Результати збережені успішно.");
                    }
                    else
                    {
                        Console.WriteLine("Назва файлу не може бути порожньою.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Слово не знайдено.");
            }
        }

        public override void TranslatePhrase()
        {
            string phrase = GetInput("Введіть фразу для перекладу: ", "Фраза не може бути порожньою.");

            // пошук слів та зкаків, зберігаючі пробіли
            var wordsInPhrase = System.Text.RegularExpressions.Regex.Matches(phrase, @"\S+|\s+")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Value)
                .ToArray();

            List<string> translatedWords = new List<string>(); 

            foreach (var word in wordsInPhrase)
            {
                string lowerWord = word.ToLower();

                if (string.IsNullOrWhiteSpace(word))
                {
                    translatedWords.Add(word); 
                }
                else if (char.IsPunctuation(word[0]))
                {
                    translatedWords.Add(word); //додаємо знаки пунктуації
                }
                else if (Words.TryGetValue(lowerWord, out var translations) && translations.Any())
                {
                    translatedWords.Add(translations.First()); // додаємо перекладені слова
                }
                else
                {
                    translatedWords.Add(word); 
                    Console.WriteLine($"Переклад для слова \"{word}\" не знайдено.");
                }
            }

            // створюємо фразу знов, зі всіма знаками і пробілами
            StringBuilder resultBuilder = new StringBuilder();
            int index = 0;

            foreach (var word in wordsInPhrase)
            {
                resultBuilder.Append(translatedWords[index]);
                index++;
                if (index < wordsInPhrase.Length && char.IsWhiteSpace(phrase[phrase.IndexOf(word) + word.Length]))
                {
                    resultBuilder.Append(" ");
                }
            }

            string result = resultBuilder.ToString();
            if (result.Length > 0)
            {
                result = char.ToUpper(result[0]) + result.Substring(1); 
            }

            Console.WriteLine("Переклад фрази:");
            Console.WriteLine(result);

        
            Console.Write("\nБажаєте зберегти результат у файл? (Так/Ні): ");
            string? response = Console.ReadLine()?.Trim().ToLower();

      
            if (response == "так" || response == "yes")
            {
                Console.Write("Введіть назву файлу для збереження: ");
                string? fileName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var fileStorage = new FileStorage(); 
                    var existingDictionary = fileStorage.Load($"{fileName}.json"); 

                    string entryKey = phrase; 
                    existingDictionary[entryKey] = new List<string> { result }; 

                    fileStorage.Save(existingDictionary, fileName); 
                    Console.WriteLine("Результати збережені успішно."); 
                }
                else
                {
                    Console.WriteLine("Назва файлу не може бути порожньою."); 
                }
            }
        }



    }
    public class Word
    {
        public string? Term { get; set; }
        public List<string> Translations { get; set; } = new List<string>();
    }
}
