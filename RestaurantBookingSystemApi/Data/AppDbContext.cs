using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBookingSystemApi.Model.Admin;
using RestaurantBookingSystemApi.Model.Booking;
using RestaurantBookingSystemApi.Model.Branch;
using RestaurantBookingSystemApi.Model.Tables;
using System.Data.Common;

namespace RestaurantBookingSystemApi.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AdminManagementModel> Users { get; set; }
        public DbSet<BranchManagementModel> Branches { get; set; }
        public DbSet<TablesManagementModel> Tables { get; set; }
        public DbSet<BookingManagementModel> Booking { get; set; }
    }
}