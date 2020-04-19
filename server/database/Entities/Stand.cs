namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("STAND")]
    public class Stand
    {
        [Column("STAND_ID")] 
        public long? Id { get; set; }
            
        [Column("LATITUDE")]
        public double Latitude { get; set; }
        
        [Column("LONGITUDE")]
        public double Longitude { get; set; }
        
        [Column("OPEN_MO")]
        public bool OpenMo { get; set; }
        
        [Column("OPEN_TU")]
        public bool OpenTu { get; set; }
        
        [Column("OPEN_WE")]
        public bool OpenWe { get; set; }
        
        [Column("OPEN_TH")]
        public bool OpenTh { get; set; }
        
        [Column("OPEN_FR")]
        public bool OpenFr { get; set; }
        
        [Column("OPEN_SA")]
        public bool OpenSa { get; set; }
        
        [Column("OPEN_SU")]
        public bool OpenSu { get; set; }
        
        [Column("EMAIL")]
        public string Email { get; set; }
        
        [Column("HOMEPAGE")]
        public string Homepage { get; set; }
        
        [Column("PHONE")]
        public string Phone { get; set; }
        
        [Column("TITLE")]
        public string Title { get; set; }
        
        [Column("SUMMARY")]
        public string Summary { get; set; }
    }
}