using CoreDX.Domain.Model.Entity;
using CoreDX.EntityFrameworkCore.Extensions.DataAnnotations;
using System;

namespace TestEntity
{
    public class TestEntity1 : DomainEntityBase<int, int>
    {
    }

    public class TestTreeEntity1 : DomainTreeEntityBase<int, TestTreeEntity1, int, TestUser>
    {
    }

    public class TestUser : DomainEntityBase<int>
    {
        [DbDescription("用户名")]
        public string Name { get; set; }
    }
}
