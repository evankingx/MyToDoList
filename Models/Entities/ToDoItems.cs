namespace MyToDoList.Models.Entities
{
    public class ToDoItems
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public bool IsCompleted { get; private set; }

        public ToDoItems(string title, bool isCompleted = false)
        {
            ValidateTitle(title);
            Title = title;
            IsCompleted = isCompleted;
        }

        public void Update(string title, bool isCompleted)
        {
            ValidateTitle(title);
            Title = title;
            IsCompleted = isCompleted;
        }

        //THIS IS FOR MY ERROR HANDLING
        private void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty.");
            if (title.Length > 255)
                throw new ArgumentException("Task title cannot exceed 255 characters.");
        }

        private ToDoItems() { }
    }
}
