using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;

namespace LM.Model.RequestModel;

public class LMSCategoryRequestModel
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("CategorySID")]
    [StringLength(50)]
    public string CategorySid { get; set; } = null!;

    [StringLength(50)]
    public string? CategoryName { get; set; }

    [InverseProperty("CategoryS")]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}