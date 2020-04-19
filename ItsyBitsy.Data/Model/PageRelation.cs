using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ItsyBitsy.Data
{
    [Table("PageRelation")]
    public class PageRelation
    {
        public virtual Page ParentPage { get; set; }

        [Key, Column(Order=1), ForeignKey("ParentPage")]
        public int ParentPageId { get; set; }

        public virtual Page ChildPage { get; set; }

        [Key, Column(Order = 2), ForeignKey("ChildPage")]
        public int ChildPageId { get; set; }
    }
}
