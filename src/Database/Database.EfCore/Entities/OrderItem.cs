using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.EfCore.Entities;

[Table("OrderItems")]
public class OrderItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;
    
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
} 