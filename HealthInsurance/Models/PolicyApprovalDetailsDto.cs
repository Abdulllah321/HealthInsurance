using System.ComponentModel.DataAnnotations;

public class PolicyApprovalDetailsDto
{
    [Key]
    public int Id { get; set; }

    public int PolicyId { get; set; }
    public int RequestId { get; set; }  // No need for [ForeignKey] here

    public DateTime Date { get; set; }

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a valid number.")]
    public decimal Amount { get; set; }

    public bool Approved { get; set; }

    [StringLength(50)]
    public string Reason { get; set; }
}
