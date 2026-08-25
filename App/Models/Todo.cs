public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDone { get; set; }
    public required UserCreatedBy CreatedBy { get; set; }
}
