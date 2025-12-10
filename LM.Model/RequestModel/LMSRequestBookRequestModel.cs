using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.RequestModel;

public class LMSRequestBookRequestModel
{
    
    [Column("RequestBookSID")]
    [StringLength(50)]
    public string RequestBookSid { get; set; } = null!;
    
}