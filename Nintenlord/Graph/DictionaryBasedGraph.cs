// -----------------------------------------------------------------------
// <copyright file="DictionaryBasedGraph.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Nintenlord.Graph
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public sealed class DictionaryBasedGraph<TNode> : IGraph<TNode>
    {
        private readonly IDictionary<TNode, IEnumerable<TNode>> neighbours;

        public DictionaryBasedGraph(IDictionary<TNode, IEnumerable<TNode>> neighbours)
        {
            this.neighbours = neighbours;
        }

        #region IGraph<T> Members

        public int NodeCount => neighbours.Count;

        public IEnumerable<TNode> GetNeighbours(TNode node) => neighbours[node];

        public bool IsEdge(TNode node1, TNode node2) => neighbours[node1].Contains(node2);

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<TNode> GetEnumerator() => neighbours.Keys.GetEnumerator();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion
    }
}
