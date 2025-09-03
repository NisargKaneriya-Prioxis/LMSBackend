using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;
using Newtonsoft.Json;

namespace LM.Model.ResponseModel;

public class LMSBookResponseModel
{
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
    [JsonProperty(PropertyName = "Available_Quantity")]
    public int? AvailableQuantity { get; set; }

    public int? PublishYear { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }

    public int BorrowedStatus { get; set; }

    public int Status { get; set; }

    [Column("CreatedAT", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    // [Column("ModifiedAT", TypeName = "datetime")]
    // public DateTime? ModifiedAt { get; set; }
    //
    // [Column("CreatedBY")]
    // [StringLength(50)]
    // public string? CreatedBy { get; set; }
    //
    // [Column("ModifiedBY")]
    // [StringLength(50)]
    // public string? ModifiedBy { get; set; }

    [StringLength(50)]
    public string? CategoryName { get; set; }

    // [InverseProperty("BookS")]
    // public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}