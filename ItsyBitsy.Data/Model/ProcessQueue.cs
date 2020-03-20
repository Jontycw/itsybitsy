using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    public class ProcessQueue
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Url)]
        public Uri Link { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        [ForeignKey("SessionId")]
        public Session Session { get; set; }
        public int SessionId { get; set; }
    }
}
