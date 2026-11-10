using System.ComponentModel.DataAnnotations;

namespace SicaiWallet.Web.Models.ViewModels;

public class RechargeViewModel
{
    public int WalletAccountId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}