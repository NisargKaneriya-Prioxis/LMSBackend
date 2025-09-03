using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.RequestModel;

public class LMSBorrowedBookRequestModel
{
    [Key]
    [Column("BorrowedID")]
    public int BorrowedId { get; set; }

    [Column("UserSID")]
    [StringLength(50)]
    public string UserSid { get; set; } = null!;

    [Column("BookSID")]
    [StringLength(50)]
    public string BookSid { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? IssueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnDate { get; set; }

    public int BorrowedStatus { get; set; }

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

    [ForeignKey("BookSid")]
    [InverseProperty("BorrowedBooks")]
    public virtual Book BookS { get; set; } = null!;

    [ForeignKey("UserSid")]
    [InverseProperty("BorrowedBooks")]
    public virtual User UserS { get; set; } = null!;
}