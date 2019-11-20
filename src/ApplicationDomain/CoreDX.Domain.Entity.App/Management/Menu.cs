using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using CoreDX.Domain.Model.Entity;

//菜单类在输出到菜单视图前要转化成视图专用结构，去除多余数据，编辑中才用完整数据
namespace CoreDX.Domain.Entity.App.Management
{
    //[Owned]
    public class MenuIcon
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    //[Owned]
    public class MenuItemIcon
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class MenuItem : MenuItem<int> { }

    public abstract class MenuItem<TMenuId> : DomainEntityBase<int, int>
      where TMenuId : struct, IEquatable<TMenuId>
    {
        public MenuItemIcon MenuItemIcon { get; set; } = new MenuItemIcon();
        public string Title { get; set; }
        public string Link { get; set; }
        public short Order { get; set; }

        public virtual TMenuId MenuId { get; set; }
        public virtual Menu Menu { get; set; }
    }

    //public class Group
    //{
    //    public Icon Icon { get; set; }
    //    public string Title { get; set; }
    //    public List<Item> Items { get; set; }
    //}

    public class Menu : DomainTreeEntityBase<int, Menu, int>
    {
        public MenuIcon MenuIcon { get; set; } = new MenuIcon();
        public string Title { get; set; }
        public short Order { get; set; }

        public virtual List<MenuItem> Items { get; set; } = new List<MenuItem>();
        //public List<Group> Groups { get; set; }
    }

    public class MenuView : DomainTreeEntityViewBase<int, MenuView, int>
    {
        public string MenuIcon_Type { get; set; }
        public string MenuIcon_Value { get; set; }
        //public MenuIcon MenuIcon { get; set; } = new MenuIcon();
        public string Title { get; set; }
        public short Order { get; set; }
    }
}
