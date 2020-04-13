namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ALLOWED_STAND_SPECIFIC_ACTIONS")]
    public class AllowedStandSpecificActions
    {
        [Column("ALLOWED_STAND_SPECIFIC_ACTIONS_ID")] 
        public long Id { get; set; }
            
        [Column("EMAIL")] 
        public string Email { get; set; }
        
        [Column("STAND_ID")] 
        public long StandId { get; set; }
        
        [Column("ACTION")] 
        public string Action { get; set; }
        
        [Column("ROLE")] 
        public string Role { get; set; }
    }
}