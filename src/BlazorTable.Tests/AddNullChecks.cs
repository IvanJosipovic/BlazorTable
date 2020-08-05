using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace BlazorTable.Tests
{
    public class AddNullChecks
    {
        [Fact]
        public void AddToSingle()
        {
            Expression<Func<Parent, object>> Field = x => x.Child;
            var exp = Field.Body.CreateNullChecks();
            exp.ToString().ShouldBe("(x.Child != null)");
        }

        [Fact]
        public void AddToMulti()
        {
            Expression<Func<Parent, object>> Field = x => x.Child.Name;
            var exp = Field.Body.CreateNullChecks();
            exp.ToString().ShouldBe("((x.Child != null) AndAlso (x.Child.Name != null))");
        }

        [Fact]
        public void AddToMulti2()
        {
            Expression<Func<Parent, object>> Field = x => x.Child.GrandChild.Name;
            var exp = Field.Body.CreateNullChecks();
            exp.ToString().ShouldBe("(((x.Child != null) AndAlso (x.Child.GrandChild != null)) AndAlso (x.Child.GrandChild.Name != null))");
        }

        [Fact]
        public void Skip()
        {
            Expression<Func<Parent, object>> Field = x => x.Child;
            var exp = Field.Body.CreateNullChecks(true);
            exp.ToString().ShouldBe("(x.Child != null)");
        }

        [Fact]
        public void Skip2()
        {
            Expression<Func<Parent, object>> Field = x => x.Child.Name;
            var exp = Field.Body.CreateNullChecks(true);
            exp.ToString().ShouldBe("(x.Child != null)");
        }

        [Fact]
        public void Skip3()
        {
            Expression<Func<Parent, object>> Field = x => x.Child.GrandChild.Name;
            var exp = Field.Body.CreateNullChecks(true);
            exp.ToString().ShouldBe("((x.Child != null) AndAlso (x.Child.GrandChild != null))");
        }

        [Fact]
        public void NonNullable()
        {
            Expression<Func<Parent, object>> Field = x => x.Enum;
            var exp = Field.Body.CreateNullChecks();
            exp.ToString().ShouldBe("(True == True)");
        }

        [Fact]
        public void NonNullable2()
        {
            Expression<Func<Parent, object>> Field = x => x.Child.Enum;
            var exp = Field.Body.CreateNullChecks();
            exp.ToString().ShouldBe("(x.Child != null)");
        }

        private class Parent
        {
            public Child Child { get; set; }

            public MyEnum Enum { get; set; }
        }

        private class Child
        {
            public string Name { get; set; }

            public GrandChild GrandChild { get; set; }

            public MyEnum Enum { get; set; }
        }

        private class GrandChild
        {
            public string Name { get; set; }
        }

        private enum MyEnum
        {
            One,
            Two,
            Three
        }
    }
}
