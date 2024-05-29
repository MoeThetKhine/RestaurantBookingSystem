using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Data;
using RestaurantBookingSystemApi.Model.Tables;

namespace RestaurantBookingSystemApi.Controllers;

public class TableController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public TableController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    [HttpGet]
    [Route("/api/Tables")]

    public async Task<IActionResult> GetTables()
    {
        try
        {
            List<TablesManagementModel>lst=await _appDbContext.Tables
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
    [Route("/api/Tables")]
    public async Task<IActionResult> CreateTables([FromBody]TablesManagementModel managementModel)
    {
        try
        {
            if (string.IsNullOrEmpty(managementModel.TableNumber))
            {
                return BadRequest();
            }
            var item = await _appDbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TableNumber == managementModel.TableNumber && x.BranchCode == managementModel.BranchCode && x.IsAvailable);
            if (item is not null)
                return Conflict("Table Number already exists!");
            await _appDbContext.Tables.AddAsync(managementModel);
            int result = await _appDbContext.SaveChangesAsync();
            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut]
    [Route("/api/Tables")]
    public async Task<IActionResult> UpdateTables([FromBody]TableRequestModel requestModel,long id)
    {
        try
        {
            if(string.IsNullOrEmpty(requestModel.TableNumber)|| id <= 0)
                return BadRequest();
            bool isDuplicate = await _appDbContext.Tables
                .AsNoTracking()
                .AnyAsync(x=> x.TableNumber == requestModel.TableNumber && x.BranchCode == requestModel.BranchCode && x.IsAvailable && x.TableId != id );
            if (isDuplicate)
                return Conflict("Table Number already exists");
            var item = await _appDbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TableId == id && x.IsAvailable);
            if (item is null)
                return NotFound("Table Not Found Or IsActive");
            item.TableNumber = requestModel.TableNumber;
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
    [Route("/api/Tables/{id}")]
    public async Task<IActionResult>DeleteTables(long id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();
            var item = await _appDbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TableId == id && x.IsAvailable);
            if (item is null)
                return NotFound("Table is not available now");
            item.IsAvailable = false;
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
