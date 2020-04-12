using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("USER")]
    public class User
    {
        [Column("USER_ID")] 
        public long Id { get; set; }
            
        [Column("EMAIL")] 
        public string Email { get; set; }
            
        [Column("PASSWORD_HASH")] 
        public string PasswordHash { get; set; }
            
        [Column("PASSWORD_SALT")] 
        public string PasswordSalt { get; set; }
            
        [InverseProperty("User")]
        public virtual ICollection<RoleAssignment> RoleAssignments { get; set; }
    }
}