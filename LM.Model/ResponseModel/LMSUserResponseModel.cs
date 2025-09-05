using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;
using Newtonsoft.Json;

namespace LM.Model.ResponseModel;

public class LMSUserResponseModel
{
    [Column("UserSID")]
    [StringLength(50)]
    public string UserSid { get; set; } = null!;

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }
    
    [Column("phone_number")]
    [JsonProperty(PropertyName = "phone_number")]
    public long? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

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
    
    [StringLength(20)]
    public string Role { get; set; } = null!;


    [InverseProperty("UserS")]
    public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}