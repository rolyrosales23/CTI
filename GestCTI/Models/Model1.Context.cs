﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GestCTI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBCTIEntities : DbContext
    {
        public DBCTIEntities()
            : base("name=DBCTIEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Campaing> Campaing { get; set; }
        public virtual DbSet<CampaingSkills> CampaingSkills { get; set; }
        public virtual DbSet<CampaingType> CampaingType { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanySkills> CompanySkills { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<Switch> Switch { get; set; }
        public virtual DbSet<UserLocation> UserLocation { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UserSkill> UserSkill { get; set; }
        public virtual DbSet<VDN> VDN { get; set; }
    }
}
