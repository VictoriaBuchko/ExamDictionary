namespace Dictionary
{
    public class Menu
    {
        private DictionaryManager? manager;
        private DictionaryFactory factory;
        private string activeDictionaryType;
        private List<string> existingDictionaryTypes;
        private FileStorage storage;

        public Menu()
        {
            factory = new DictionaryFactory();
            storage = new FileStorage();
            existingDictionaryTypes = storage.LoadDictionaryList();
            activeDictionaryType = string.Empty;
            manager = null; 
        }

        public void DisplayMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("             Головне меню");
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("1. Створити новий словник");
                Console.WriteLine("2. Відкрити існуючий словник");
                Console.WriteLine("0. Вихід");
                Console.Write("Виберіть опцію: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateNewDictionary();
                        break;
                    case "2":
                        ViewExistingDictionaries();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }

        private void CreateNewDictionary()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("      Створення власного словника     ");
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("Правила назви типу словника:");
                Console.WriteLine("- Назва не повинна починатися з цифри.");
                Console.WriteLine("- Назва не повинна містити знаків пунктуації (.,:;!? та інші).");
                Console.WriteLine("- Назва має бути обов'язково на англійській мові.");
                Console.WriteLine("Введіть '0' для виходу в меню.\n");

                Console.Write("Введіть назву типу словника: ");
                string? customDictionaryType = Console.ReadLine();

                if (customDictionaryType == "0")
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(customDictionaryType) &&
                    !char.IsDigit(customDictionaryType[0]) && // превірка на першу цифру
                    customDictionaryType.All(c => char.IsLetterOrDigit(c) || c == ' ') && // перевірка на знаки пунктуації
                    customDictionaryType.All(c => c < 128)) // перевірка на англійські символи
                {
                    if (!existingDictionaryTypes.Contains(customDictionaryType))
                    {
                        existingDictionaryTypes.Add(customDictionaryType);
                        storage.SaveDictionaryList(existingDictionaryTypes);

                        activeDictionaryType = customDictionaryType;
                        var dictionary = factory.CreateDictionary(customDictionaryType);
                        manager = new DictionaryManager(dictionary);

                        storage.Save(dictionary.ToDictionary(), customDictionaryType);
                        Console.WriteLine($"\nСловник '{customDictionaryType}' створено успішно.");

                        DisplayMenu();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"\nСловник з назвою '{customDictionaryType}' вже існує. Спробуйте ввести іншу назву.");
                        Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
                        Console.ReadKey();
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("\nНазва типу словника не відповідає правилам. Спробуйте ще раз.");
                    Console.WriteLine("Натисніть будь-яку клавішу для продовження або введіть '0' для виходу в меню...");
                    Console.ReadKey();
                }
            }
        }

        private void ViewExistingDictionaries()
        {
            Console.Clear();
            if (existingDictionaryTypes.Any())
            {
                Console.WriteLine(new string('=', 40));
                Console.WriteLine("Существующие словари:");
                Console.WriteLine(new string('=', 40));
                foreach (var dictionaryType in existingDictionaryTypes)
                {
                    Console.WriteLine($"- {dictionaryType}");
                }

                Console.Write("Введите тип словаря, чтобы открыть его (или '0' для выхода): ");
                string? type = Console.ReadLine();
                if (type?.ToLower() == "0") return;

                if (!string.IsNullOrEmpty(type) && existingDictionaryTypes.Contains(type))
                {
                    activeDictionaryType = type;
                    var dictionary = factory.CreateDictionary(type);
                    manager = new DictionaryManager(dictionary);
                    DisplayMenu();
                }
                else
                {
                    Console.WriteLine("Словарь с таким типом не найден.");
                    WaitForUser();
                }
            }
            else
            {
                Console.WriteLine("Словарей пока нет.");
                WaitForUser();
            }
        }
        public void DisplayMenu()
        {
            while (true)
            {
                DrawMenu();

                Console.SetCursorPosition(0, 13);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, 13);

                Console.Write("Виберіть опцію (1-7): ");
                string? choice = Console.ReadLine();

                Console.SetCursorPosition(0, 14);
                Console.Write(new string(' ', Console.WindowWidth * 10));
                Console.SetCursorPosition(0, 14);
                switch (choice)
                {
                    case "1":
                        manager.AddWord(activeDictionaryType);
                        WaitForUser();
                        break;

                    case "2":
                        manager.DeleteWord(activeDictionaryType);
                        WaitForUser();
                        break;

                    case "3":
                        manager.DeleteTranslation(activeDictionaryType);
                        WaitForUser();
                        break;

                    case "4":
                        manager.ReplaceWord(activeDictionaryType);
                        WaitForUser();
                        break;

                    case "5":
                        manager.ReplaceTranslation(activeDictionaryType);
                        WaitForUser();
                        break;

                    case "6":
                        Console.Write("Введіть слово для пошуку перекладів: ");
                        string? wordToSearch = Console.ReadLine();

                        if (!string.IsNullOrEmpty(wordToSearch))
                        {
                            manager.SearchTranslations(wordToSearch);
                        }
                        else
                        {
                            Console.WriteLine("Будь ласка, введіть коректне слово.");
                        }
                        WaitForUser();
                        break;
                    case "7":
                        manager.TranslatePhrase();
                        WaitForUser();
                        break;
                    case "8":
                        Console.WriteLine("Меню");
                        return;
                    case "9":
                        Environment.Exit(0);
                        return;
                    default:
                        Console.WriteLine("Невірний вибір, спробуйте ще раз.");
                        WaitForUser();
                        break;
                }
                Console.SetCursorPosition(0, 10);
                Console.Write(new string(' ', Console.WindowWidth * 10));
                Console.SetCursorPosition(0, 10);
            }
        }

        private void DrawMenu()
        {
            Console.Clear(); 
            Console.WriteLine(new string('=', 40));
            Console.WriteLine("         Головне меню          ");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"Поточний словник: {activeDictionaryType}");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine("1. Додати слово");
            Console.WriteLine("2. Видалити слово");
            Console.WriteLine("3. Видалити переклад слова");
            Console.WriteLine("4. Змінити слово");
            Console.WriteLine("5. Змінити переклад слова");
            Console.WriteLine("6. Пошук перекладів слова");
            Console.WriteLine("7. Пошук перекладу фрази");
            Console.WriteLine("8. Меню");
            Console.WriteLine("9. Вийти з програми");
            Console.WriteLine(new string('=', 40));
        }

        private void WaitForUser()
        {
            Console.WriteLine("\nНатисніть будь-яку кнопку для продовження...");
            Console.ReadKey();
        }
    }
}

