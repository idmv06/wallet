using System.ComponentModel.DataAnnotations;

namespace SicaiWallet.Web.Models;

public class WalletAccount
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public decimal Balance { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}