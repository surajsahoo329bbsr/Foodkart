//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Foodkart
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admin
    {
        public long AdminId { get; set; }
        public string AdminFName { get; set; }
        public string AdminLName { get; set; }
        public string AdminPassword { get; set; }
        public long AdminMenuId { get; set; }
    
        public virtual Menu Menu { get; set; }
    }
}
