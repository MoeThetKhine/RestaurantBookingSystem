using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Branch;

namespace RestaurantBookingSystemApi.Controllers;

public class BranchController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public BranchController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    [Route("/api/Branch")]
    public async Task<IActionResult> GetBranches()
    {
        try
        {
           // List<BranchManagementModel> lst = await _appDbContext.Branches
            var branch = await _appDbContext.Branches
            .Where(b => b.IsActive == true)
            .AsNoTracking()
            .ToListAsync();

            return Ok(branch);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/Branch")]
    public async Task<IActionResult> CreateBranches([FromBody] BranchManagementModel managementmodel)
    {
        try
        {
            if (string.IsNullOrEmpty(managementmodel.BranchCode) || string.IsNullOrEmpty(managementmodel.BranchName))
                return BadRequest();

            var item = await _appDbContext.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BranchCode == managementmodel.BranchCode
                && x.BranchName == managementmodel.BranchName && x.IsActive);

            if (item is not null)
                return Conflict("Branch already exists");

            await _appDbContext.Branches.AddAsync(managementmodel);
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/Branch")]
    public async Task<IActionResult> DeleteBranches(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();

            var item = await _appDbContext.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BranchId == id && x.IsActive);

            if (item is null)
                return NotFound("Branch Name Not Found or Active");

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