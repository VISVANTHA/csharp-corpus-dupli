// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderKit
{
    /// <summary>
    /// Caller-info attributes arrived with the compiler the family below defaults
    /// to; string interpolation and nameof arrive with this one.
    /// </summary>
    public sealed class Diagnostics
    {
        private readonly IList<string> _entries = new List<string>();

        public void Note(string message, [CallerMemberName] string member = null)
        {
            _entries.Add($"[{member ?? "?"}] {message}");
        }

        public void NoteField(object value)
        {
            _entries.Add($"{nameof(Diagnostics)}.{nameof(NoteField)}={value}");
        }

        public IEnumerable<string> Entries() => _entries;

        public int Count() => _entries.Count;
    }
}
