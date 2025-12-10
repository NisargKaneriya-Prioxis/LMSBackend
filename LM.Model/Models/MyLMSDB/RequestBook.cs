using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

[Table("RequestBook")]
[Index("RequestBookSid", Name = "UQ__RequestB__96B57DEC6C48B122", IsUnique = true)]
public partial class RequestBook
{
    [Key]
    [Column("RequestBookID")]
    public int RequestBookId { get; set; }

    [Column("RequestBookSID")]
    [StringLength(50)]
    public string RequestBookSid { get; set; } = null!;

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("BookID")]
    public int BookId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RequestDate { get; set; }

    public int RequestBookStatus { get; set; }

    [Column("CreatedAT", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("ModifiedAT", TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    [Column("CreatedBY")]
    public int CreatedBy { get; set; }

    [Column("ModifiedBY")]
    public int? ModifiedBy { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("RequestBooks")]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("RequestBookCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("RequestBookModifiedByNavigations")]
    public virtual User? ModifiedByNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RequestBookUsers")]
    public virtual User User { get; set; } = null!;
}
