using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class RoleManageModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "角色名称")]
        public string KEYED_NAME { get; set; }

        [Display(Name = "人员列表")]
        public string PersonList { get; set; }

        [Display(Name = "部门")]
        public string Department { get; set; }

        [Display(Name = "地区")]
        public string Region { get; set; }


    }
}