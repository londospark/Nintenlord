// -----------------------------------------------------------------------
// <copyright file="UpdateOnDemand.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Nintenlord.Utility
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public sealed class UpdateOnDemand<T>
    {
        private T item;
        private bool update;
        private readonly Func<T> valueFactory;
        private readonly object lockObject = new object();

        public T Value
        {
            get
            {
                if (update)
                {
                    lock (lockObject)
                    {
                        if (update)
                        {
                            item = valueFactory();
                            update = false;
                        }
                    }
                }
                return item;
            }
        }

        public UpdateOnDemand(Func<T> valueFactory)
        {
            this.valueFactory = valueFactory;
            update = true;
            item = default(T);
        }

        public UpdateOnDemand(Func<T> valueFactory, T startValue)
        {
            this.valueFactory = valueFactory;
            update = false;
            item = startValue;
        }

        public void NeedsUpdate() => update = true;

        public static implicit operator UpdateOnDemand<T>(T item) => new(() => item, item);
    }
}
