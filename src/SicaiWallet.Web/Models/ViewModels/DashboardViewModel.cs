using SicaiWallet.Web.Models;

namespace SicaiWallet.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }

    public int ActiveWallets { get; set; }

    public decimal TotalBalance { get; set; }

    public int TotalTransactions { get; set; }

    public List<WalletTransaction> RecentTransactions { get; set; }
        = new();
}