//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Dispositions
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IdCampaign { get; set; }
        public int IdResult { get; set; }
    
        public virtual CallResult CallResult { get; set; }
        public virtual Campaign Campaign { get; set; }
    }
}
