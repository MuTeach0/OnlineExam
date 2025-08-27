namespace OnlineExam.ViewModels;

public class ExamResultViewModel
{
    public int ResultId { get; set; }   // ← أضف ده
    public int ExamId { get; set; }
    public string ExamTitle { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public TimeSpan Duration { get; set; }
}