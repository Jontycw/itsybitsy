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

        [ForeignKey("WebsiteId")]
        public virtual Website Website { get; set; }

        public int WebsiteId { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        public int SessionId { get; set; }
    }
}
