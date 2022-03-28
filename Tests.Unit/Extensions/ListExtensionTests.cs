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
        public void PowerSets_Should_Create_List_Off_All_Combinations()
        {
            var list = new List<int> { 10, 11, 12 };
            var powerList = list.PowerSets().ToList();

            powerList.Should().NotBeNull();
            powerList.Count.Should().Be(8);

            powerList[0].Should().BeEmpty();

            powerList[1].ToList()[0].Should().Be(list[0]);
            powerList[2].ToList()[0].Should().Be(list[1]);

            powerList[3].ToList()[0].Should().Be(list[0]);
            powerList[3].ToList()[1].Should().Be(list[1]);

            powerList[4].ToList()[0].Should().Be(list[2]);

            powerList[5].ToList()[0].Should().Be(list[0]);
            powerList[5].ToList()[1].Should().Be(list[2]);

            powerList[6].ToList()[0].Should().Be(list[1]);
            powerList[6].ToList()[1].Should().Be(list[2]);

            powerList[7].ToList()[0].Should().Be(list[0]);
            powerList[7].ToList()[1].Should().Be(list[1]);
            powerList[7].ToList()[2].Should().Be(list[2]);
        }

        [Fact]
        public void ConvertToBsonArray_Should_Create_BsonArray_From_Strings()
        {
            var list = new List<string> { "10", "11", "12" };
            var bsonArray = list.ConvertToBsonArray().ToList();

            bsonArray.Should().NotContainNulls();
            bsonArray.Count.Should().Be(3);

            bsonArray[0].AsString.Should().Be(list[0]);
            bsonArray[1].AsString.Should().Be(list[1]);
            bsonArray[2].AsString.Should().Be(list[2]);
        }

        [Fact]
        public void ConvertToBsonArray_Should_Create_BsonArray_From_Int()
        {
            var list = new List<int> { 10, 11, 12 };
            var bsonArray = list.ConvertToBsonArray().ToList();

            bsonArray.Should().NotContainNulls();
            bsonArray.Count.Should().Be(3);

            bsonArray[0].AsInt32.Should().Be(list[0]);
            bsonArray[1].AsInt32.Should().Be(list[1]);
            bsonArray[2].AsInt32.Should().Be(list[2]);
        }

        [Fact]
        public void ConvertToBsonArray_Should_Create_BsonArray_From_LibraryType()
        {
            var list = new List<LibraryType> { LibraryType.TvShow, LibraryType.Books, LibraryType.BoxSets };
            var bsonArray = list.ConvertToBsonArray().ToList();

            bsonArray.Should().NotContainNulls();
            bsonArray.Count.Should().Be(3);

            bsonArray[0].AsString.Should().Be(list[0].ToString());
            bsonArray[1].AsString.Should().Be(list[1].ToString());
            bsonArray[2].AsString.Should().Be(list[2].ToString());
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
    }
}
