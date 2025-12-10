using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.ResponseModel;

public class LMSCategoryResponseModel
{
    
    [Column("CategorySID")]
    [StringLength(50)]
    public string CategorySid { get; set; } = null!;

    [StringLength(50)]
    public string? CategoryName { get; set; }
}