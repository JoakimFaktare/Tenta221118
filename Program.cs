using static dtp15_todolist.Todo;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace dtp15_todolist
{
    //Joakim Fäktare
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task, string taskDescription)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile()
        {
            string todoFileName = "todo.lis"; 
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
            int numRead = 0;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
        }
        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(bool verbose = false)
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
            {
                item.Print(verbose);
            }
            PrintFoot(verbose);
        }
        public static void AddNewTodoItem()
        {
            string task = MyIO.ReadCommand("Skriv in uppgift: ");
            int prio = int.Parse(MyIO.ReadCommand("Skriv in prioritet: "));
            string taskDescription = MyIO.ReadCommand("Skriv beskrivning för uppgiften: "); ;
            Todo.TodoItem item = new Todo.TodoItem(prio, task, taskDescription);
            Todo.list.Add(item);
        }
        public static void ChangeStatus(string command) 
        {
            bool check = false;
            string[] cwords = command.Split(' ');
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].task == $"{cwords[1]} {cwords[2]}" && list[i].status != Active && cwords[0] == "aktivera")
                {
                    list[i].status = 1;
                    Console.WriteLine($"{list[i].task} status har uppdaterats till 'aktiv'.");
                    check = true;
                }
                else if (list[i].task == $"{cwords[1]} {cwords[2]}" && list[i].status != Waiting && cwords[0] == "vänta")
                {
                    list[i].status = 2;
                    Console.WriteLine($"{list[i].task} status har uppdaterats till 'väntande'.");
                    check=true;
                }
                else if (list[i].task == $"{cwords[1]} {cwords[2]}" && list[i].status != Ready && cwords[0] == "klar")
                {
                    list[i].status = 3;
                    Console.WriteLine($"{list[i].task} status har uppdaterats till 'klar'.");
                    check = true;
                }
            }
            if (check == false)
            {
                Console.WriteLine("Kommando felaktigt");
            }
        }
        public static void PrintActiveTodoList( bool verbose = false) 
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
                if (item.status == Active)
                    item.Print(verbose);
            PrintFoot(verbose);
        }
        public static void PrintWaitingTodolist(bool verbose = false)
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
                if (item.status == Waiting)
                    item.Print(verbose);
            PrintFoot(verbose);
        }
        public static void PrintReadyTodolist(bool verbose = false)
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
                if (item.status == Ready)
                    item.Print(verbose);
            PrintFoot(verbose);
        }
        public static void SaveList()
        {
            string lastFileName = "todo.lis"; 
            using (StreamWriter sw = new StreamWriter(lastFileName))
            {
                foreach (TodoItem item in list)
                {
                    if (item != null)
                    {
                        sw.WriteLine($"{item.status}|{item.priority}|{item.task}|{item.taskDescription}");
                    }
                }
            }
        }
        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("hjälp________________lista denna hjälp");
            Console.WriteLine("ladda________________ladda att-göra-lista");
            Console.WriteLine("lista________________lista alla Aktiva uppdrag i att-göra-listan");
            Console.WriteLine("lista allt___________lista alla uppdrag i att-göra-listan");
            Console.WriteLine("beskriv______________lista alla Aktiva uppdrag i att-göra-listan med beskrivning");
            Console.WriteLine("beskriv allt---------lista alla uppdrag med beskrivning");
            Console.WriteLine("ny___________________lägg till nytt uppdrag i att-göra-listan");
            Console.WriteLine("klar *uppgift*_______sätter status på uppgift till 'avklarad'");
            Console.WriteLine("vänta *uppgift*______sätter status på uppgift till 'avklarad'");
            Console.WriteLine("aktivera *uppgift*___sätter status på uppgift till 'avklarad'");
            Console.WriteLine("spara________________spara att-göra-listan");
            Console.WriteLine("sluta________________spara att-göra-listan och sluta");
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            string command;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command, "sluta"))
                {
                    SaveList();
                    Console.WriteLine("Hej då!");
                    break;
                }
                else if (MyIO.Equals(command, "lista"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: false);
                    else if (MyIO.HasArgument(command, "väntande"))
                        Todo.PrintWaitingTodolist(verbose: false);
                    else if (MyIO.HasArgument(command, "klara"))
                        Todo.PrintReadyTodolist(verbose: false);
                    else
                        Todo.PrintActiveTodoList(); 
                }
                else if (MyIO.Equals(command, "beskriv"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: true);
                    else
                    Todo.PrintActiveTodoList(verbose: true); 
                }
                else if (MyIO.Equals(command, "ny"))
                {
                    AddNewTodoItem();
                }
                else if (MyIO.Equals(command, "ladda"))
                {
                    ReadListFromFile();
                }
                else if (MyIO.Equals(command, "spara"))
                {
                    SaveList();
                }
                else if (MyIO.Equals(command, "klar"))
                {
                    ChangeStatus(command);
                }
                else if (MyIO.Equals(command, "vänta"))
                {
                    ChangeStatus(command);
                }
                else if (MyIO.Equals(command, "aktivera"))
                {
                    ChangeStatus(command);
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: {command}");
                }
            }
            while (true);
        }

    }
    class MyIO
    {
        static public string ReadCommand(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected) 
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;
                if (cwords[1] == expected) return true;
            }
            return false;
        }
    }
}
