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
    
    public partial class CampaingSkills
    {
        public int Id { get; set; }
        public int IdSkill { get; set; }
        public int IdCampaing { get; set; }
    
        public virtual Campaing Campaing { get; set; }
        public virtual Skills Skills { get; set; }
    }
}
