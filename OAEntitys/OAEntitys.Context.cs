﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace OAEntitys
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class InnovatorSolutionsEntities : DbContext
    {

        public void FixEfProviderServicesProblem()
        { //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer' //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. //Make sure the provider assembly is available to the running application. //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information. 
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public InnovatorSolutionsEntities()
            : base("name=InnovatorSolutionsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACTIVITY> ACTIVITY { get; set; }
        public virtual DbSet<ACTIVITY_ASSIGNMENT> ACTIVITY_ASSIGNMENT { get; set; }
        public virtual DbSet<ACTIVITY_TEMPLATE> ACTIVITY_TEMPLATE { get; set; }
        public virtual DbSet<HISTORY> HISTORY { get; set; }
        public virtual DbSet<IDENTITY> IDENTITY { get; set; }
        public virtual DbSet<MEMBER> MEMBER { get; set; }
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<WORKFLOW> WORKFLOW { get; set; }
        public virtual DbSet<WORKFLOW_MAP> WORKFLOW_MAP { get; set; }
        public virtual DbSet<WORKFLOW_MAP_ACTIVITY> WORKFLOW_MAP_ACTIVITY { get; set; }
        public virtual DbSet<WORKFLOW_MAP_PATH> WORKFLOW_MAP_PATH { get; set; }
        public virtual DbSet<WORKFLOW_PROCESS> WORKFLOW_PROCESS { get; set; }
        public virtual DbSet<WORKFLOW_PROCESS_ACTIVITY> WORKFLOW_PROCESS_ACTIVITY { get; set; }
        public virtual DbSet<WORKFLOW_PROCESS_PATH> WORKFLOW_PROCESS_PATH { get; set; }
        public virtual DbSet<B_PROJECTMANAGE> B_PROJECTMANAGE { get; set; }
        public virtual DbSet<B_PRMANAGE> B_PRMANAGE { get; set; }
        public virtual DbSet<B_EXPENSEREIMBURSEMENT> B_EXPENSEREIMBURSEMENT { get; set; }
        public virtual DbSet<B_EXPENSECATEGORY> B_EXPENSECATEGORY { get; set; }
        public virtual DbSet<B_ORGANIZATIONALSTRUCTURE> B_ORGANIZATIONALSTRUCTURE { get; set; }
        public virtual DbSet<B_REIMBURSEMENTITEM> B_REIMBURSEMENTITEM { get; set; }
        public virtual DbSet<R_REIMBURSEMENTITEM> R_REIMBURSEMENTITEM { get; set; }
        public virtual DbSet<B_TRIPREIMBURSEMENTFORM> B_TRIPREIMBURSEMENTFORM { get; set; }
        public virtual DbSet<B_EXPENSEAUDITCONFIGURATION> B_EXPENSEAUDITCONFIGURATION { get; set; }
        public virtual DbSet<B_HOTELEXPENSE> B_HOTELEXPENSE { get; set; }
        public virtual DbSet<B_MEALSANDFIXEDSUBSIDIES> B_MEALSANDFIXEDSUBSIDIES { get; set; }
        public virtual DbSet<B_OTHERS> B_OTHERS { get; set; }
        public virtual DbSet<B_TRAFFICEXPENSE> B_TRAFFICEXPENSE { get; set; }
        public virtual DbSet<R_HOTELEXPENSE> R_HOTELEXPENSE { get; set; }
        public virtual DbSet<R_MEALSANDFIXEDSUBSIDIES> R_MEALSANDFIXEDSUBSIDIES { get; set; }
        public virtual DbSet<R_OTHERS> R_OTHERS { get; set; }
        public virtual DbSet<R_TRAFFICEXPENSE> R_TRAFFICEXPENSE { get; set; }
        public virtual DbSet<B_TAXCODECONFIGURE> B_TAXCODECONFIGURE { get; set; }
        public virtual DbSet<B_PRCHOICESUPPLIERS> B_PRCHOICESUPPLIERS { get; set; }
        public virtual DbSet<B_CHOICESUPPLIERS> B_CHOICESUPPLIERS { get; set; }
        public virtual DbSet<B_AGENTSET> B_AGENTSET { get; set; }
        public virtual DbSet<B_BUSINESSTRAVEL> B_BUSINESSTRAVEL { get; set; }
        public virtual DbSet<B_WORKFLOWVERSIONMANAGEMENT> B_WORKFLOWVERSIONMANAGEMENT { get; set; }
        public virtual DbSet<B_BOOKINGSTAFFING> B_BOOKINGSTAFFING { get; set; }
    }
}