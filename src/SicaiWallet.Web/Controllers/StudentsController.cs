using Microsoft.AspNetCore.Mvc;
using SicaiWallet.Web.Data;
using System.Linq;
using SicaiWallet.Web.Models;


namespace SicaiWallet.Web.Controllers;

public class StudentsController : Controller
{

    private readonly WalletDbContext _context;

    public StudentsController(WalletDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
{
    var students = _context.Students.ToList();

    return View(students);
}


[HttpGet]
public IActionResult Create()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Student student)
{
    if (!ModelState.IsValid)
    {
        return View(student);
    }

    _context.Students.Add(student);
    _context.SaveChanges();

    return RedirectToAction(nameof(Index));
}



[HttpGet]
public IActionResult Edit(int id)
{
    var student = _context.Students.Find(id);

    if (student == null)
    {
        return NotFound();
    }

    return View(student);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Student student)
{
    if (id != student.Id)
    {
        return BadRequest();
    }

    if (!ModelState.IsValid)
    {
        return View(student);
    }

    var existingStudent = _context.Students.Find(id);

    if (existingStudent == null)
    {
        return NotFound();
    }

    existingStudent.SicaiStudentId = student.SicaiStudentId;
    existingStudent.FirstName = student.FirstName;
    existingStudent.LastName = student.LastName;

    _context.SaveChanges();

    return RedirectToAction(nameof(Index));
}

[HttpGet]
public IActionResult Delete(int id)
{
    var student = _context.Students.Find(id);

    if (student == null)
    {
        return NotFound();
    }

    return View(student);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public IActionResult DeleteConfirmed(int id)
{
    var student = _context.Students.Find(id);

    if (student == null)
    {
        return NotFound();
    }

    _context.Students.Remove(student);
    _context.SaveChanges();

    return RedirectToAction(nameof(Index));
}

}