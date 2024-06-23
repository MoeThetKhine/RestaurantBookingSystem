using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantBookingSystemApi.Controllers;
using RestaurantBookingSystemApi.Model.Booking;
using RestaurantBookingSystemApi.Model.Tables;

namespace RestaurantBookingSystemApi.Mapper
{
    public static class ChangeModel
    {
        public static Table Change(this TablesManagementModel requestModel)
        {
            return new Table
            {
                TableNumber = requestModel.TableNumber,
                Capa
            };
        }
        public static Booking Change(this BookingRequestModel requestModel)
        {
            return new Booking
            {

                CustomerName = requestModel.CustomerName,
                PhoneNumber = requestModel.PhoneNumber,
                NumberOfPeople = requestModel.NumberOfPeople,
                BookingDateAndTime = requestModel.BookingDateAndTime,
                BranchCode = requestModel.BranchCode,
                TableNumber = requestModel.TableNumber,
                UserName = requestModel.UserName,
                IsBooked = requestModel.IsBooked
            };
        }
            
            

    

    }
}
