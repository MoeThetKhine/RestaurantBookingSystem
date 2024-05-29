namespace RestaurantBookingSystemApi.Model.Admin
{
    public class AdminRequestModel
    {
        public string UserName {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BranchCode { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}