using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Data;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext context) : Controller
{
    #region Private Method
    private bool ExamExists(int id) => context.Exams.Any(e => e.Id == id);
    private bool QuestionExists(int id) => context.Questions.Any(q => q.Id == id);
    #endregion

    // GET: Admin
    public IActionResult Index() => View();
    #region Exams Enpoints
    // GET: Admin/Exams
    public async Task<IActionResult> Exams()
    {
        var exams = await context.Exams
            .Include(e => e.Questions)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return View(exams);
    }

    // GET: Admin/CreateExam
    public IActionResult CreateExam() => View(new ExamViewModel());
    // POST: Admin/CreateExam
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExam(ExamViewModel model)
    {
        if (ModelState.IsValid)
        {
            var exam = new Exam
            {
                Title = model.Title,
                Description = model.Description,
                DurationMinutes = model.DurationMinutes,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            context.Add(exam);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Exams));
        }
        return View(model);
    }

    // GET: Admin/EditExam/5
    public async Task<IActionResult> EditExam(int? id)
    {
        if (id == null)
            return NotFound();

        var exam = await context.Exams.FindAsync(id);
        if (exam == null)
            return NotFound();

        var viewModel = new ExamViewModel
        {
            Id = exam.Id,
            Title = exam.Title,
            Description = exam.Description,
            DurationMinutes = exam.DurationMinutes,
            IsActive = exam.IsActive
        };

        return View(viewModel);
    }
    // POST: Admin/EditExam/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExam(int id, ExamViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var exam = await context.Exams.FindAsync(id);
                if (exam == null)
                    return NotFound();

                exam.Title = model.Title;
                exam.Description = model.Description;
                exam.DurationMinutes = model.DurationMinutes;
                exam.IsActive = model.IsActive;

                context.Update(exam);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamExists(model.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Exams));
        }
        return View(model);
    }

    // POST: Admin/DeleteExam/5
    [HttpPost, ActionName("DeleteExam")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExamConfirmed(int id)
    {
        var exam = await context.Exams.FindAsync(id);
        if (exam != null)
        {
            context.Exams.Remove(exam);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Exams));
    }
    #endregion

    #region Questions EndPoints
    // GET: Admin/Questions
    public async Task<IActionResult> Questions()
    {
        var questions = await context.Questions
            .Include(q => q.Exam)
            .OrderBy(q => q.Exam.Title)
            .ThenBy(q => q.Id)
            .ToListAsync();

        return View(questions);
    }

    // GET: Admin/CreateQuestion
    public async Task<IActionResult> CreateQuestion()
    {
        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(new QuestionViewModel());
    }
    // POST: Admin/CreateQuestion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateQuestion(QuestionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var question = new Question
            {
                Title = model.Title,
                ChoiceA = model.ChoiceA,
                ChoiceB = model.ChoiceB,
                ChoiceC = model.ChoiceC,
                ChoiceD = model.ChoiceD,
                CorrectAnswer = model.CorrectAnswer,
                ExamId = model.ExamId
            };

            context.Add(question);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Questions));
        }

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(model);
    }

    // GET: Admin/EditQuestion/5
    public async Task<IActionResult> EditQuestion(int? id)
    {
        if (id == null)
            return NotFound();

        var question = await context.Questions
            .Include(q => q.Exam)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound();

        var viewModel = new QuestionViewModel
        {
            Id = question.Id,
            Title = question.Title,
            ChoiceA = question.ChoiceA,
            ChoiceB = question.ChoiceB,
            ChoiceC = question.ChoiceC,
            ChoiceD = question.ChoiceD,
            CorrectAnswer = question.CorrectAnswer,
            ExamId = question.ExamId,
            // ExamTitle = question.Exam.Title
        };

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(viewModel);
    }
    // POST: Admin/EditQuestion/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQuestion(int id, QuestionViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var question = await context.Questions.FindAsync(id);
                if (question == null)
                    return NotFound();

                question.Title = model.Title;
                question.ChoiceA = model.ChoiceA;
                question.ChoiceB = model.ChoiceB;
                question.ChoiceC = model.ChoiceC;
                question.ChoiceD = model.ChoiceD;
                question.CorrectAnswer = model.CorrectAnswer;
                question.ExamId = model.ExamId;

                context.Update(question);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(model.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Questions));
        }

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(model);
    }

    // POST: Admin/DeleteQuestion/5
    [HttpPost, ActionName("DeleteQuestion")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteQuestionConfirmed(int id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question != null)
        {
            context.Questions.Remove(question);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Questions));
    }
    #endregion

    #region Users EndPoints
    // GET: Admin/Users
    public async Task<IActionResult> Users()
    {
        var users = await context.Users
            .Include(u => u.ExamResults)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return View(users);
    }

    // GET: Admin/EditUser/5
    public async Task<IActionResult> EditUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await context.Users
            .Include(u => u.AvailableExams) // الامتحانات المخصصة
                .ThenInclude(ae => ae.Exam)
            .Include(u => u.ExamResults)   // الامتحانات اللي اتحلت فعلاً
                .ThenInclude(er => er.Exam)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        var allExams = await context.Exams.ToListAsync();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,

            // Available Exams (CheckBoxes)
            AvailableExams = allExams.Select(e => new ExamCheckboxVM
            {
                ExamId = e.Id,
                Title = e.Title,
                IsSelected = user.AvailableExams.Any(ae => ae.ExamId == e.Id)
            }).ToList(),

            // Solved Exams (اللي اتحلت فعلاً)
            SolvedExams = user.ExamResults.Select(er => new SolvedExamVM
            {
                ExamId = er.ExamId,
                Title = er.Exam.Title,
                Score = er.Score,
                CompletedAt = er.CompletedAt,
                IsPassed = er.IsPassed
            }).ToList()
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await context.Users
            .Include(u => u.AvailableExams)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (user == null)
            return NotFound();

        // Update basic info
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.EmailConfirmed = model.EmailConfirmed;

        // 🟢 Update Available Exams (clear & re-add)
        var currentExams = context.UserAvailableExams.Where(ae => ae.UserId == user.Id);
        context.UserAvailableExams.RemoveRange(currentExams);

        foreach (var examId in model.SelectedAvailableExamIds)
        {
            context.UserAvailableExams.Add(new UserAvailableExam
            {
                UserId = user.Id,
                ExamId = examId
            });
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }

    // GET: Admin/CreateUser
    public async Task<IActionResult> CreateUser()
    {
        var exams = await context.Exams.ToListAsync();

        var model = new RegisterViewModel
        {
            AvailableExams = [.. exams.Select(e => new ExamCheckboxVM
            {
                ExamId = e.Id,
                Title = e.Title
            })]
        };

        return View(model);
    }
    // POST: Admin/CreateUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableExams = await context.Exams
                .Select(e => new ExamCheckboxVM { ExamId = e.Id, Title = e.Title })
                .ToListAsync();
            return View(model);
        }

        var user = new Users
        {
            FullName = model.Name,
            UserName = model.Email,
            NormalizedUserName = model.Email.ToUpper(),
            Email = model.Email,
            NormalizedEmail = model.Email.ToUpper(),
            EmailConfirmed = true
        };
        var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<Users>>();
        var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            await userManager.AddToRoleAsync(user, "User");

            // 🟢 اربط الامتحانات المختارة
            foreach (var examId in model.SelectedExamIds)
            {
                context.UserAvailableExams.Add(new UserAvailableExam
                {
                    UserId = user.Id,
                    ExamId = examId
                });
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        model.AvailableExams = await context.Exams
            .Select(e => new ExamCheckboxVM { ExamId = e.Id, Title = e.Title })
            .ToListAsync();

        return View(model);
    }
    #endregion
}