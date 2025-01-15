using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderKit
{
    /// <summary>
    /// Caller-info attributes arrived with the compiler this family defaults to.
    /// A second, softer language marker alongside the async lock.
    /// </summary>
    public sealed class Diagnostics
    {
        private readonly IList<string> _entries;

        public Diagnostics()
        {
            _entries = new List<string>();
        }

        public void Note(string message, [CallerMemberName] string member = null)
        {
            _entries.Add(string.Format("[{0}] {1}", member == null ? "?" : member, message));
        }

        public IEnumerable<string> Entries()
        {
            return _entries;
        }

        public int Count()
        {
            return _entries.Count;
        }
    }
}
