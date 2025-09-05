using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.ResponseModel;

public class LMSBorrowedBookResponseModel
{
    [Column("BookSID")]
    [StringLength(50)]
    public string BorrowedSID { get; set; } = null!; 
    
    [StringLength(50)]
    public string? Name { get; set; }
    
    [StringLength(50)]
    public string? Title { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? IssueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnDate { get; set; }

    public int BorrowedBookStatus { get; set; }
}