using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SicaiWallet.Web.Data;
using SicaiWallet.Web.Models;
using SicaiWallet.Web.Models.ViewModels;

namespace SicaiWallet.Web.Controllers;

public class WalletsController : Controller
{
    private readonly WalletDbContext _context;

    public WalletsController(WalletDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var wallets = _context.WalletAccounts
            .Include(wallet => wallet.Student)
            .ToList();

        return View(wallets);
    }

    [HttpGet]
    public IActionResult Recharge(int id)
    {
        var wallet = _context.WalletAccounts
            .Include(wallet => wallet.Student)
        .FirstOrDefault(wallet => wallet.Id == id);

      if (wallet == null)
        {
            return NotFound();
     }

      var model = new RechargeViewModel
     {
         WalletAccountId = wallet.Id
     };

     ViewBag.StudentName = $"{wallet.Student.FirstName} {wallet.Student.LastName}";
     ViewBag.StudentId = wallet.Student.SicaiStudentId;

     return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Recharge(RechargeViewModel model)
    {
     var wallet = _context.WalletAccounts
         .Include(wallet => wallet.Student)
         .FirstOrDefault(wallet => wallet.Id == model.WalletAccountId);

     if (wallet == null)
     {
         return NotFound();
     }

     if (!wallet.IsActive)
     {
         ModelState.AddModelError("", "This wallet is inactive.");
     }

     if (!ModelState.IsValid)
     {
         ViewBag.StudentName =
             $"{wallet.Student.FirstName} {wallet.Student.LastName}";

         ViewBag.StudentId = wallet.Student.SicaiStudentId;

         return View(model);
     }

        // Update wallet balance
        wallet.Balance += model.Amount;

        // Create transaction record
        var transaction = new WalletTransaction
        {
        WalletAccountId = wallet.Id,
        Type = WalletTransactionType.Recharge,
        Amount = model.Amount,
        Description = model.Description
     };

     _context.WalletTransactions.Add(transaction);

     // Save both changes
     _context.SaveChanges();

     return RedirectToAction(nameof(Index));
    }
}