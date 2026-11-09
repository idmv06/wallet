using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SicaiWallet.Web.Data;

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
}