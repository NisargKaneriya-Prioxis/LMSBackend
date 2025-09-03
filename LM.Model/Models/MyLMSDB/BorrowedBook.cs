using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

[Table("BorrowedBook")]
[Index("BorrowedSid", Name = "UQ__Borrowed__B3ACE37F15948D1C", IsUnique = true)]
public partial class BorrowedBook
{
    [Key]
    [Column("BorrowedID")]
    public int BorrowedId { get; set; }

    [Column("BorrowedSID")]
    [StringLength(50)]
    public string BorrowedSid { get; set; } = null!;

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("BookID")]
    public int BookId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? IssueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnDate { get; set; }

    public int BorrowedBookStatus { get; set; }

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

    [ForeignKey("BookId")]
    [InverseProperty("BorrowedBooks")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("BorrowedBooks")]
    public virtual User User { get; set; } = null!;
}
