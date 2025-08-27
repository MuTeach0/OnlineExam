namespace OnlineExam.ViewModels;

public class AvailableExamViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int QuestionCount { get; set; }
    public bool HasTaken { get; set; }
}