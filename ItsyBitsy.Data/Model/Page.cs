using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Url)]
        public string Uri { get; set; }

        [MaxLength(3)]
        public string StatusCode { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        public virtual Website Website { get; set; }

        [ForeignKey("Website")]
        public int WebsiteId { get; set; }

        public virtual Session Session { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }
    }
}
