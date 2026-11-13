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

    [HttpGet]
public IActionResult Purchase(int id)
{
    var wallet = _context.WalletAccounts
        .Include(wallet => wallet.Student)
        .FirstOrDefault(wallet => wallet.Id == id);

        if (wallet == null)
        {
        return NotFound();
        }

        var model = new PurchaseViewModel
        {
            WalletAccountId = wallet.Id
        };

        ViewBag.StudentName =
        $"{wallet.Student.FirstName} {wallet.Student.LastName}";   

        ViewBag.StudentId = wallet.Student.SicaiStudentId;
        ViewBag.Balance = wallet.Balance;

        return View(model);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Purchase(PurchaseViewModel model)
    {
        using var dbTransaction = _context.Database.BeginTransaction(
        System.Data.IsolationLevel.Serializable);

        var wallet = _context.WalletAccounts
        .Include(wallet => wallet.Student)
        .FirstOrDefault(wallet => wallet.Id == model.WalletAccountId);

        if (wallet == null)
        {
            return NotFound();
        }

        ViewBag.StudentName =
            $"{wallet.Student.FirstName} {wallet.Student.LastName}";

        ViewBag.StudentId = wallet.Student.SicaiStudentId;
        ViewBag.Balance = wallet.Balance;

        if (!wallet.IsActive)
            {
                ModelState.AddModelError("", "This wallet is inactive.");
        }

        if (ModelState.IsValid && wallet.Balance < model.Amount)
        {
            ModelState.AddModelError("", "Insufficient wallet balance.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Deduct the purchase amount
        wallet.Balance -= model.Amount;

        // Create purchase transaction
        var transaction = new WalletTransaction
        {
            WalletAccountId = wallet.Id,
            Type = WalletTransactionType.Purchase,
            Amount = model.Amount,
            Description = model.Description
        };

        _context.WalletTransactions.Add(transaction);

        // Save changes and commit the database transaction
        _context.SaveChanges();
        dbTransaction.Commit();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult History(int id)
    {
        var wallet = _context.WalletAccounts
            .Include(wallet => wallet.Student)
            .FirstOrDefault(wallet => wallet.Id == id);

        if (wallet == null)
        {
            return NotFound();
        }

            var transactions = _context.WalletTransactions
            .Where(transaction => transaction.WalletAccountId == id)
            .OrderByDescending(transaction => transaction.CreatedAt)
            .ToList();

        ViewBag.StudentName =
            $"{wallet.Student.FirstName} {wallet.Student.LastName}";

        ViewBag.StudentId = wallet.Student.SicaiStudentId;
        ViewBag.Balance = wallet.Balance;
        ViewBag.WalletId = wallet.Id;

        return View(transactions);
    }
}