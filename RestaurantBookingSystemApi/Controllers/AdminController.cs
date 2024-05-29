using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Admin;

namespace RestaurantBookingSystemApi.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public AdminController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Route("/api/User")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<AdminManagementModel>lst = await _appDbContext.Users
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
        [Route("/api/User")]
        public async Task<IActionResult> CreateUser([FromBody] AdminManagementModel managementmodel)
        {
            try
            {
                if (string.IsNullOrEmpty(managementmodel.BranchCode) || string.IsNullOrEmpty(managementmodel.UserName) || string.IsNullOrEmpty(managementmodel.Email))
                {
                    return BadRequest();
                }
                var item = await _appDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.BranchCode == managementmodel.BranchCode && x.IsActive);
                if (item is not null)
                    return Conflict("Admin already exists!");
                await _appDbContext.Users.AddAsync(managementmodel);
                int result = await _appDbContext.SaveChangesAsync();
                return result > 0 ? StatusCode(201, "Branchcode Successful") : BadRequest("Branchcode Fail");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPut]
        [Route("/api/User")]
        public async Task<IActionResult> UpdateUser([FromBody]AdminManagementModel requestmodel,long id)
        {
            try
            {
                if(string.IsNullOrEmpty(requestmodel.BranchCode) || id <= 0)
                    return BadRequest();
                bool isDuplicate = await _appDbContext.Users
                    .AsNoTracking()
                    .AnyAsync(x => x.BranchCode == requestmodel.BranchCode && x.IsActive && x.UserId != id );
                if(isDuplicate)
                    return Conflict("BranchCode already exists!");
                var item = await _appDbContext.Users 
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x=> x.UserId == id && x.IsActive);
                if (item is null)
                    return NotFound("BranchCode  Not Found Or Active");
                item.UserName = requestmodel.UserName;
                _appDbContext.Entry(item).State = EntityState.Modified;
                int result = await _appDbContext.SaveChangesAsync();
                return result > 0 ? StatusCode(202, "Updating Successful") :
                BadRequest("Updating Fai");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete]
        [Route("/api/User/{id}")]
        public async Task<IActionResult>DeleteUser(long id)
        {
            try
            {
                if(id <= 0)
                    return BadRequest();
                var item = await _appDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x=> x.UserId == id && x.IsActive);
                if (item is null)
                    return NotFound("Admin not found or Active");
                item.IsActive = false;
                _appDbContext.Entry(item).State |= EntityState.Modified;
                int result = await _appDbContext.SaveChangesAsync();
                return result > 0 ? StatusCode(202, "Deleting Successful") : BadRequest("Deleting Fail");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
    }
}