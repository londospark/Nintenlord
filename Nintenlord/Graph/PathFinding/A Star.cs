using Nintenlord.Collections;
using System.Collections.Generic;

namespace Nintenlord.Graph.PathFinding
{
    //http://www.policyalmanac.org/games/aStarTutorial.htm
    //http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html#S1

    public static class A_Star
    {
        public static List<TNode> GetPath<TNode>(TNode start, TNode goal,
            IWeighedGraph<TNode> map, IHeurestic<TNode> heurestics) =>
            GetPath(start, goal, map, heurestics, EqualityComparer<TNode>.Default);

        public static List<TNode> GetPath<TNode>(TNode start, TNode goal,
            IWeighedGraph<TNode> map, IHeurestic<TNode> heurestics, IEqualityComparer<TNode> nodeComparer)
        {
            IPriorityQueue<int, TNode> open =
                new SkipListPriorityQueue<int, TNode>(10);
            var closed = new HashSet<TNode>(nodeComparer);
            var gCosts = map.GetTempCostCollection();
            var hCosts = map.GetTempCostCollection();
            IDictionary<TNode, TNode> parents = new Dictionary<TNode, TNode>(nodeComparer);

            open.Enqueue(start, 0);
            gCosts[start] = 0;
            hCosts[start] = 0;
            while (open.Count > 0 && !nodeComparer.Equals(open.Peek(), goal))
            {
                var current = open.Dequeue();
                closed.Add(current);

                foreach (var neighbour in map.GetNeighbours(current))
                {
                    var gCost = gCosts[current] + map.GetMovementCost(current, neighbour);
                    int oldGcost;
                    if (gCosts.TryGetValue(neighbour, out oldGcost) && gCost < oldGcost)
                    {//If we found a better route to neighbour 
                        var hCost = hCosts[neighbour];
                        open.Remove(neighbour, oldGcost + hCost);
                        closed.Remove(neighbour);

                        gCosts[neighbour] = gCost;
                        open.Enqueue(neighbour, gCost + hCost);
                        parents[neighbour] = current;

                    }
                    else if (!closed.Contains(neighbour) && !open.Contains(neighbour))
                    {//If we got here the first time
                        var hCost = heurestics.GetCostEstimate(neighbour);
                        hCosts[neighbour] = hCost;

                        gCosts[neighbour] = gCost;
                        open.Enqueue(neighbour, gCost + hCost);
                        parents[neighbour] = current;
                    }
                }
            }

            gCosts.Release();
            hCosts.Release();

            if (open.Count == 0)//No path exists
            {
                return new List<TNode>();
            }

            var last = open.Dequeue();
            open.Clear();

            var result = new List<TNode>();
            while (parents.ContainsKey(last))
            {
                result.Add(last);
                last = parents[last];
            }

            result.Add(last);
            result.Reverse();
            return result;
        }
    }
}
