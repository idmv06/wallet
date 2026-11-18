using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SicaiWallet.Web.Data;
using SicaiWallet.Web.Models.ViewModels;

namespace SicaiWallet.Web.Controllers;

public class DashboardController : Controller
{
    private readonly WalletDbContext _context;

    public DashboardController(WalletDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            TotalStudents = _context.Students.Count(),

            ActiveWallets = _context.WalletAccounts
                .Count(wallet => wallet.IsActive),

            TotalBalance = _context.WalletAccounts
                .Sum(wallet => (decimal?)wallet.Balance) ?? 0m,

            TotalTransactions = _context.WalletTransactions.Count(),

            RecentTransactions = _context.WalletTransactions
                .Include(transaction => transaction.WalletAccount)
                .ThenInclude(wallet => wallet.Student)
                .OrderByDescending(transaction => transaction.CreatedAt)
                .Take(5)
                .ToList()
        };

        return View(model);
    }
}