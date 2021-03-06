﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMS.Data.Infrastructure
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IMSDbContext : DbContext
    {
        public IMSDbContext()
            : base("name=IMSDbContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepreciationRate> DepreciationRates { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<DocumentNumbering> DocumentNumberings { get; set; }
        public virtual DbSet<DocumentSetup> DocumentSetups { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<FiscalYear> FiscalYears { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemGroup> ItemGroups { get; set; }
        public virtual DbSet<ItemRelease> ItemReleases { get; set; }
        public virtual DbSet<ItemReleaseDetail> ItemReleaseDetails { get; set; }
        public virtual DbSet<ItemRequest> ItemRequests { get; set; }
        public virtual DbSet<ItemRequestDetail> ItemRequestDetails { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<ItemUnit> ItemUnits { get; set; }
        public virtual DbSet<LoginHistory> LoginHistories { get; set; }
        public virtual DbSet<PasswordResetLog> PasswordResetLogs { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<StockTransaction> StockTransactions { get; set; }
        public virtual DbSet<StockTransactionDetail> StockTransactionDetails { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
    }
}
