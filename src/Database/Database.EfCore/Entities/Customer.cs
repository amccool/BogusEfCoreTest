using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.EfCore.Entities;

[Table("Customers")]
public class Customer
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(21)]
    public string? Phone { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(50)]
    public string? State { get; set; }
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    public DateTime ModifiedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
} 