using System.ComponentModel.DataAnnotations;

namespace SicaiWallet.Web.Models;

public class WalletTransaction
{
    public int Id { get; set; }

    [Required]
    public int WalletAccountId { get; set; }

    public WalletAccount WalletAccount { get; set; } = null!;

    public WalletTransactionType Type { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}