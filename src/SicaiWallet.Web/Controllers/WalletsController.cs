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

    // Display all wallets
    public IActionResult Index()
    {
        var wallets = _context.WalletAccounts
            .Include(wallet => wallet.Student)
            .ToList();

        return View(wallets);
    }

    // RECHARGE - GET
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

        ViewBag.StudentName =
            $"{wallet.Student.FirstName} {wallet.Student.LastName}";

        ViewBag.StudentId = wallet.Student.SicaiStudentId;

        return View(model);
    }

    // RECHARGE - POST
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

    // PURCHASE - GET
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

    // PURCHASE - POST
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

    // HISTORY - GET 
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
         .Include(transaction => transaction.Refunds)
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

    // REFUND - GET
    [HttpGet]
    public IActionResult Refund(int id)
    {
        var purchase = _context.WalletTransactions
            .Include(transaction => transaction.WalletAccount)
            .ThenInclude(wallet => wallet.Student)
            .FirstOrDefault(transaction => transaction.Id == id);

        if (purchase == null ||
            purchase.Type != WalletTransactionType.Purchase)
        {
            return NotFound();
        }

        var refundedAmount = _context.WalletTransactions
            .Where(transaction =>
                transaction.OriginalTransactionId == id &&
                transaction.Type == WalletTransactionType.Refund)
            .Sum(transaction => (decimal?)transaction.Amount) ?? 0m;

        var remainingAmount = purchase.Amount - refundedAmount;

        if (remainingAmount <= 0)
        {
            return BadRequest(
                "This purchase has already been fully refunded.");
        }

        var model = new RefundViewModel
        {
            OriginalTransactionId = purchase.Id,
            Amount = remainingAmount
        };

        ViewBag.StudentName =
            $"{purchase.WalletAccount.Student.FirstName} {purchase.WalletAccount.Student.LastName}";

        ViewBag.StudentId = purchase.WalletAccount.Student.SicaiStudentId;
        ViewBag.PurchaseAmount = purchase.Amount;
        ViewBag.RefundedAmount = refundedAmount;
        ViewBag.RemainingAmount = remainingAmount;

        return View(model);
    }

    // REFUND - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Refund(RefundViewModel model)
    {
        using var dbTransaction = _context.Database.BeginTransaction(
            System.Data.IsolationLevel.Serializable);

        var purchase = _context.WalletTransactions
            .Include(transaction => transaction.WalletAccount)
            .ThenInclude(wallet => wallet.Student)
            .FirstOrDefault(transaction =>
                transaction.Id == model.OriginalTransactionId);

        if (purchase == null ||
            purchase.Type != WalletTransactionType.Purchase)
        {
            return NotFound();
        }

        var refundedAmount = _context.WalletTransactions
            .Where(transaction =>
                transaction.OriginalTransactionId == purchase.Id &&
                transaction.Type == WalletTransactionType.Refund)
            .Sum(transaction => (decimal?)transaction.Amount) ?? 0m;

        var remainingAmount = purchase.Amount - refundedAmount;

        ViewBag.StudentName =
            $"{purchase.WalletAccount.Student.FirstName} {purchase.WalletAccount.Student.LastName}";

        ViewBag.StudentId = purchase.WalletAccount.Student.SicaiStudentId;
        ViewBag.PurchaseAmount = purchase.Amount;
        ViewBag.RefundedAmount = refundedAmount;
        ViewBag.RemainingAmount = remainingAmount;

        if (ModelState.IsValid && model.Amount > remainingAmount)
        {
            ModelState.AddModelError(
                "Amount",
                "The refund amount exceeds the remaining refundable amount.");
        }

        if (remainingAmount <= 0)
        {
            ModelState.AddModelError(
                "",
                "This purchase has already been fully refunded.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Return money to the original wallet
        purchase.WalletAccount.Balance += model.Amount;

        // Create refund transaction
        var refund = new WalletTransaction
        {
            WalletAccountId = purchase.WalletAccountId,
            OriginalTransactionId = purchase.Id,
            Type = WalletTransactionType.Refund,
            Amount = model.Amount,
            Description = model.Description
        };

        _context.WalletTransactions.Add(refund);

        // Save and commit both changes
        _context.SaveChanges();

        dbTransaction.Commit();

        return RedirectToAction(
            nameof(History),
            new { id = purchase.WalletAccountId });
    }
}