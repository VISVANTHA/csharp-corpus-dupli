using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderKit
{
    /// <summary>
    /// Caller-info attributes came two language versions ago, interpolation one;
    /// ref locals and returns arrive with this one.
    /// </summary>
    public sealed class Diagnostics
    {
        private readonly IList<string> _entries = new List<string>();
        private readonly int[] _counters = new int[4];

        public void Note(string message, [CallerMemberName] string member = null)
        {
            _entries.Add($"[{member ?? "?"}] {message}");
        }

        public void NoteField(object value)
        {
            _entries.Add($"{nameof(Diagnostics)}.{nameof(NoteField)}={value}");
        }

        /// <summary>A ref return: the caller gets the slot, not a copy.</summary>
        public ref int CounterAt(int index)
        {
            if (index < 0 || index >= _counters.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return ref _counters[index];
        }

        public void Bump(int index)
        {
            ref int slot = ref CounterAt(index);
            slot++;
        }

        public int CounterValue(int index) => _counters[index];

        /// <summary>An out variable in its own argument list.</summary>
        public bool TryCount(string prefix, out int matches)
        {
            matches = 0;
            foreach (string entry in _entries)
            {
                if (entry.StartsWith(prefix, StringComparison.Ordinal))
                {
                    matches++;
                }
            }
            return matches > 0;
        }

        public IEnumerable<string> Entries() => _entries;

        public int Count() => _entries.Count;
    }
}
