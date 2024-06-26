﻿using Microsoft.AspNetCore.Mvc;
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
    #region Table List

    [HttpGet]
    [Route("/api/Tables/{branchCode}")]
    public async Task<IActionResult> GetTables(string branchCode)
    {
        try
        {
            List<TablesManagementModel> lst = await _appDbContext.Tables
                .Where(b => b.BranchCode == branchCode && b.IsAvailable == true)
                .AsNoTracking()
                .ToListAsync();
            if (lst == null || lst.Count == 0)
            {
                return NotFound("No available tables found for the given branch code.");
            }
            return Ok(lst);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Create New Table

    [HttpPost]
    [Route("/api/Tables")]
    public async Task<IActionResult> CreateTables([FromBody] TablesManagementModel managementModel)
    {
        try
        {
            #region Validation

            if (string.IsNullOrEmpty(managementModel.TableNumber))
                return BadRequest();
            if (string.IsNullOrEmpty(managementModel.Capacity))
                return BadRequest();
            if (string.IsNullOrEmpty(managementModel.Location))
                return BadRequest();
            if (string.IsNullOrEmpty(managementModel.BranchCode))
                return BadRequest();

            #endregion

            var item = await _appDbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TableNumber == managementModel.TableNumber
                && x.BranchCode == managementModel.BranchCode && x.IsAvailable);

            if (item is not null)
                return Conflict("Table Number already exists in this BranchCode!");

            await _appDbContext.Tables.AddAsync(managementModel);
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(201, "Creating Successful") : BadRequest("Creating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    #endregion

    #region Change IsAvailable False to True

    [HttpPut]
    [Route("/api/Tables")]
    public async Task<IActionResult> UpdateTables([FromBody] TableRequestModel requestModel, long id)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel.TableNumber) || id <= 0)
                return BadRequest();

            bool isDuplicate = await _appDbContext.Tables
                .AsNoTracking()
                .AnyAsync(x => x.TableNumber == requestModel.TableNumber && x.BranchCode == requestModel.BranchCode && x.IsAvailable && x.TableId != id);
            if (isDuplicate)
                return Conflict("Table Number already exists");

            var item = await _appDbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TableId == id && x.IsAvailable == false);

            if (item is null)
                return NotFound("Table Not Found Or IsActive");

            item.IsAvailable = true;
            _appDbContext.Entry(item).State = EntityState.Modified;
            int result = await _appDbContext.SaveChangesAsync();

            return result > 0 ? StatusCode(202, "This table is available") : BadRequest("Updating Fail");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region IsAvailable False when table is booked

    [HttpDelete]
    [Route("/api/Tables/{id}")]
    public async Task<IActionResult> DeleteTables(long id)
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

            return result > 0 ? StatusCode(202, "This table is booked") : BadRequest();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion







}