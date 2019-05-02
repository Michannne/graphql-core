using GraphQL_Core.Tests.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL_Core.Tests.Models
{
    /// <summary>
    /// Contains data types supported by GraphQL-dotnet
    /// </summary>
    public class Book
    {
        public long BookId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
        public float Rating { get; set; }
        public double Cost { get; set; }
    }

    public struct BookDescription
    {
        public string Blurb { get; set; }
    }

    /// <summary>
    /// Extends the Book class by adding all C# value types
    /// <para>
    /// <see cref="!:https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/value-types"></see>
    /// </para>
    /// </summary>
    public class Book_WithValueTypes : Book
    {
        public Guid Identifier { get; set; }
        public char Category { get; set; }
        public decimal Dewey { get; set; }
        public byte OwnerCount { get; set; }
        public sbyte Modifier { get; set; }
        public short Value { get; set; }
        public uint Copies { get; set; }
        public ulong Pages { get; set; }
        public ushort AuthorCount { get; set; }
    }

    public class Book_WithValueTypesAndStruct : Book_WithValueTypes
    {
        public BookDescription Info { get; set; }
    }

    public class Book_WithEnumerables : Book
    {
        public List<int> OwnerIds { get; set; }
        public IList<float> SiteRatings { get; set; }
        public IQueryable<double> AvgSalesPerMonth { get; set; }
        public IEnumerable<bool> AreFavorited { get; set; }
    }

    public class Book_WithAdvancedEnumerables : Book_WithEnumerables
    {
        public List<IEnumerable<IQueryable<int>>> Fans { get; set; }
    }

    public class Book_WithEnumTypes : Book
    {
        public BookType Type { get; set; }
    }

    public class Book_WithAuthor : Book
    {
        public Author Author { get; set; }
    }
}
