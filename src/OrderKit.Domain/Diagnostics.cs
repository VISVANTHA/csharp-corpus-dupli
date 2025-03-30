// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderKit
{
    /// <summary>
    /// Caller-info attributes came three language versions ago, interpolation two,
    /// ref returns one; the bare `default` literal arrives with this one.
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

        /// <summary>An out variable, and a `default` whose type comes from the
        /// declaration rather than being restated.</summary>
        public bool TryCount(string prefix, out int matches)
        {
            matches = default;
            foreach (string entry in _entries)
            {
                if (entry.StartsWith(prefix, StringComparison.Ordinal))
                {
                    matches++;
                }
            }
            return matches > 0;
        }

        /// <summary>Inferred tuple element names over the counters, read back by
        /// the names the locals happen to have.</summary>
        public string Totals()
        {
            int total = default;
            int distinct = default;
            foreach (int c in _counters)
            {
                total += c;
                if (c > 0) distinct++;
            }
            var totals = (total, distinct);
            return $"{totals.total}/{totals.distinct}";
        }

        public IEnumerable<string> Entries() => _entries;

        public int Count() => _entries.Count;
    }
}
