namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ACTION")]
    public class Action
    {
        [Column("ACTION_ID")] 
        public long Id { get; set; }
            
        [Column("CODE")] 
        public string Code { get; set; }
        
        [Column("STAND_SPECIFIC")] 
        public bool StandSpecific { get; set; }
    }
}