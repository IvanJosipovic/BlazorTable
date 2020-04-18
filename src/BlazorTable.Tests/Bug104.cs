using Shouldly;
using System;
using System.Linq.Expressions;
using Xunit;

namespace BlazorTable.Tests
{
    public class Bug104
    {
        [Fact]
        public void AddToExisting()
        {
            Expression<Func<Parent, bool>> query = x => ((x.Child.Name != null) && (x.Child.Name == "bob"));

            Expression<Func<Parent, bool>> queryfixed = x => ((x.Child != null) &&((x.Child.Name != null) && (x.Child.Name == "bob")));

            query.AddNullChecks().ToString().ShouldBe(queryfixed.ToString());
        }


        [Fact]
        public void NoParent()
        {
            Expression<Func<Parent, bool>> query = x => x.Name != null;

            Expression<Func<Parent, bool>> queryfixed = x => x.Name != null;

            query.AddNullChecks().ToString().ShouldBe(queryfixed.ToString());
        }

        [Fact]
        public void SingleParent()
        {
            Expression<Func<Parent, bool>> query = x => x.Child.Name != null;

            Expression<Func<Parent, bool>> queryfixed = x => x.Child != null
                                                        && x.Child.Name != null;

            query.AddNullChecks().ToString().ShouldBe(queryfixed.ToString());
        }

        [Fact]
        public void MultiParent()
        {
            Expression<Func<Parent, bool>> query1 = x => x.Child.GrandChild.Name != null;

            Expression<Func<Parent, bool>> query1fixed = x => ((x.Child != null)
                                                            && ((x.Child.GrandChild != null) && (x.Child.GrandChild.Name != null)));
            
            query1.AddNullChecks().ToString().ShouldBe("x => ((x.Child != null) AndAlso ((x.Child.GrandChild != null) AndAlso (x.Child.GrandChild.Name != null)))");
        }

        private class Parent
        {
            public string Name { get; set; }

            public Child Child { get; set; }
        }

        private class Child
        {
            public string Name { get; set; }

            public GrandChild GrandChild { get; set; }
        }

        private class GrandChild
        {
            public string Name { get; set; }
        }
    }
}
