using Nintenlord.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nintenlord.Graph.PathFinding
{
    public static class Dijkstra_algorithm
    {
        public static List<TNode> GetArea<TNode>(TNode toStartFrom, IWeighedGraph<TNode> map,
            IEqualityComparer<TNode> nodeComparer, int movementLimit)
        {
            IPriorityQueue<int, TNode> open = new SkipListPriorityQueue<int, TNode>(10);
            var closed = new HashSet<TNode>(nodeComparer);
            var costs = map.GetTempCostCollection();

            costs[toStartFrom] = 0;
            open.Enqueue(toStartFrom, 0);

            while (open.Count > 0)
            {
                int cost;
                var node = open.Dequeue(out cost);
                closed.Add(node);

                if (cost < movementLimit)
                {
                    foreach (var neighbour in map.GetNeighbours(node))
                    {
                        var newCost = cost + map.GetMovementCost(node, neighbour);
                        int oldCost;
                        if (!costs.TryGetValue(neighbour, out oldCost))
                        {
                            open.Enqueue(neighbour, newCost);
                            costs[neighbour] = newCost;
                        }
                        else if (newCost < oldCost)
                        {
                            open.Remove(neighbour, oldCost);
                            open.Enqueue(neighbour, newCost);
                            costs[neighbour] = newCost;
                        }
                    }
                }
            }

            var result = new List<TNode>(closed.Count);
            result.AddRange(closed.Where(node => costs[node] < movementLimit));
            costs.Release();
            return result;
        }

        public static int GetCost<TNode>(TNode toStartFrom, TNode toEnd,
            IWeighedGraph<TNode> map, IEqualityComparer<TNode> nodeComparer)
        {
            IPriorityQueue<int, TNode> open = new SkipListPriorityQueue<int, TNode>(10);
            var closed = new HashSet<TNode>(nodeComparer);
            var costs = map.GetTempCostCollection();

            costs[toStartFrom] = 0;
            open.Enqueue(toStartFrom, 0);

            while (open.Count > 0)
            {
                int cost;
                var node = open.Dequeue(out cost);
                closed.Add(node);
                int endCost;
                if (!costs.TryGetValue(toEnd, out endCost) || cost < endCost)
                {
                    foreach (var neighbour in map.GetNeighbours(node))
                    {
                        var newCost = cost + map.GetMovementCost(node, neighbour);
                        int oldCost;
                        if (!costs.TryGetValue(neighbour, out oldCost))
                        {
                            open.Enqueue(neighbour, newCost);
                            costs[neighbour] = newCost;
                        }
                        else if (newCost < oldCost)
                        {
                            open.Remove(neighbour, oldCost);
                            open.Enqueue(neighbour, newCost);
                            costs[neighbour] = newCost;
                        }//http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
                    }
                }
            }

            return closed.Contains(toEnd) ? costs[toEnd] : int.MinValue;
        }

        public static IDictionary<TNode, int> GetCosts<TNode>(TNode toStartFrom,
            IWeighedGraph<TNode> map, IEqualityComparer<TNode> nodeComparer)
        {
            IPriorityQueue<int, TNode> open = new SkipListPriorityQueue<int, TNode>(10);
            var closed = new HashSet<TNode>(nodeComparer);
            IDictionary<TNode, int> costs = new Dictionary<TNode, int>(nodeComparer);

            costs[toStartFrom] = 0;
            open.Enqueue(toStartFrom, 0);

            while (open.Count > 0)
            {
                int cost;
                var node = open.Dequeue(out cost);
                closed.Add(node);

                foreach (var neighbour in map.GetNeighbours(node))
                {
                    var newCost = cost + map.GetMovementCost(node, neighbour);
                    int oldCost;
                    if (!costs.TryGetValue(neighbour, out oldCost))
                    {
                        open.Enqueue(neighbour, newCost);
                        costs[neighbour] = newCost;
                    }
                    else if (newCost < oldCost)
                    {
                        open.Remove(neighbour, oldCost);
                        open.Enqueue(neighbour, newCost);
                        costs[neighbour] = newCost;
                    }//http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
                }
            }
            return costs;
        }

        public static HashSet<TNode> GetConnectedNodes<TNode>(TNode toStartFrom,
            IGraph<TNode> map, IEqualityComparer<TNode> nodeComparer)
        {
            var open = new LinkedList<TNode>();
            var closed = new HashSet<TNode>(nodeComparer);

            open.AddLast(toStartFrom);

            while (open.Count > 0)
            {
                var node = open.First.Value;
                closed.Add(node);

                foreach (var neighbour in map.GetNeighbours(node))
                {
                    if (!closed.Contains(neighbour))
                    {
                        open.AddLast(neighbour);
                    }
                }
            }
            return closed;
        }
    }
}
