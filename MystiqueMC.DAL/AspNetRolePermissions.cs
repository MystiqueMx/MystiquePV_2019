//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MystiqueMC.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetRolePermissions
    {
        public string RoleId { get; set; }
        public int ControllerId { get; set; }
        public int ControllerActivityId { get; set; }
    
        public virtual AspNetController AspNetController { get; set; }
        public virtual AspNetControllerActivity AspNetControllerActivity { get; set; }
        public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
