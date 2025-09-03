using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

[Index("UserSid", Name = "UQ__Users__2B6A7E55A94DCFD7", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("UserSID")]
    [StringLength(50)]
    public string UserSid { get; set; } = null!;

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [MaxLength(64)]
    public byte[] PasswordHash { get; set; } = null!;

    [Column("phone_number")]
    public long? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    public int Status { get; set; }

    [Column("CreatedAT", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("ModifiedAT", TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [Column("CreatedBY")]
    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [Column("ModifiedBY")]
    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}
