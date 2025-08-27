using Microsoft.AspNetCore.Identity;

namespace OnlineExam.Models;

public class Users : IdentityUser
{
    public string FullName { get; set; }
    public virtual List<ExamResult> ExamResults { get; set; } = [];
    public virtual List<UserAvailableExam> AvailableExams { get; set; } = [];

}
