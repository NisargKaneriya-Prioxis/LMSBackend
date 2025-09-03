using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

[Table("Admin")]
[Index("AdminSid", Name = "UQ__Admin__375BD13623CBB650", IsUnique = true)]
public partial class Admin
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
