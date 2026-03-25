using System;

namespace Nintenlord.RandomDistributions
{
    public sealed class UniformDistribution : IDistribution<double>
    {
        private readonly double start;
        private readonly double length;
        private readonly Random random;

        private UniformDistribution(Random random, double start, double length)
        {
            this.start = start;
            this.length = length;
            this.random = random;
        }

        public double NextValue() => start + random.NextDouble() * length;

        public static UniformDistribution FromStartEnd(Random random, double start, double end) => new(random, start, end - start);

        public static UniformDistribution FromStartLength(Random random, double start, double length) => new(random, start, length);
    }
}
