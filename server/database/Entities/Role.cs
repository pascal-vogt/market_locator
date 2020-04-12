using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("ROLE")]
    public class Role
    {
        [Column("ROLE_ID")] 
        public long Id { get; set; }
            
        [Column("CODE")]
        public string Code { get; set; }
            
        [InverseProperty("Role")]
        public virtual ICollection<RoleAssignment> RoleAssignments { get; set; }
    }
}