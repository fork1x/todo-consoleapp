using System.Text;
using Newtonsoft.Json;

namespace ToDoApp
{
    class Program
    {
        List<Task> tasks = new List<Task>();
        Random rnd = new Random();
        private readonly string todoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        private void Setup()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            
            Console.Title = "TODO app";
            Console.CursorVisible = false;

            try
            {
                int windowWidth = 70;
                int windowHeight = 20;

                Console.WindowWidth = windowWidth;
                Console.BufferWidth = windowWidth;
                Console.WindowHeight = windowHeight;
                Console.BufferHeight = windowHeight;
            }
            catch(Exception ex)
            {
                Alert(ex.ToString());
            }

            Console.Clear();
        }


        private static void CenterText(string text)
        {
            int indent = ((Console.WindowWidth - 1) - text.Length) / 2;
            string centered = text.PadLeft(indent + text.Length);
            string full = centered.PadRight(Console.WindowWidth - 1);
            Console.WriteLine(full);
        }


        private void Menu()
        {
            int selectedIndex = 0;
            string[] options =
            {
                "[+]  Добавить задачу",
                "[-]  Удалить задачу",
                "[*]  Изменить статус",
                "[=]  Показать все задачи",
                "[x]  Выйти"
            };

            Console.Clear();
            StageTitle("Главное меню");
            CenterText("Используйте ↑ и ↓ для перемещения по меню.");
            CenterText("Чтобы перейти дальше, нажмите Enter.");
            Console.WriteLine();

            int menuStartRow = Console.CursorTop;

            while (true)
            {
                Console.SetCursorPosition(0, menuStartRow);

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        if (i == options.Length - 1)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        CenterText($"> {options[i]} <");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Обычный вид невыбранных пунктов
                        CenterText($"{options[i]}");
                    }
                }

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if(selectedIndex < 0) selectedIndex = options.Length - 1;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= options.Length) selectedIndex = 0;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    switch (selectedIndex)
                    {
                        case 0: AddTask(); break;
                        case 1: DeleteTask(); break;
                        case 2: ChangeStatusTask(); break;
                        case 3: ShowAllTasks(); break;
                        case 4: return;
                    }
                    Console.Clear();
                    StageTitle("Главное меню");
                    CenterText("Используйте ↑ и ↓ для перемещения по меню.");
                    CenterText("Чтобы перейти дальше, нажмите Enter.\n");
                    menuStartRow = Console.CursorTop;
                }
            }
        }


        private void AddTask()
        {
            Console.Clear();
            StageTitle("Добавить задачу");

            Console.Write("Введите название задачи: ");
            string Title = Console.ReadLine().Trim();
            
            bool TaskAlreadyExist = false;
            foreach (var task in tasks)
            {
                if (Title.ToLower() == task.Title.ToLower())
                {
                    TaskAlreadyExist = true;
                }
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                Alert("Название задачи не может быть пустым.\n");
                Thread.Sleep(1900);
                return;
            }
            else if (TaskAlreadyExist)
            {
                Alert("Задача с таким названием уже существует.");
                Thread.Sleep(1900);
                return;
            }

            tasks.Add(new Task(Title, false));
            SaveTasks();
            Console.Clear();

            string[] hints =
            {
                "используй метод Помидора: 25 минут работы, 5 минут отдыха.",
                "разбивай большие задачи на маленькие подпункты.",
                "проверяй список задач каждое утро перед началом работы.",
                "регулярно удаляй задачи, которые потеряли актуальность.",
                "не держи задачи в голове — записывай всё в TODO app."
            };

            string randomHint = hints[rnd.Next(hints.Length)];

            Console.ForegroundColor = ConsoleColor.Green;
            CenterText("┌───────────────────────────────────────────────┐");
            CenterText("│           Задача успешно добавлена!           │");
            CenterText("└───────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.WriteLine();
            
            Console.Write($"> Совет: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(randomHint);
            Console.ResetColor();

            CenterText("\nНажмите любую клавишу, чтобы вернуться в меню...");

            Console.ReadKey(true);
            //Thread.Sleep(2000);
        }


        private void DeleteTask()
        {
            if (tasks.Count > 0)
            {
                Console.Clear();
                StageTitle("Удалить задачу");

                int i = 0;
                CenterText("Какую задачу вы хотите удалить?\n");
                foreach (var task in tasks)
                {
                    i++;
                    CenterText($"{i} - {task.Title}");
                }
                CenterText("q - назад в меню");
                Console.Write("\n--> ");
                string input = Console.ReadLine();

                if (input.ToLower() == "q")
                {
                    Console.Clear();
                    return;
                }

                if (int.TryParse(input, out int TaskNum))
                {
                    int index = TaskNum - 1;

                    if (index >= 0 && index < tasks.Count)
                    {
                        Success($"Задача \"{tasks[index].Title}\" удалена успешно");
                        Thread.Sleep(1500);
                        tasks.Remove(tasks[index]);
                        SaveTasks();
                    }
                    else
                    {
                        Console.Clear();
                        Alert("Задачи с таким номером не существует.");
                        CenterText("> Попробуйте ещё раз ...");
                        Thread.Sleep(2000);
                    }
                }
            }
            else
            {
                NoTasks();
            }
        }


        private void ChangeStatusTask()
        {
            Console.Clear();
            StageTitle("Изменить статус задачи");

            int i = 0;
            if (tasks.Count > 0)
            {
                CenterText("Для какой задачи изменить статус?\n");
                foreach (var task in tasks)
                {
                    i++;
                    CenterText($"{i} - {task.Title}");
                }
                CenterText("q - назад в меню");
                Console.Write("\n--> ");
                string input = Console.ReadLine();


                if (input.ToLower() == "q")
                {
                    Console.Clear();
                    return;
                }


                if (int.TryParse(input, out int TaskNum))
                {
                    int index = TaskNum - 1;

                    if (index >= 0 && index < tasks.Count)
                    {
                        if (tasks[index].IsDone == false)
                        {
                            tasks[index].IsDone = true;
                            SaveTasks();
                            Success($"Задача \"{tasks[index].Title}\" теперь выполнена.");
                        }
                        else
                        {
                            tasks[index].IsDone = false;
                            SaveTasks();
                            Success($"Задача \"{tasks[index].Title}\" теперь не выполнена.");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Alert("Задачи с таким номером не существует.");
                    }
                }
                else
                {
                    Console.Clear();
                    Alert("Введите номер задачи или 'q' для выхода.");
                }

            }
            else
            {
                NoTasks();
            }
            Thread.Sleep(1900);
            Console.Clear();
        }


        private void ShowAllTasks()
        {
            Console.Clear();
            StageTitle("Все задачи");

            if (tasks.Count > 0)
            {
                int i = 0;
                bool hasDone = tasks.Any(t => t.IsDone);
                bool hasNotDone = tasks.Any(t => !t.IsDone);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                CenterText("Выполненые задачи:");
                Console.ResetColor();

                if (!hasDone) CenterText("Здесь пока ничего нет...");

                foreach (var task in tasks)
                {
                    if (task.IsDone)
                    {
                        i++;
                        CenterText($"{i}. {task.Title}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(new string('─', Console.WindowWidth - 1));
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                CenterText("Не выполненные задачи:");
                Console.ResetColor();

                if (!hasNotDone) CenterText("Здесь пока ничего нет...");

                i = 0;
                foreach (var task in tasks)
                {
                    if (!task.IsDone)
                    {
                        i++;
                        CenterText($"{i}. {task.Title}");
                    }
                }

                Console.WriteLine();
                Console.Write("\nНажмите любую клавишу, чтобы выйти в меню . . .");
                Console.ReadKey(true);
            }
            else
            {
                NoTasks();
            }
            Console.Clear();
        }

        public static void NoTasks()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            CenterText("┌───────────────────────────────────────────────┐");
            CenterText("│          Похоже, у вас ещё нет задач          │");
            CenterText("└───────────────────────────────────────────────┘");
            Console.ResetColor();

            CenterText("\nЧтобы начать работу:");
            Console.WriteLine("> Нажмите любую клавишу, чтобы выйти в меню;");
            Console.Write("> Выберите пункт ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\"Добавить задачу\"");
            Console.ResetColor();
            Console.Write(";");

            Console.ReadKey(true);
        }


        private void StageTitle(string title)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            CenterText($"======>   {title.ToUpper()}   <======");
            Console.ResetColor();

            Console.WriteLine();
        }


        private void Alert(string message)
        {
            Console.BackgroundColor= ConsoleColor.DarkRed;
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine($"[!] {message}");
            Console.ResetColor();
        }


        private void Success(string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[!] {message}");
            Console.ResetColor();
        }


        private void SaveTasks()
        {
            string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(todoFile, json, Encoding.Unicode);
        }

        private void LoadTasks()
        {
            if (!File.Exists(todoFile))
            {
                tasks = new List<Task>();
                return;
            }

            try
            {
                string json = File.ReadAllText(todoFile, Encoding.Unicode);
                var deserializedTasks = JsonConvert.DeserializeObject<List<Task>>(json);
                
                tasks = deserializedTasks ?? new List<Task>();
            }
            catch (Exception)
            {
                tasks = new List<Task>();
            }
        }


        static void Main(string[] args)
        { 
            Program program = new Program();
            program.Setup();
            program.LoadTasks();
            program.Menu();
        }
    }
}
