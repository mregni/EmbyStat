using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class ListExtensionTests
    {

        [Fact]
        public void AreListEqual_Should_Return_True_If_List_Are_equal()
        {
            var listOne = new List<int> {10, 11, 12};
            var listTwo = new List<int> {10, 11, 12};

            var equal = listOne.AreListEqual(listTwo);
            equal.Should().BeTrue();
        }

        [Fact]
        public void AreListEqual_Should_Return_False_If_List_Not_Same_Size()
        {
            var listOne = new List<int> { 10, 11, 12 };
            var listTwo = new List<int> { 10, 11, 12, 13 };

            var equal = listOne.AreListEqual(listTwo);
            equal.Should().BeFalse();
        }

        [Fact]
        public void AreListEqual_Should_Return_False_If_List_Not_Same_Content()
        {
            var listOne = new List<int> { 10, 11, 12 };
            var listTwo = new List<int> { 10, 11, 13 };

            var equal = listOne.AreListEqual(listTwo);
            equal.Should().BeFalse();
        }

        [Fact]
        public void MaybeAdd_Should_Add_Item_To_List()
        {
            var list = new List<int?> {1};
            list.AddIfNotNull(2);

            list.Count.Should().Be(2);
            list[0].Should().Be(1);
            list[1].Should().Be(2);
        }

        [Fact]
        public void MaybeAdd_Should_Not_Add_Null_Item_To_List()
        {
            var list = new List<int?> { 1 };
            list.AddIfNotNull(null);

            list.Count.Should().Be(1);
            list[0].Should().Be(1);
        }

        [Fact]
        public void AnyNotNull_Should_Return_True()
        {
            var list = new List<int?>() {1};
            list.AnyNotNull().Should().Be(true);
        }
        
        [Fact]
        public void AnyNotNull_Should_Return_False_Because_Empty()
        {
            var list = new List<int?>();
            list.AnyNotNull().Should().Be(false);
        }
        
        [Fact]
        public void AnyNotNull_Should_Return_False_Because_Null()
        {
            ((List<int?>) null).AnyNotNull().Should().Be(false);
        }
    }
}
