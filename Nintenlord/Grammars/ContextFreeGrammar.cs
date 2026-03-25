// -----------------------------------------------------------------------
// <copyright file="ContextFreeGrammar.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Nintenlord.Grammars
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public sealed class ContextFreeGrammar<T>
    {
        private readonly IDictionary<T, T[][]> productions = default!;
        private readonly T startingSymbol = default!;
        private readonly T[] variables = default!;
        private readonly T[] terminals = default!;

        public T[][] this[T variable] => productions[variable];

        public T StartingSymbol => startingSymbol;

        public IEnumerable<T> Variables => variables;

        public IEnumerable<T> Terminals => terminals;

        public T[] DeriveRandom(Random random)
        {
            var word = new List<T>(20) { startingSymbol };
            while (true)
            {
                int i;
                for (i = 0; i < word.Count; i++)
                {
                    if (variables.Contains(word[i]))
                        break;
                }

                if (i == word.Count)
                    break;

                var rules = productions[word[i]];
                var ruleToUse = rules[random.Next(rules.Length)];

                word.RemoveAt(i);
                word.InsertRange(i, ruleToUse);
            }
            return word.ToArray();
        }
    }
}
