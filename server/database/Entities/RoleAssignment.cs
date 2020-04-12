using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("ROLE_ASSIGNMENT")]
    public class RoleAssignment
    {
        [Column("ROLE_ASSIGNMENT_ID")] 
        public long Id { get; set; }
            
        [Column("USER_ID")] 
        public long UserId { get; set; }
            
        [ForeignKey("UserId")] 
        public virtual  User User { get; set; }
            
        [Column("ROLE_ID")] 
        public long RoleId { get; set; }
            
        [ForeignKey("RoleId")] 
        public virtual Role Role { get; set; }
    }
}