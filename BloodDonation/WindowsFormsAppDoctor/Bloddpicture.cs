using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WindowsFormsAppDoctor
{
    [Table("BLODDPICTURES")]
    public partial class Bloddpicture
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("ID_PERSON")]
        public int IdPerson { get; set; }
        [Column("ERYTROCYT", TypeName = "decimal(10, 5)")]
        public decimal? Erytrocyt { get; set; }
        [Column("FIBRINOGEN", TypeName = "decimal(10, 5)")]
        public decimal? Fibrinogen { get; set; }
        [Column("HEMOCYT", TypeName = "decimal(10, 5)")]
        public decimal? Hemocyt { get; set; }
        [Column("LEUKOCYT", TypeName = "decimal(10, 5)")]
        public decimal? Leukocyt { get; set; }
        [Column("PROTROMBIN", TypeName = "decimal(10, 5)")]
        public decimal? Protrombin { get; set; }
        [Column("TROMBOCYT", TypeName = "decimal(10, 5)")]
        public decimal? Trombocyt { get; set; }
        [Column("SAW_PERSON")]
        public int SawPerson { get; set; }
        [Column("SAW_DOCTOR")]
        public int SawDoctor { get; set; }
    }
}
