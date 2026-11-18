using System.ComponentModel.DataAnnotations;

namespace SicaiWallet.Web.Models.ViewModels;

public class RefundViewModel
{
    public int OriginalTransactionId { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "The refund amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}