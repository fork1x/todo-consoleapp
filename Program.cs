using System.Text;

namespace ToDoApp
{
    class Program
    {
        List<Task> tasks = new List<Task>();

        static void Setup()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            
            Console.Title = "Simple TODO app by fork1x";
            Console.CursorVisible = false;

            Console.Clear();
        }

        internal void Menu()
        {
            while (true)
            {
                Console.WriteLine("1 - добавить задачу");
                Console.WriteLine("2 - изменить статус задачи");
                Console.WriteLine("3 - показать все задачи");
                Console.WriteLine("4 - выйти");

                var Key = Console.ReadKey(true).Key;

                switch (Key)
                {
                    case ConsoleKey.D1:
                        AddTask();
                        break;

                    case ConsoleKey.D2:
                        ChangeStatusTask();
                        break;

                    case ConsoleKey.D3:
                        ShowAllTasks();
                        break;

                    case ConsoleKey.D4 or ConsoleKey.Escape:
                        return;

                    default:
                        Console.Clear();
                        Console.WriteLine("[!] Такого варианта нет.\n");
                        break;

                }
            }
        }

        void AddTask()
        {
            Console.Clear();

            Console.Write("Введите название задачи: ");
            string Title = Console.ReadLine().Trim();
            
            bool TaskAlreadyExist = false;
            foreach (var task in tasks)
            {
                if (Title == task.Title)
                {
                    TaskAlreadyExist = true;
                }
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                Console.Clear();
                Console.WriteLine("[!] Название задачи не может быть пустым.\n");
                return;
            }
            else if (TaskAlreadyExist)
            {
                Console.WriteLine("\n[!] Задача с таким названием уже существует.");
                return;
            }

            tasks.Add(new Task(Title, false));
            Console.Clear();
            Console.WriteLine($"[!] Задача \"{Title}\" добавлена.\n");
        }

        void ChangeStatusTask()
        {
            Console.Clear();

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
                            Console.WriteLine($"[!] Задача {tasks[index].Title} выполнена.");
                        }
                        else
                        {
                            tasks[index].IsDone = false;
                            Console.WriteLine($"[!] Задача {tasks[index].Title} теперь не выполнена.");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("[!] Задачи с таким номером не существует.");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("[!] Ошибка: введите номер задачи или 'q' для выхода.");
                }

            }
            else
            {
                Console.Clear();
                Console.WriteLine("Задач нет.\n");
            }
            Console.ReadLine();
            Console.Clear();
        }

        void ShowAllTasks()
        {
            Console.Clear();

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
                Console.WriteLine("[!] Задачи отсутствуют.\n");
            }
            Console.ReadKey(true);
            Console.Clear();
        }

        static void Main(string[] args)
        {
            Setup();
            
            Program program = new Program();
            program.Menu();
        }
    }
}
