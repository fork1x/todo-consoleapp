using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace ToDoApp
{
    class Program
    {
        List<Task> tasks = new List<Task>();
        private readonly string todoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        private static void Setup()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            
            Console.Title = "TODO app";
            Console.CursorVisible = false;

            Console.Clear();
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

            while (true)
            {
                Console.Clear();

                StageTitle("Главное меню");
                Console.WriteLine("Используйте ↑ и ↓ для перемещения по меню.");
                Console.WriteLine("Чтобы перейти дальше, нажмите Enter.\n");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;

                        Console.WriteLine($"   > {options[i]} <   ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"   {options[i]}   ");
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
                    switch (selectedIndex)
                    {
                        case 0: AddTask(); break;
                        case 1: DeleteTask(); break;
                        case 2: ChangeStatusTask(); break;
                        case 3: ShowAllTasks(); break;
                        case 4: return;
                    }
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
                return;
            }

            tasks.Add(new Task(Title, false));
            SaveTasks();
            Console.Clear();
            Success($"Задача \"{Title}\" добавлена.");
            Thread.Sleep(2000);
        }


        private void DeleteTask()
        {
            if (tasks.Count > 0)
            {
                Console.Clear();
                StageTitle("Удалить задачу");

                int i = 0;
                if (tasks.Count > 0)
                {
                    Console.WriteLine("Какую задачу вы хотите удалить?\n");
                    foreach (var task in tasks)
                    {
                        i++;
                        Console.WriteLine($"{i} - {task.Title}");
                    }
                    Console.WriteLine("q - назад в меню");
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
                            Console.WriteLine("> Попробуйте ещё раз ...");
                            Thread.Sleep(2000);
                        }
                    }
                }
                else
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("┌───────────────────────────────────────────────┐");
                    Console.WriteLine("│          Похоже, у вас ещё нет задач          │");
                    Console.WriteLine("└───────────────────────────────────────────────┘");
                    Console.ResetColor();

                    Console.WriteLine("\nЧтобы начать работу:");
                    Console.Write("> Перейдите в меню и выберите ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\"Добавить задачу\"");
                    Console.ResetColor();
                }
            }
        }


        private void ChangeStatusTask()
        {
            Console.Clear();
            StageTitle("Изменить статус задачи");

            int i = 0;
            if (tasks.Count > 0)
            {
                Console.WriteLine("Для какой задачи изменить статус?\n");
                foreach (var task in tasks)
                {
                    i++;
                    Console.WriteLine($"{i} - {task.Title}");
                }
                Console.WriteLine("q - назад в меню");
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
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("┌───────────────────────────────────────────────┐");
                Console.WriteLine("│          Похоже, у вас ещё нет задач          │");
                Console.WriteLine("└───────────────────────────────────────────────┘");
                Console.ResetColor();

                Console.WriteLine("\nЧтобы начать работу:");
                Console.Write("> Перейдите в меню и выберите ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\"Добавить задачу\"");
                Console.ResetColor();
            }
            Thread.Sleep(2000);
            //Console.ReadLine();
            Console.Clear();
        }


        private void ShowAllTasks()
        {
            Console.Clear();
            StageTitle("Все задачи");

            int i = 0;
            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    i++;
                    Console.WriteLine($"{i}. {task}");
                }
                Console.WriteLine("\nНажмите любую клавишу, чтобы выйти в меню . . .");
            }
            else
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("┌───────────────────────────────────────────────┐");
                Console.WriteLine("│          Похоже, у вас ещё нет задач          │");
                Console.WriteLine("└───────────────────────────────────────────────┘");
                Console.ResetColor();

                Console.WriteLine("\nЧтобы начать работу:");
                Console.Write("> Перейдите в меню и выберите ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\"Добавить задачу\"");
                Console.ResetColor();
            }
            Console.ReadKey(true);
            Console.Clear();
        }


        private void StageTitle(string title)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"======>   {title.ToUpper()}   <======\n");
            Console.ResetColor();
            
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
            Console.WriteLine($"[!] {message}");
            Console.ResetColor();
        }


        private void SaveTasks()
        {
            string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(todoFile, json, Encoding.Unicode);
        }

        private void LoadTasks()
        {
            Console.Write($"В поисках файла: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(todoFile);
            Console.ResetColor();
            
            if (!File.Exists(todoFile))
            {
                Console.WriteLine("\n");
                Alert($"Файл не был обнаружен");
                Console.WriteLine("> Файл будет создан автоматически после создания первой задачи.");
                Thread.Sleep(3000);
                return;
            }

            string json = File.ReadAllText(todoFile, Encoding.Unicode);
            tasks = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
            Console.WriteLine();
            Console.WriteLine($"Загружено задач: {tasks.Count}");
            Thread.Sleep(600);
        }


        static void Main(string[] args)
        {
            Setup();
            
            Program program = new Program();
            program.LoadTasks();
            program.Menu();
        }
    }
}
