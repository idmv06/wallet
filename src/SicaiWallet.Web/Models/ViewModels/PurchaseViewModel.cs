using System.ComponentModel.DataAnnotations;

namespace SicaiWallet.Web.Models.ViewModels;

public class PurchaseViewModel
{
    public int WalletAccountId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "The amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}   