using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Data;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers.Admin;

public class AdminUserController(AppDbContext context) : Controller
{
    // GET: Admin/Users
    public async Task<IActionResult> GetAll()
    {
        var users = await context.Users
            .Include(u => u.ExamResults)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return View(users);
    }

    // GET: Admin/Edit/5
    public async Task<IActionResult> Edit(string id)
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
            AvailableExams = [.. allExams.Select(e => new ExamCheckboxVM
            {
                ExamId = e.Id,
                Title = e.Title,
                IsSelected = user.AvailableExams.Any(ae => ae.ExamId == e.Id)
            })],

            // Solved Exams (اللي اتحلت فعلاً)
            SolvedExams = [.. user.ExamResults.Select(er => new SolvedExamVM
            {
                ExamId = er.ExamId,
                Title = er.Exam.Title,
                Score = er.Score,
                CompletedAt = er.CompletedAt,
                IsPassed = er.IsPassed
            })]
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
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
        return RedirectToAction(nameof(GetAll));
    }

    // GET: Admin/Create
    public async Task<IActionResult> Create()
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
    // POST: Admin/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
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
            return RedirectToAction(nameof(GetAll));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        model.AvailableExams = await context.Exams
            .Select(e => new ExamCheckboxVM { ExamId = e.Id, Title = e.Title })
            .ToListAsync();

        return View(model);
    }
}
