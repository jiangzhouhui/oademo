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
    
    public partial class ACTIVITY_TEMPLATE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACTIVITY_TEMPLATE()
        {
            this.WORKFLOW_MAP_ACTIVITY = new HashSet<WORKFLOW_MAP_ACTIVITY>();
        }
    
        public string CLASSIFICATION { get; set; }
        public string ID { get; set; }
        public string KEYED_NAME { get; set; }
        public System.DateTime CREATED_ON { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string MODIFIED_BY_ID { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string STATE { get; set; }
        public string CURRENT_STATE { get; set; }
        public string LOCKED_BY_ID { get; set; }
        public string MAJOR_REV { get; set; }
        public string IS_CURRENT { get; set; }
        public string MINOR_REV { get; set; }
        public string IS_RELEASED { get; set; }
        public string NOT_LOCKABLE { get; set; }
        public string TEAM_ID { get; set; }
        public string CSS { get; set; }
        public string CAN_DELEGATE { get; set; }
        public string CAN_REFUSE { get; set; }
        public string CONSOLIDATE_ONDELEGATE { get; set; }
        public string ESCALATE_TO { get; set; }
        public Nullable<int> EXPECTED_DURATION { get; set; }
        public string ICON { get; set; }
        public string IS_AUTO { get; set; }
        public string IS_END { get; set; }
        public string IS_START { get; set; }
        public string LABEL { get; set; }
        public string MESSAGE { get; set; }
        public string NAME { get; set; }
        public Nullable<int> PRIORITY { get; set; }
        public Nullable<int> REMINDER_COUNT { get; set; }
        public Nullable<int> REMINDER_INTERVAL { get; set; }
        public string ROLE { get; set; }
        public string SUBFLOW { get; set; }
        public Nullable<int> TIMEOUT_DURATION { get; set; }
        public string WAIT_FOR_ALL_INPUTS { get; set; }
        public string WAIT_FOR_ALL_VOTES { get; set; }
        public Nullable<int> X { get; set; }
        public Nullable<int> Y { get; set; }
        public string CONFIG_ID { get; set; }
        public Nullable<int> GENERATION { get; set; }
        public string MANAGED_BY_ID { get; set; }
        public string NEW_VERSION { get; set; }
        public string OWNED_BY_ID { get; set; }
        public string PERMISSION_ID { get; set; }
        public string LABEL_ZC1 { get; set; }
        public string MESSAGE_ZC1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WORKFLOW_MAP_ACTIVITY> WORKFLOW_MAP_ACTIVITY { get; set; }
    }
}