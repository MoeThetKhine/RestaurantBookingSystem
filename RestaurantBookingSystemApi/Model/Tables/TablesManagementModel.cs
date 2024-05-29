using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystemApi.Model.Tables;

public class TablesManagementModel
{
    [Key]
    public long TableId {  get; set; }
    public String TableNumber {  get; set; }    
    public string Capacity {  get; set; }
    public string Location { get; set;}
    public string BranchCode {  get; set; }
    public bool IsAvailable { get; set; }
}