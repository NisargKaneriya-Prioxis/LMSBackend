using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LM.Model.Models.MyLMSDB;
using Newtonsoft.Json;

namespace LM.Model.RequestModel;

public class LMSBookRequestModel
{
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
    
    public int? Quantity { get; set; }

    [Column("Available_Quantity")]
    [JsonProperty(PropertyName = "Available_Quantity")]
    public int? AvailableQuantity { get; set; }

    public int? PublishYear { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }
}