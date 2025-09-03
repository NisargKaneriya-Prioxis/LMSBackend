using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LM.Model.ResponseModel;

public class LMSAdminResponseModel
{
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