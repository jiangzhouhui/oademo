//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class R_REIMBURSEMENTITEM
    {
        public string id { get; set; }
        public string CLASSIFICATION { get; set; }
        public string KEYED_NAME { get; set; }
        public System.DateTime CREATED_ON { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string OWNED_BY_ID { get; set; }
        public string MANAGED_BY_ID { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string MODIFIED_BY_ID { get; set; }
        public string CURRENT_STATE { get; set; }
        public string STATE { get; set; }
        public string LOCKED_BY_ID { get; set; }
        public string IS_CURRENT { get; set; }
        public string MAJOR_REV { get; set; }
        public string MINOR_REV { get; set; }
        public string IS_RELEASED { get; set; }
        public string NOT_LOCKABLE { get; set; }
        public string CSS { get; set; }
        public Nullable<int> GENERATION { get; set; }
        public string NEW_VERSION { get; set; }
        public string CONFIG_ID { get; set; }
        public string PERMISSION_ID { get; set; }
        public string SOURCE_ID { get; set; }
        public string BEHAVIOR { get; set; }
        public Nullable<int> SORT_ORDER { get; set; }
        public string TEAM_ID { get; set; }
        public string RELATED_ID { get; set; }
    
        public virtual B_EXPENSEREIMBURSEMENT B_EXPENSEREIMBURSEMENT { get; set; }
        public virtual B_REIMBURSEMENTITEM B_REIMBURSEMENTITEM { get; set; }
    }
}
