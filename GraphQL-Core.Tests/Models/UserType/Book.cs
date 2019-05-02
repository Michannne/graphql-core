using GraphQL_Core.Tests.Models.Enum;
using GraphQL_Core.Tests.Models.Interfaces;
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

    public class Book_WithDateTime : Book
    {
        public DateTime PublishDate { get; set; }
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

    public class Book_WithVirtualAuthor : Book
    {
        public virtual Author Author { get; set; }
    }

    public class Book_WithVirtualAuthorAndId : Book
    {
        public long AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }

    public class Book_WithTwoAuthors : Book
    {
        public virtual Author AuthorA { get; set; }
        public virtual Author AuthorB { get; set; }
    }

    public class Book_WithDuplicateBaseTypes : Book
    {
        public long Duplicate_BookId { get; set; }
        public string Duplicate_Name { get; set; }
        public int Duplicate_Number { get; set; }
        public bool Duplicate_IsAvailable { get; set; }
        public float Duplicate_Rating { get; set; }
        public double Duplicate_Cost { get; set; }
    }

    public class Book_WithCommonInterface : Book, ICommonInterface
    {
        public int DumbField { get; set; }
    }

    public class Book_WithSameInterfaceAsProperty : Book, ICommonInterface
    {
        public Author_WithCommonInterface Author { get; set; }
    }

    public class Book_WithSameInterfaceAsProperty_WithVirtualProperty : Book, ICommonInterface
    {
        public virtual Author_WithCommonInterface Author { get; set; }
    }

    public class Book_WithConstructor : Book
    {
        public Book_WithConstructor() { }
    }
}
