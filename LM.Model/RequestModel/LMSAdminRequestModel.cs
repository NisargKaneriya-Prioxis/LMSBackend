using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LM.Model.RequestModel;

public class LMSAdminRequestModel
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [Column("AdminSID")]
    [StringLength(50)]
    public string AdminSid { get; set; } = null!;

    [StringLength(50)]
    public string? AdminName { get; set; }

    [StringLength(50)]
    public string? AdminEmail { get; set; }

    [MaxLength(64)]
    public byte[] AdminPassword { get; set; } = null!;
}