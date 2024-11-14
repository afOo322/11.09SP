using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

class Program
{
    private static Mutex mutex = new Mutex(); // Объект для синхронизации потоков
    private static System.Timers.Timer timer; // Таймер для автоматического обновления файла
    private static int currentId = 1; // Переменная для хранения текущего идентификатора
    private static string lastUserInput = string.Empty; // Переменная для хранения последнего ввода пользователя

    static async Task Main()
    {
        // 1. Запускаем таймер для автоматического обновления файла каждые 5 секунд
        timer = new System.Timers.Timer(5000); // Интервал 5 секунд
        timer.Elapsed += async (sender, e) => await UpdateFile(); // Подписка на событие таймера
        timer.Start(); // Запуск таймера

        // 2. Запускаем поток для обработки ввода пользователя
        Thread inputThread = new Thread(HandleUserInput);
        inputThread.Start(); // Запускаем поток

        // Ожидаем завершения программы
        Console.ReadLine(); // Это не завершит работу, пока пользователь не введет что-то
    }

    // Функция для обработки пользовательского ввода
    static void HandleUserInput()
    {
        while (true) // Бесконечный цикл для ожидания ввода
        {
            // Ожидание строки ввода от пользователя
            Console.WriteLine("Введите строку для добавления в файл (или 'exit' для выхода):");
            string input = Console.ReadLine(); // Чтение введённой строки

            if (input.ToLower() == "exit") // Если пользователь введёт 'exit', завершить цикл
            {
                break;
            }

            // Записываем введённые пользователем данные в файл
            using (StreamWriter sw = File.AppendText("data.txt"))
            {
                sw.WriteLine(input); // Добавляем строку в файл
            }

            lastUserInput = input; // Сохраняем последний ввод пользователя
            Console.WriteLine($"Данные '{input}' добавлены в файл."); // Сообщаем, что данные добавлены
        }
    }

    // Функция для обновления файла при срабатывании таймера
    private static async Task UpdateFile()
    {
        try
        {
            // Открываем файл для дозаписи и добавляем новую строку с текущей датой и временем
            using (StreamWriter sw = File.AppendText("data.txt"))
            {
                sw.WriteLine($"Автообновление: {DateTime.Now}"); // Записываем время в файл
            }
            Console.WriteLine("Файл был автоматически обновлен."); // Сообщаем об обновлении файла

            // Логирование информации об обновлении файла
            LogFileUpdate();

            // Получаем и выводим данные с сервера
            string data = await GetDataFromServer(currentId);
            Console.WriteLine("Полученные данные с сервера:");
            Console.WriteLine(data); // Выводим данные, полученные от сервера

            // Увеличиваем идентификатор для следующего запроса
            currentId++;
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка обновления файла: " + ex.Message);
        }
    }

    // Метод для логирования информации об обновлении файла
    private static void LogFileUpdate()
    {
        try
        {
            using (StreamWriter logWriter = File.AppendText("log.txt"))
            {
                logWriter.WriteLine($"{DateTime.Now}: Файл 'data.txt' был обновлен.");

                if (!string.IsNullOrEmpty(lastUserInput))
                {
                    logWriter.WriteLine($"Последний ввод пользователя: {lastUserInput}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка записи в лог-файл: " + ex.Message);
        }
    }

    // Асинхронная функция для выполнения HTTP-запроса к серверу
    static async Task<string> GetDataFromServer(int id)
    {
        HttpClient client = new HttpClient(); // Создаем HTTP-клиент для отправки запросов
        try
        {
            // Асинхронно отправляем запрос и получаем ответ от сервера по указанному идентификатору
            string url = $"https://jsonplaceholder.typicode.com/posts/{id}";
            string result = await client.GetStringAsync(url);
            return result; // Возвращаем результат запроса
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Ошибка при отправке HTTP-запроса: " + e.Message);
            return "Ошибка при запросе данных с сервера"; // Возвращаем сообщение об ошибке
        }
    }
}
