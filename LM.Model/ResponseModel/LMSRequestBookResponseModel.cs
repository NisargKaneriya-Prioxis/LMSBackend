using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.ResponseModel;

public class LMSRequestBookResponseModel
{
    [Column("RequestBookSID")]
    [StringLength(50)]
    public string RequestBookSid { get; set; } = null!;

    [StringLength(50)]
    public string? Name { get; set; }
    
    [StringLength(50)]
    public string? Title { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime? RequestDate { get; set; }
    
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
    
    public int RequestBookStatus { get; set; }
    
}