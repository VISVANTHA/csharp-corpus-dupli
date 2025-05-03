using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderKit
{
    /// <summary>Ref returns came two language versions ago; choosing WHICH slot to
    /// return a reference to, in an expression, arrives with this one.</summary>
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

        /// <summary>A conditional ref expression: the ternary yields a reference,
        /// not a value, so the increment lands in the chosen slot.</summary>
        public void BumpEither(bool first)
        {
            ref int slot = ref (first ? ref _counters[0] : ref _counters[1]);
            slot++;
        }

        public int CounterValue(int index) => _counters[index];

        public bool TryCount(string prefix, out int matches)
        {
            matches = default;
            foreach (string entry in _entries)
            {
                if (entry.StartsWith(prefix, StringComparison.Ordinal)) matches++;
            }
            return matches > 0;
        }

        /// <summary>Inferred tuple element names, read back by the locals' names.</summary>
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
