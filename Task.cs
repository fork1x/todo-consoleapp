namespace ToDoApp
{
    internal class Task
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }
        
        public Task(string title, bool isDone)
        {
            this.Title = title;
            this.IsDone = isDone;
        }

        public override string ToString()
        {
            return $"{(this.IsDone ? "[V]" : "[X]")} | {this.Title}";
        }
    }
}
