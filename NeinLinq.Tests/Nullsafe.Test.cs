﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.Nullsafe
{
    public class Test
    {
        private readonly IQueryable<Dummy> data;

        public Test()
        {
            data = new[]
            {
                new Dummy
                {
                    SomeInteger = 7,
                    OneDay = new DateTime(1977, 05, 25),
                    SomeOther = new Dummy { SomeInteger = 42 }
                },
                new Dummy
                {
                    SomeInteger = 1138,
                    OneDay = new DateTime(1980, 05, 21),
                    SomeOthers = new[]
                    {
                        new Dummy { OneDay = new DateTime(2000, 3, 1) },
                        new Dummy { OneDay = new DateTime(2000, 6, 1) }
                    }
                },
                new Dummy
                {
                    SomeInteger = 123456,
                    OneDay = new DateTime(1983, 05, 25),
                    MoreOthers = new[]
                    {
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 5) } },
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 8) } }
                    }
                },
                new Dummy
                {
                    SomeInteger = 654321,
                    OneDay = new DateTime(2015, 12, 18),
                    EvenLotMoreOthers = new HashSet<Dummy>()
                    {
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 4) } },
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 7) } }
                    }
                },
                null
            }
            .AsQueryable();
        }

        [Fact]
        public void OrdinaryQueryShouldFail()
        {
            Assert.Throws<NullReferenceException>(() =>
                Query(data).ToList());
        }

        [Fact]
        public void NullsafeQueryShouldSucceed()
        {
            var result = Query(data.ToNullsafe()).ToList();

            Assert.Equal(5, result.Count);

            AssertDummy(result[0]);
            AssertDummy(result[1], year: 1977, integer: 42);
            AssertDummy(result[2], year: 1980, other: new[] { 3, 6 });
            AssertDummy(result[3], year: 1983, more: new[] { 5, 8 });
            AssertDummy(result[4], year: 2015, lot: new[] { 4, 7 });
        }

        private static IQueryable<DummyView> Query(IQueryable<Dummy> data)
        {
            return from a in data
                   orderby a.SomeInteger
                   select new DummyView
                   {
                       Year = a.OneDay.Year,
                       Integer = a.SomeOther.SomeInteger,
                       Other = from b in a.SomeOthers
                               select b.OneDay.Month,
                       More = from c in a.MoreOthers
                              select c.SomeOther.OneDay.Day,
                       Lot = from d in a.EvenLotMoreOthers
                             select d.SomeOther.OneDay.Day
                   };
        }

        private static void AssertDummy(DummyView dummy,
                                        int year = 0,
                                        int integer = 0,
                                        int[] other = null,
                                        int[] more = null,
                                        int[] lot = null)
        {
            Assert.Equal(year, dummy.Year);
            Assert.Equal(integer, dummy.Integer);
            Assert.Equal(other ?? new int[0], dummy.Other);
            Assert.Equal(more ?? new int[0], dummy.More);
            Assert.Equal(lot ?? new int[0], dummy.Lot);
        }
    }
}