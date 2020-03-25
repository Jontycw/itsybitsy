using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItsyBitsy.Data
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime{ get; set; }
    }
}