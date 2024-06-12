using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Admin;

namespace RestaurantBookingSystemApi.Controllers;

public class AdminController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public AdminController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /*[HttpGet]
    [Route("/api/User/branchcode")]
    public async Task<IActionResult> GetUsers(string branchcode)
    {
        try
        {
            List<AdminManagementModel> lst = await _appDbContext.Users
                .AsNoTracking()
                .ToListAsync();
            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    } */

    [HttpGet]
    [Route("/api/User/branchcode")]
    public async Task<IActionResult> GetUsers(string branchcode)
    {
        try
        {
            var branch = await _appDbC
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/User")]
    public async Task<IActionResult> CreateUser([FromBody] AdminManagementModel managementModel)
    {
        try
        {
            if (string.IsNullOrEmpty(managementModel.UserName) || string.IsNullOrEmpty(managementModel.Email) || string.IsNullOrEmpty(managementModel.Password) || string.IsNullOrEmpty(managementModel.BranchCode))
            {
                return BadRequest();
            }

            var item = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BranchCode == managementModel.BranchCode && x.IsActive);
            if (item is not null)
                return Conflict("Admin already exists!");
            await _appDbContext.Users.AddAsync(managementModel);
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut]
    [Route("/api/User")]
    public async Task<IActionResult> UpdateUser([FromBody] AdminManagementModel requestModel, long id)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel.UserName))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.Email))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.Password))
                return BadRequest();

            if (string.IsNullOrEmpty(requestModel.BranchCode))
                return BadRequest();

            if (id <= 0)
                return BadRequest();

            bool isDuplicate = await _appDbContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.BranchCode == requestModel.BranchCode && x.IsActive && x.UserId != id);
            if (isDuplicate)
                return Conflict("Admin already exists!");

            var item = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == id && x.IsActive);
            if (item is null)
                return NotFound("Admin Not Found Or InActive");

            item.Password = requestModel.Password;
            _appDbContext.Entry(item).State = EntityState.Modified;
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(202, "Updating Successful") : BadRequest("Updating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/User/{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();

            var item = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == id && x.IsActive);
            if (item is null)
                return NotFound("Admin not found or Active");

            item.IsActive = false;
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