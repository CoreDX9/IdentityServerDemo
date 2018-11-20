using System;
using EntityFrameworkCore.Extensions.DataAnnotations;

namespace Domain.Security
{
    [DbDescription("权限值类型，物理存储都是数字")]
    public enum PermissionValueType : sbyte
    {
        [DbDescription("布尔型，表示权限只存在有和没有两种取值，大于零为有，否则为没有")]
        Boolean = 1,
        [DbDescription("整数型，大于零的整数越大表示权限越高，否则表示没有权限")]
        Number = 2
    }

    
    public class PermissionDefinition : DomainEntityBase<Guid, Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public PermissionValueType ValueType { get; set; }
    }
}
