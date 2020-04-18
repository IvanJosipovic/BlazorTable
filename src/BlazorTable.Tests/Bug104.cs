using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace BlazorTable.Tests
{
    public class Bug104
    {
        [Fact]
        public void SingleChild()
        {
            Expression<Func<Parent, bool>> query1 = x => x.Child.Name != null;

            Expression<Func<Parent, bool>> query1fixed = x => x.Child != null
                                                        && x.Child.Name != null;

            AddNullChecks(query1).ShouldBeSameAs(query1fixed);
        }

        [Fact]
        public void GrandChildChild()
        {
            Expression<Func<Parent, bool>> query1 = x => x.Child.GrandChild.Name != null;

            Expression<Func<Parent, bool>> query1fixed = x => x.Child != null
                                                            && x.Child.GrandChild != null 
                                                            && x.Child.GrandChild.Name != null;

            AddNullChecks(query1).ShouldBeSameAs(query1fixed);
        }

        private static Expression<Func<Parent, bool>> AddNullChecks(Expression<Func<Parent, bool>> expression)
        {
            //Todo logic


            return expression;
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
