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
    
    public partial class Calls
    {
        public int Id { get; set; }
        public string ucid { get; set; }
        public int IdAgent { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCustomer { get; set; }
        public int IdDispositionCampaign { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual DispositionCampaigns DispositionCampaigns { get; set; }
        public virtual Users Users { get; set; }
    }
}
