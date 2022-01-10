namespace RetrofitDemoApi.Api;

public class Todo
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool Completed { get; set; }

    public Todo(){}

    public Todo(string text, bool completed)
    {
        Id = 0;
        Text = text;
        Completed = completed;
    }

    public Todo(int id, string text, bool completed)
    {
        Id = id;
        Text = text;
        Completed = completed;
    }
}