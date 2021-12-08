using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Concurrency;

namespace BookLibrary.Models
{
    /// <summary>
    /// Data about a book.
    /// This class is immutable.
    /// Operations on this class always return a new copy.
    /// </summary>
    [Serializable]
    [Immutable]
    public class Book : IEquatable<Book>
    {
        public Immutable<Guid> Guid { get; }
        public Immutable<string> Title { get; }
        public List<string> Pages { get; }

        public bool Equals(Book? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Guid.Value.Equals(other.Guid.Value) &&
                   Title.Value.Equals(other.Title.Value) &&
                   Pages.SequenceEqual(other.Pages);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Book) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, Title, Pages);
        }

        public static bool operator ==(Book? left, Book? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Book? left, Book? right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Constructor for a book.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="pages"></param>
        public Book(Immutable<Guid> guid, List<string> pages, Immutable<string> title)
        {
            Guid = guid;
            Pages = pages;
            Title = title;
        }

        /// <summary>
        /// Creates an immutable copy of the current book with the given book pages.
        /// </summary>
        /// <param name="newPages"></param>
        /// <returns></returns>
        public Book WithNewPages(List<string> newPages) =>
            new Book(Guid, newPages, Title);
    }
}