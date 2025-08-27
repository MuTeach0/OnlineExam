namespace OnlineExam.Models;

public class ExamResult
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int ExamId { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public decimal Score { get; set; } // Percentage score

    public bool IsPassed { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    // Navigation properties
    public virtual Users User { get; set; }
    public virtual Exam Exam { get; set; }
}