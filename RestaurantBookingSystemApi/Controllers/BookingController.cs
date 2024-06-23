using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Mapper;
using RestaurantBookingSystemApi.Model.Booking;

namespace RestaurantBookingSystemApi.Controllers;

public class BookingController : BaseController
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
            #region Validation

            if (string.IsNullOrEmpty(managementmodel.CustomerName))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.PhoneNumber))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.NumberOfPeople))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.NumberOfPeople))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.BranchCode))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.TableNumber))
                return BadRequest();

            if (string.IsNullOrEmpty(managementmodel.UserName))
                return BadRequest();

            if (managementmodel.BookingDateAndTime < DateTime.Now || managementmodel.BookingDateAndTime == default)
                return BadRequest("Your Booking is not approved");

            #endregion

            var item = await _appDbContext.Booking
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerName == managementmodel.CustomerName && x.BookingDateAndTime == managementmodel.BookingDateAndTime && x.IsBooked);
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



    [HttpPost]
    public async Task<IActionResult> CreateTables([FromBody] BookingRequestModel requestModel)
    {


        await using var transaction = await _appDbContext.Database.BeginTransactionAsync();

        try
        {

            if (string.IsNullOrEmpty(requestModel.CustomerName))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.PhoneNumber))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.NumberOfPeople))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.NumberOfPeople))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.BranchCode))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.TableNumber))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.UserName))
                return BadRequest();

            if (requestModel.BookingDateAndTime < DateTime.Now || requestModel.BookingDateAndTime == default)
                return BadRequest("Your Booking is not approved");

            if(requestModel.TableBooking is null)
                return BadRequest();

            // Check if the table is available
            var table = await _appDbContext.Tables
                    .Where(t => t.TableNumber == requestModel.TableNumber && t.IsAvailable)
                    .FirstOrDefaultAsync();

            if (table == null)
            {
                return BadRequest("Table is not available.");
            }

            // Create a new booking
            var booking = new Booking
            {
                TableNumber = requestModel.TableNumber,
                BookingDate = requestModel.BookingDateAndTime
                // Set other properties as needed
            };
            await _appDbContext.Booking.AddAsync(booking);

            // Mark the table as unavailable
            table.IsAvailable = false;
            _appDbContext.Tables.Update(table);

            // Save changes
            int bookingResult = await _appDbContext.SaveChangesAsync();

            if (bookingResult > 0)
            {
                await transaction.CommitAsync();
                return CreatedAtAction(nameof(GetBooking), new { num = requestModel.TableNumber }, booking);
            }

            await transaction.RollbackAsync();
            return BadRequest("Failed to book the table.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Internal server error.");
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

//internal class Booking : BookingManagementModel
//{
//    public string TableNumber { get; set; }
//    public DateTime BookingDate { get; set; }
//}