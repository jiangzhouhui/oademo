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
    
    public partial class B_OTHERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public B_OTHERS()
        {
            this.R_OTHERS = new HashSet<R_OTHERS>();
        }
    
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
        public string TEAM_ID { get; set; }
        public Nullable<System.DateTime> B_STARTDATE { get; set; }
        public Nullable<System.DateTime> B_ENDDATE { get; set; }
        public string B_PROJECTNAME { get; set; }
        public string B_PLACE { get; set; }
        public string B_TYPE { get; set; }
        public string B_REASON { get; set; }
        public string B_CURRENCY { get; set; }
        public Nullable<double> B_RATE { get; set; }
        public Nullable<decimal> B_ORIGINALCURRENCY { get; set; }
        public string B_COUNT { get; set; }
        public Nullable<double> B_TAXRATE { get; set; }
        public Nullable<decimal> B_TAX { get; set; }
        public Nullable<decimal> B_TAXFREEAMOUNT { get; set; }
        public Nullable<decimal> B_CNYSUBTOTAL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<R_OTHERS> R_OTHERS { get; set; }
    }
}
