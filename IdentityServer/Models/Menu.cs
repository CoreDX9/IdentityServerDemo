using System.Collections.Generic;

namespace IdentityServer.Models
{
    public class Menu
    {
        public List<Item> Items { get; set; }
        public List<Group> Groups { get; set; }
        public List<Submenu> SubMenus { get; set; }
    }

    public class Item
    {
        public string Index { get; set; }
        public Icon Icon { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public class Group
    {
        public Icon Icon { get; set; }
        public string Title { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Submenu
    {
        public string Index { get; set; }
        public Icon Icon { get; set; }
        public string Title { get; set; }
        public List<Item> Items { get; set; }
        public List<Group> Groups { get; set; }
        public List<Submenu> SubMenus { get; set; }
    }

    public class Icon
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
