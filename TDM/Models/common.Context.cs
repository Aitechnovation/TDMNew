﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TDM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class commonEntities : DbContext
    {
        public commonEntities()
            : base("name=commonEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TB_MAS_LANDOFFICE> TB_MAS_LANDOFFICE { get; set; }
        public virtual DbSet<TB_MAS_AMPHUR> TB_MAS_AMPHUR { get; set; }
        public virtual DbSet<TB_MAS_PROVINCE> TB_MAS_PROVINCE { get; set; }
        public virtual DbSet<TB_MAS_TAMBOL> TB_MAS_TAMBOL { get; set; }
    }
}
