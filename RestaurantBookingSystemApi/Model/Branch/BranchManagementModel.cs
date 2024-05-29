using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBookingSystemApi.Model.Branch;

[Table("Branch")]
public class BranchManagementModel
{
    [Key]
    public long BranchId { get; set; }   
    public string BranchCode { get; set; }
    public string BranchName { get; set; }
    public string Location { get; set; }
    public bool IsActive {  get; set; }
}