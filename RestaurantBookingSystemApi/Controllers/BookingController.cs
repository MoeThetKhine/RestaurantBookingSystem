using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    #region Booking List By BranchCode

    [HttpGet]
    [Route("/api/Booking/branchCode")]
    public async Task<IActionResult> GetBooking(string branchCode)
    {
        try
        {
            List<BookingManagementModel> lst = await _appDbContext.Booking
                .Where(b => b.BranchCode == branchCode && b.IsBooked == true)
                .AsNoTracking()
                .ToListAsync();
            if (lst == null || lst.Count == 0)
            {
                return NotFound("All tables are available at this Branch");
            }

            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Create Booking

    [HttpPost]
    [Route("/api/Booking")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingManagementModel managementmodel)
    {
        try
        {
            #region Validation

            if (string.IsNullOrEmpty(managementmodel.CustomerName))
                return BadRequest("Please fill CustomerName");

            if (string.IsNullOrEmpty(managementmodel.PhoneNumber))
                return BadRequest("Please fill PhoneNumber");

            if (string.IsNullOrEmpty(managementmodel.NumberOfPeople))
                return BadRequest("Please fill Number Of People");

            if (string.IsNullOrEmpty(managementmodel.BranchCode))
                return BadRequest("Please fill BranchCode");

            if (string.IsNullOrEmpty(managementmodel.TableNumber))
                return BadRequest("Please fill TableNumber");

            if (string.IsNullOrEmpty(managementmodel.UserName))
                return BadRequest("Please fill UserName");

            if (managementmodel.BookingDateAndTime < DateTime.Now || managementmodel.BookingDateAndTime == default)
                return BadRequest("Your Booking is not approved");
            #endregion

            var item = await _appDbContext.Booking
                .AsNoTracking()
                .FirstOrDefaultAsync
                (x => (x.CustomerName == managementmodel.CustomerName && x.BookingDateAndTime == managementmodel.BookingDateAndTime && x.IsBooked)
                || (x.TableNumber == managementmodel.TableNumber && x.BranchCode == managementmodel.BranchCode && x.IsBooked));
            if (item is not null)
                return Conflict("Customer with same Booking Date/Time is already exist or Table is not available");
            await _appDbContext.Booking.AddAsync(managementmodel);
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region DeleteBooking

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

    #endregion



}

