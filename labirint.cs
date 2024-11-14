using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static int screenWidth;
    static int screenHeight;
    static char[,] maze;
    static Point player;
    static List<Point> treasures;
    static Random random = new Random();
    static int score = 0;
    static bool gameRunning = true;

    static void Main()
    {
        Console.CursorVisible = false;

        // Получение размеров лабиринта от пользователя
        Console.Write("Введите ширину лабиринта: ");
        screenWidth = int.Parse(Console.ReadLine());

        Console.Write("Введите высоту лабиринта: ");
        screenHeight = int.Parse(Console.ReadLine());

        // Инициализация и генерация лабиринта
        InitializeMaze();
        GenerateMaze();
        PlaceTreasures(5); // Разместить 5 сокровищ в лабиринте

        do
        {
            while (gameRunning)
            {
                Input();
                Draw();
                Thread.Sleep(100); // Регулировка скорости игры
            }

            Console.SetCursorPosition(0, screenHeight + 1);
            Console.WriteLine($"Игра окончена! Ваш счёт: {score}");

            Console.WriteLine("Нажмите любую клавишу для начала новой игры или Esc для выхода.");
        }
        while (Console.ReadKey(true).Key != ConsoleKey.Escape);
    }

    static void InitializeMaze()
    {
        maze = new char[screenHeight, screenWidth];
        player = new Point(1, 1); // Начальная позиция игрока
        treasures = new List<Point>();

        // Заполнение лабиринта стенами
        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                if (x == 0 || y == 0 || x == screenWidth - 1 || y == screenHeight - 1)
                {
                    maze[y, x] = '■'; // Стена
                }
                else
                {
                    maze[y, x] = ' '; // Пустое пространство
                }
            }
        }
    }

    static void GenerateMaze()
    {
        // Пример простого лабиринта. Можно заменить на более сложный алгоритм генерации
        for (int i = 0; i < screenHeight; i++)
        {
            for (int j = 0; j < screenWidth; j++)
            {
                if (random.Next(100) < 20 && (i > 1 && i < screenHeight - 2) && (j > 1 && j < screenWidth - 2))
                {
                    maze[i, j] = '■'; // Случайное размещение стен
                }
            }
        }
        maze[player.Y, player.X] = 'P'; // Позиция игрока
    }

    static void PlaceTreasures(int count)
    {
        treasures.Clear();
        for (int i = 0; i < count; i++)
        {
            Point treasure;
            do
            {
                treasure = new Point(random.Next(1, screenWidth - 1), random.Next(1, screenHeight - 1));
            } while (maze[treasure.Y, treasure.X] != ' ' || treasure.Equals(player) || treasures.Contains(treasure));

            maze[treasure.Y, treasure.X] = 'T'; // Размещение сокровища
            treasures.Add(treasure);
        }
    }

    static void Input()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            Point newPosition = player;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    newPosition = new Point(player.X, player.Y - 1);
                    break;
                case ConsoleKey.DownArrow:
                    newPosition = new Point(player.X, player.Y + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    newPosition = new Point(player.X - 1, player.Y);
                    break;
                case ConsoleKey.RightArrow:
                    newPosition = new Point(player.X + 1, player.Y);
                    break;
            }

            if (maze[newPosition.Y, newPosition.X] != '■')
            {
                if (maze[newPosition.Y, newPosition.X] == 'T')
                {
                    score += 10;
                    treasures.Remove(newPosition);
                    if (treasures.Count == 0)
                    {
                        gameRunning = false; // Завершить игру, если все сокровища собраны
                    }
                }
                maze[player.Y, player.X] = ' ';
                player = newPosition;
                maze[player.Y, player.X] = 'P';
            }
        }
    }

    static void Draw()
    {
        Console.Clear();
        for (int y = 0; y < screenHeight; y++)
        {
            for (int x = 0; x < screenWidth; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(maze[y, x]);
            }
        }
        Console.SetCursorPosition(0, screenHeight);
        Console.WriteLine($"Счёт: {score}");
    }

    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return X == p.X && Y == p.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
