using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

//菜单类在输出到菜单视图前要转化成视图专用结构，去除多余数据，编辑中才用完整数据
namespace CoreDX.Domain.Model.Entity.Management
{
    [Owned]
    public class Icon
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class MenuItem : DomainEntityBase<int, Guid>
    {
        public Icon Icon { get; set; } = new Icon();
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

    public class Menu : DomainTreeEntityBase<int, Menu, Guid>
    {
        public Icon Icon { get; set; } = new Icon();
        public string Title { get; set; }
        public short Order { get; set; }

        public virtual List<MenuItem> Items { get; set; } = new List<MenuItem>();
        //public List<Group> Groups { get; set; }
    }
}
