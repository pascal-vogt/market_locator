namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ALLOWED_GLOBAL_ACTIONS")]
    public class AllowedGlobalActions
    {
        [Column("ALLOWED_GLOBAL_ACTIONS_ID")] 
        public long Id { get; set; }
            
        [Column("EMAIL")] 
        public string Email { get; set; }
        
        [Column("ACTION")] 
        public string Action { get; set; }
        
        [Column("ROLE")] 
        public string Role { get; set; }
    }
}