using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private static List<string> names = new List<string>
    {
        "Anna", "Jonathan", "Canada", "Andrew", "Banana", "Alan"
    };
    private static string currentSearch = string.Empty; // Переменная для хранения текущего ввода пользователя

    static void Main()
    {
        // Инициализация экрана
        Console.Clear();
        DisplayList(names);

        // Основной цикл для обработки ввода пользователя
        while (true)
        {
            // Обновление экрана с текущим поисковым запросом
            UpdateScreen();

            // Показ приглашения и получение ввода пользователя
            Console.WriteLine("\nВведите строку для поиска (или 'exit' для завершения):");
            Console.Write($"Поиск: '{currentSearch}'"); // Показываем текущий запрос

            ConsoleKeyInfo keyInfo;
            while (true)
            {
                keyInfo = Console.ReadKey(intercept: true); // Читаем клавишу без отображения в консоли

                if (keyInfo.Key == ConsoleKey.Enter) // При нажатии Enter выходим из внутреннего цикла
                {
                    Console.WriteLine(); // Для перевода строки после ввода
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace) // При нажатии Backspace удаляем последний символ
                {
                    if (currentSearch.Length > 0)
                    {
                        // Удаляем последний символ из текущего запроса
                        currentSearch = currentSearch.Remove(currentSearch.Length - 1);
                        // Обновляем экран
                        UpdateScreen();
                        // Убираем последний символ из консоли
                        Console.Write("\b \b"); // Удаляем последний символ в консоли
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape) // При нажатии Escape очищаем текущий запрос
                {
                    currentSearch = string.Empty;
                    UpdateScreen();
                }
                else if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsWhiteSpace(keyInfo.KeyChar)) // Для ввода символов
                {
                    currentSearch += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar); // Отображаем символ в консоли
                }
            }
        }
    }

    // Метод для обновления экрана
    static void UpdateScreen()
    {
        Console.Clear();
        DisplayList(names);
        DisplayResults(SearchStrings(names, currentSearch), currentSearch);
    }

    // Метод для отображения списка строк
    static void DisplayList(List<string> list)
    {
        Console.WriteLine("Список строк:");
        foreach (var name in list)
        {
            Console.WriteLine(name);
        }
    }

    // Метод для отображения результатов поиска
    static void DisplayResults(List<string> results, string search)
    {
        Console.WriteLine($"\nРезультаты поиска по '{search}':");
        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                // Подсветка совпадений
                string highlighted = HighlightMatch(result, search);
                Console.WriteLine(highlighted);
            }
        }
        else
        {
            Console.WriteLine("Совпадений не найдено.");
        }
    }

    // Функция для поиска строк
    static List<string> SearchStrings(List<string> list, string search)
    {
        return list.Where(s => s.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
    }

    // Функция для подсветки совпадений
    static string HighlightMatch(string text, string search)
    {
        int index = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            return text.Substring(0, index) +
                   "\x1b[42;30m" + text.Substring(index, search.Length) + "\x1b[0m" +
                   text.Substring(index + search.Length);
        }
        return text;
    }
}
