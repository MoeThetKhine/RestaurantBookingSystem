using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Booking;
using RestaurantBookingSystemApi.Model.Tables;

namespace RestaurantBookingSystemApi.Controllers;

public class BookingController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public BookingController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    [Route("/api/Booking")]
    public async Task<IActionResult> GetBooking()
    {
        try
        {
            List<BookingManagementModel> lst = await _appDbContext.Booking
                .AsNoTracking()
                .ToListAsync();
            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/Booking")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingManagementModel managementmodel)
    {
        try
        {
            if (string.IsNullOrEmpty(managementmodel.CustomerName) ||
                string.IsNullOrEmpty(managementmodel.PhoneNumber) || 
                string.IsNullOrEmpty(managementmodel.NumberOfPeople) ||
                string.IsNullOrEmpty(managementmodel.BranchCode) ||
                string.IsNullOrEmpty(managementmodel.TableNumber) || 
                string.IsNullOrEmpty(managementmodel.UserName))
            {
                return BadRequest();
            }
            if (managementmodel.BookingDateAndTime < DateTime.Now)
            {
                return BadRequest("Your Booking is not approved");
            }
            var item = await _appDbContext.Booking
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerName == managementmodel.CustomerName  && x.BookingDateAndTime == managementmodel.BookingDateAndTime && x.IsBooked);
            if (item is not null)
                return Conflict("Customer with same Booking Date/Time is already exist");
            await _appDbContext.Booking.AddAsync(managementmodel);
            int result = await _appDbContext.SaveChangesAsync();
            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/Booking/{id}")]
    public async Task<IActionResult> DeleteBooking(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();
            var item = await _appDbContext.Booking
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BookingId == id && x.IsBooked);
            if (item is null)
                return NotFound("Booking Not Found or Active");
            item.IsBooked = false;
            _appDbContext.Entry(item).State = EntityState.Modified;
            int result = await _appDbContext.SaveChangesAsync();
            return result > 0 ? StatusCode(202, "Deleting Successful") : BadRequest("Deleting Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}