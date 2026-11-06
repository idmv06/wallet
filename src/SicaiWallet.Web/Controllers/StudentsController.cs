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

}