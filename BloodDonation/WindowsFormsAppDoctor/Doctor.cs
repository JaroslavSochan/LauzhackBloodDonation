using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WindowsFormsAppDoctor
{
    [Table("DOCTORS")]
    public partial class Doctor
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("LASTNAME")]
        [StringLength(50)]
        public string Lastname { get; set; }
        [Column("USERNAME")]
        [StringLength(50)]
        public string Username { get; set; }
        [Column("PASS")]
        [StringLength(50)]
        public string Pass { get; set; }
    }
}
