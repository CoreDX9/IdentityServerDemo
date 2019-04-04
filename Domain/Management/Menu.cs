using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Domain.Management
{
    [Owned]
    public class Icon
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class MenuItem : DomainEntityBase<Guid, Guid>
    {
        public string Index { get; set; }
        public Icon Icon { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public short Order { get; set; }

        public virtual Guid MenuId { get; set; }
        public virtual Menu Menu { get; set; }
    }

    //public class Group
    //{
    //    public Icon Icon { get; set; }
    //    public string Title { get; set; }
    //    public List<Item> Items { get; set; }
    //}

    public class Menu : DomainTreeEntityBase<Guid, Menu, Guid>
    {
        public string Index { get; set; }
        public Icon Icon { get; set; }
        public string Title { get; set; }
        public short Order { get; set; }

        public virtual List<MenuItem> Items { get; set; }
        //public List<Group> Groups { get; set; }
    }
}
