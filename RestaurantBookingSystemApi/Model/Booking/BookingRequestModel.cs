using RestaurantBookingSystemApi.Model.Tables;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystemApi.Model.Booking
{
    public class BookingRequestModel
    {
        [Key]
       // public long BookingId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string NumberOfPeople { get; set; }
        public DateTime BookingDateAndTime { get; set; } = DateTime.Now;
        public string BranchCode { get; set; }
        public string TableNumber { get; set; }
        public string UserName { get; set; }
        public bool IsBooked { get; set; }
       

    }
}
