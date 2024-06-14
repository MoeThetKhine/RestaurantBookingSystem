using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Admin;
using RestaurantBookingSystemApi.Model.Tables;
using System.Text.RegularExpressions;

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
            //if (string.IsNullOrEmpty(branchcode))
            //{
            //    return BadRequest("Branch code cannot be null or empty.");
            //}
            // List<TablesManagementModel> lst = await _appDbContext.Users
            var user = await _appDbContext.Users
                .Where(b => b.BranchCode == branchcode && b.IsActive == true)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("No active user found for the given branch code.");
            }

            return Ok(user);
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
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        const string passpattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        try
        {
            if (string.IsNullOrEmpty(managementModel.UserName))
                return BadRequest("UserName cannot empty");
            if (string.IsNullOrEmpty(managementModel.Email))
                return BadRequest("Email cannot empty");
            if (string.IsNullOrEmpty(managementModel.Password))
                return BadRequest("Password cannot empty");
            if (string.IsNullOrEmpty(managementModel.BranchCode))
                return BadRequest("BranchCode cannot empty");
            if (string.IsNullOrEmpty(managementModel.UserRole))
                return BadRequest("UserRole cannot empty");
            if (managementModel.UserRole != "Admin")
                return BadRequest("UserRole must be only Admin");
            if (!(Regex.IsMatch(managementModel.Email.ToString(), pattern)))
                return BadRequest("Invalid email format.");
            if (!(Regex.IsMatch(managementModel.Password.ToString(), passpattern)))
                return BadRequest("Password must be at least 8 characters long and " +
                    "contain an uppercase " + "letter, " +
                    "a lowercase letter, a number, and a special character.");
            if (managementModel.BranchCode.Length != 5)
                return BadRequest("BranchCode must be only 5  character");

            var item = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BranchCode == managementModel.BranchCode && x.IsActive);
            if (item is not null)
                return Conflict("Admin already exists with that BranchCode!");
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
        const string passpattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        try
        {
           /* if (string.IsNullOrEmpty(requestModel.UserName))
                return BadRequest("UserName cannot empty");
            if (string.IsNullOrEmpty(requestModel.Email))
                return BadRequest("Email cannot empty");
            if (string.IsNullOrEmpty(requestModel.Password))
                return BadRequest("Password cannot empty");
            if (string.IsNullOrEmpty(requestModel.BranchCode))
                return BadRequest("BranchCode cannot empty");
            if (string.IsNullOrEmpty(requestModel.UserRole))
                return BadRequest("UserRole cannot empty");*/
             if (!(Regex.IsMatch(requestModel.Password.ToString(), passpattern)))
                return BadRequest("Password must be at least 8 characters long and " +
                    "contain an uppercase " + "letter, " +
                    "a lowercase letter, a number, and a special character.");
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