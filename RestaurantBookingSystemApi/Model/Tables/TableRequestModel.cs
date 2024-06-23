using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystemApi.Model.Tables;

public class TableRequestModel
{
    [Key]
    public long TableId { get; set; }
    public String TableNumber { get; set; }
    public string Capacity { get; set; }
    public string Location { get; set; }
    public string BranchCode { get; set; }
    public bool IsActive { get; set; }
}