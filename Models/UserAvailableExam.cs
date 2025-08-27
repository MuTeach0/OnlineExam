namespace OnlineExam.Models;

public class UserAvailableExam
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public int ExamId { get; set; }

    public virtual Users User { get; set; }
    public virtual Exam Exam { get; set; }
}
