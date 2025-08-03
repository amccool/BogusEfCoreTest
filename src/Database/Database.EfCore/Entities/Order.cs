using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.EfCore.Entities;

[Table("Orders")]
public class Order
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [MaxLength(500)]
    public string? ShippingAddress { get; set; }
    
    [MaxLength(500)]
    public string? BillingAddress { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 