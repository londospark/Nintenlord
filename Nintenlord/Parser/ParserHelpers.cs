// -----------------------------------------------------------------------
// <copyright file="ParserHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Nintenlord.Parser
{
    using Nintenlord.Parser.ParserCombinators;
    using Nintenlord.Parser.ParserCombinators.BinaryParsers;
    using Nintenlord.Parser.ParserCombinators.UnaryParsers;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ParserHelpers
    {
        public static SequenceParser<T, TOut> ParseSequence<T, TOut>(IEnumerable<T> sequence) => new(sequence);

        public static FunctionParser<TIn, TOut> Parser<TIn, TOut>(
            this Func<Nintenlord.IO.Scanners.IScanner<TIn>, Tuple<TOut, Match<TIn>>> fun) =>
            new(fun);

        public static BetweenParser<TIn, TStart, TEnd, TOut> Between<TIn, TOut, TStart, TEnd>(
            this IParser<TIn, TOut> parser, IParser<TIn, TStart> start, IParser<TIn, TEnd> end) =>
            new(start, parser, end);

        public static ManyParser<TIn, TOut> Many<TIn, TOut>(this IParser<TIn, TOut> parser) => new(parser);

        public static Many1Parser<TIn, TOut> Many1<TIn, TOut>(this IParser<TIn, TOut> parser) => new(parser);

        public static ManyTillParser<TIn, TEnd, TOut> ManyTill<TIn, TEnd, TOut>(this IParser<TIn, TOut> results,
            IParser<TIn, TEnd> ender) =>
            new(results, ender);

        public static OrParser<TIn, TOut> Or<TIn, TOut>(this IParser<TIn, TOut> choise1, IParser<TIn, TOut> choise2) => new(choise1, choise2);

        public static ChoiseParser<TIn, TOut> Choise<TIn, TOut>(params IParser<TIn, TOut>[] parsers) => new(parsers);

        public static ChoiseParser<TIn, TOut> Choise<TIn, TOut>(this IEnumerable<IParser<TIn, TOut>> parsers) => new(parsers);

        public static TransformParser<TIn, TMiddle, TOut> Transform<TIn, TMiddle, TOut>(
            this IParser<TIn, TMiddle> parser, Converter<TMiddle, TOut> f) =>
            new(parser, f);

        public static SeparatedBy1Parser<TIn, TSeb, TOut> SepBy1<TIn, TSeb, TOut>(this IParser<TIn, TOut> parser,
            IParser<TIn, TSeb> separator) =>
            new(separator, parser);

        public static SeparatedByParser<TIn, TSeb, TOut> SepBy<TIn, TSeb, TOut>(this IParser<TIn, TOut> parser,
            IParser<TIn, TSeb> separator) =>
            new(separator, parser);

        public static SatisfyParser<T> Satisfy<T>(this Predicate<T> predicate) => new(predicate);

        public static OptionalParser<TIn, TOut> Optional<TIn, TOut>(this IParser<TIn, TOut> parser) => new(parser);

        public static OptionalParser<TIn, TOut> Optional<TIn, TOut>(this IParser<TIn, TOut> parser, TOut defaultVal) => new(parser, defaultVal);

        public static NameParser<TIn, TOut> Name<TIn, TOut>(this IParser<TIn, TOut> parser, string name) => new(parser, name);

        public static SafeFailureCheckerParser<TIn, TOut> AddCheck<TIn, TOut>(this IParser<TIn, TOut> parser) => new(parser);

        public static SafeFailureCheckerParser<TIn, TOut> AddCheck<TIn, TOut>(this IParser<TIn, TOut> parser, string text) => new(parser, text);

        public static CombineParser<TIn, TMiddle1, TMiddle2, TOut> Combine<TIn, TMiddle1, TMiddle2, TOut>(
            this IParser<TIn, TMiddle1> first, IParser<TIn, TMiddle2> second, Func<TMiddle1, TMiddle2, TOut> comb) =>
            new(first, second, comb);

        public static CombineParser<TIn, TMiddle1, TMiddle2, TMiddle3, TOut>
            Combine<TIn, TMiddle1, TMiddle2, TMiddle3, TOut>(
            this IParser<TIn, TMiddle1> first,
            IParser<TIn, TMiddle2> second,
            IParser<TIn, TMiddle3> third,
            Func<TMiddle1, TMiddle2, TMiddle3, TOut> comb) =>
            new(first, second, third, comb);

        public static CombineParser<TIn, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TOut>
            Combine<TIn, TMiddle1, TMiddle2, TMiddle3, TMiddle4, TOut>(
            this IParser<TIn, TMiddle1> first,
            IParser<TIn, TMiddle2> second,
            IParser<TIn, TMiddle3> third,
            IParser<TIn, TMiddle4> fourth,
            Func<TMiddle1, TMiddle2, TMiddle3, TMiddle4, TOut> comb) =>
            new(first, second, third, fourth, comb);

        public static LazyParser<TIn, TOut> Lazy<TIn, TOut>(this Func<IParser<TIn, TOut>> parserFactory) => new(new Lazy<IParser<TIn, TOut>>(parserFactory));
    }
}
