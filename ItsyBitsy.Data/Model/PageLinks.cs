using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    public class PageLinks
    {
        public Page Page { get; set; }

        [Key, Column(Order = 0), ForeignKey("Page")]
        public int PageId { get; set; }

        public Page ParentPage { get; set; }

        [Key, Column(Order = 1), ForeignKey("ParentPage")]
        public int ParentPageId { get; set; }

        public virtual Session Session { get; set; }

        [Key, Column(Order = 2), ForeignKey("Session")]
        public int SessionId { get; set; }
    }
}
