namespace Database.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GOOGLE_DOC_IMPORTED_ROW")]
    public class GoogleDocImportedRow
    {
        [Column("GOOGLE_DOC_IMPORTED_ROW_ID")]
        public long Id { get; set; }
        
        [Column("COL_0")]
        public string Col0 { get; set; }
        
        [Column("COL_1")]
        public string Col1 { get; set; }
        
        [Column("COL_2")]
        public string Col2 { get; set; }
        
        [Column("COL_3")]
        public string Col3 { get; set; }
        
        [Column("COL_4")]
        public string Col4 { get; set; }
        
        [Column("COL_5")]
        public string Col5 { get; set; }
        
        [Column("COL_6")]
        public string Col6 { get; set; }
    }
}