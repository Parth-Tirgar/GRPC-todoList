namespace ToDoList.Models
{
    namespace ToDoGrpcService.Models
    {
        public class TodoItem
        {
            public int ID { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string ToDoStatus { get; set; } = "NEW";
        }
    }

}
