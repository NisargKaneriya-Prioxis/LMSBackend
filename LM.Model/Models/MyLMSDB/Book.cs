using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

[Table("Book")]
[Index("Isbn", Name = "UQ__Book__447D36EA6A4E2EBA", IsUnique = true)]
[Index("BookSid", Name = "UQ__Book__EEC908F1F5DDAACE", IsUnique = true)]
public partial class Book
{
    [Key]
    [Column("BookID")]
    public int BookId { get; set; }

    [Column("BookSID")]
    [StringLength(50)]
    public string BookSid { get; set; } = null!;

    [StringLength(50)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? Author { get; set; }

    [Column("ISBN")]
    [StringLength(50)]
    public string? Isbn { get; set; }

    [StringLength(50)]
    public string? Edition { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(50)]
    public string? BookPages { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    public int? Quantity { get; set; }

    [Column("Available_Quantity")]
    public int? AvailableQuantity { get; set; }

    public int? PublishYear { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }

    public int BorrowedStatus { get; set; }

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

    [InverseProperty("Book")]
    public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Books")]
    public virtual Category Category { get; set; } = null!;
}
