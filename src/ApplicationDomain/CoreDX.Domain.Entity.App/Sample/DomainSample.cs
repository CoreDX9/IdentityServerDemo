﻿using System.ComponentModel.DataAnnotations.Schema;
using CoreDX.Domain.Model.Entity;
using CoreDX.EntityFrameworkCore.DataAnnotations;

namespace CoreDX.Domain.Entity.App.Sample
{
    [DbDescription("示例实体")]
    public class Domain : DomainEntityBase<int, int>
    {
        [Column("SC")]
        [DbDescription("示例列")]
        public virtual string SampleColumn { get; set; }

        [DbDescription("示例复杂实体属性（没有作用）")]
        public virtual ComplexEntityProperty ComplexProperty { get; set; }
    }

    [DbDescription("示例复杂实体属性类（分表存储时会成为表说明，内部属性存储到主表时没有作用）")]
    //[Owned]//加入此特性会导致对Microsoft.EntityFrameworkCore的依赖，可以在OnModelCreating中进行配置
    public class ComplexEntityProperty
    {
        [DbDescription("示例复杂实体属性列1")]
        public string C1 { get; set; }

        [DbDescription("示例复杂实体属性列2")]
        public string C2 { get; set; }

        [DbDescription("示例复杂实体属性2（没有作用）")]
        public virtual ComplexEntityProperty2 ComplexProperty2 { get; set; }
    }

    [DbDescription("示例复杂实体属性类2（分表存储时会成为表说明，内部属性存储到主表时没有作用，上级表（ComplexEntityProperty的属性所在的表）分表存储时内部属性存储到上级表时会覆盖上级表的表说明）")]
    public class ComplexEntityProperty2
    {
        [DbDescription("示例复杂实体属性列3")]
        public string C3 { get; set; }

        [DbDescription("示例复杂实体属性列4")]
        public string C4 { get; set; }
    }

    [DbDescription("示例树形实体")]
    public class TreeDomain : DomainTreeEntityBase<int, TreeDomain, int>
    {
        [DbDescription("示例列")]
        public virtual string SampleColumn { get; set; }

        public override int? CreatorId { get; set; }
        public override int? LastModifierId { get; set; }
    }

    public class TreeDomainView : DomainTreeEntityViewBase<int, TreeDomainView, int>
    {
        public virtual string SampleColumn { get; set; }
    }
}
