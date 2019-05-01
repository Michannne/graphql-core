using GraphQL_Core.Tests.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL_Core.Tests.Models
{
    /// <summary>
    /// Contains data types supported by GraphQL-dotnet
    /// </summary>
    public class Author
    {
        public long AuthorId { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public bool IsAlive { get; set; }
        public float Rating { get; set; }
        public double NetWorth { get; set; }
    }

    public struct AuthorDescription
    {
        public string History { get; set; }
    }

    /// <summary>
    /// Extends the Author class by adding all C# value types
    /// <para>
    /// <see cref="!:https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/value-types"></see>
    /// </para>
    /// </summary>
    public class Author_WithValueTypes : Author
    {
        public Guid Identifier { get; set; }
        public char Gender { get; set; }
        public decimal Dewey { get; set; }
        public byte BooksWritten { get; set; }
        public sbyte Modifier { get; set; }
        public short ChildrenCount { get; set; }
        public uint Age { get; set; }
        public ulong PagesWritten { get; set; }
        public ushort ParentCount { get; set; }
    }

    public class Author_WithValueTypesAndStruct : Author_WithValueTypes
    {
        public AuthorDescription Info { get; set; }
    }

    public class Author_WithEnumTypes : Author
    {
        public AuthorType Type { get; set; }
    }
}
