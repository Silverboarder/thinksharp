using System;
using System.Collections.Generic;
using System.Text;
using ThinkSharp.Common;

namespace ThinkSharp.PathFinding
{
    #region " Depth first search "

    public class Graph_SearchDFS : BaseGraphSearchAlgo
    {
        private enum NodeState { visited, unvisited, no_parent_assigned };

        // Records the indexes of all the nodes that are visited as the search progresses
        private List<int> m_Visited;

        //this holds the route taken to the target. Given a node index, the value
        //at that index is the node's parent. ie if the path to the target is
        //3-8-27, then m_Route[8] will hold 3 and m_Route[27] will hold 8.
        private List<int> m_Route;

        //As the search progresses, this will hold all the edges the algorithm has
        //examined. THIS IS NOT NECESSARY FOR THE SEARCH, IT IS HERE PURELY
        //TO PROVIDE THE USER WITH SOME VISUAL FEEDBACK
        private List<NavGraphEdge> m_SpanningTree;

        public Graph_SearchDFS(SparseGraph graph, int source, int target)
            : base(graph, source, target)
        {
            int numNodes = m_Graph.NumNodes();

            m_Visited = new List<int>(numNodes);
            m_Route = new List<int>(numNodes);

            for (int i = 0; i < numNodes; i++)
            {
                m_Visited.Add((int)NodeState.unvisited);
                m_Route.Add((int)NodeState.no_parent_assigned);
            }

            m_SpanningTree = new List<NavGraphEdge>();

            m_bFound = Search();
        }

        //returns a list containing all the edges the search has examined
        public override List<NavGraphEdge> GetSearchTree() { return m_SpanningTree; }

        public override bool Search()
        {
            //create a std stack of edges
            Stack<NavGraphEdge> stack = new Stack<NavGraphEdge>();

            //create a dummy edge and put on the stack
            NavGraphEdge Dummy = new NavGraphEdge(m_iSource, m_iSource, 0);

            stack.Push(Dummy);

            //while there are edges in the stack keep searching
            while (stack.Count > 0)
            {
                //grab and remove the next edge
                NavGraphEdge NextEdge = stack.Pop();

                //make a note of the parent of the node this edge points to
                m_Route[NextEdge.To] = NextEdge.From;

                //put it on the tree. (making sure the dummy edge is not placed on the tree)
                if (NextEdge != Dummy)
                {
                    m_SpanningTree.Add(NextEdge);
                }

                //and mark it visited
                m_Visited[NextEdge.To] = (int)NodeState.visited;

                //if the target has been found the method can return success
                if (NextEdge.To == m_iTarget)
                {
                    return true;
                }

                //push the edges leading from the node this edge points to onto
                //the stack (provided the edge does not point to a previously 
                //visited node)

                SparseGraph.EdgeIterator EdgeItr = new SparseGraph.EdgeIterator(m_Graph, NextEdge.To);

                while (EdgeItr.MoveNext())
                {
                    if (m_Visited[EdgeItr.Current.To] == (int)NodeState.unvisited)
                    {
                        stack.Push(EdgeItr.Current);
                    }

                }
            }

            //no path to target
            return false;
        }

        public override List<int> GetPathToTarget()
        {
            List<int> path = new List<int>();

            //just return an empty path if no path to target found or if
            //no target has been specified
            if (!m_bFound || m_iTarget < 0) return path;

            int nd = m_iTarget;

            path.Add(nd);

            while (nd != m_iSource)
            {
                nd = m_Route[nd];

                path.Insert(0, nd); // Adds an element to the beginning of a list.
            }

            return path;
        }

        public override double GetCostToTarget()
        {
            return 0;
        }
    }

    #endregion

    #region " Breadth first search "

    public class Graph_SearchBFS : BaseGraphSearchAlgo
    {
        private enum NodeState { visited, unvisited, no_parent_assigned };

        // Records the indexes of all the nodes that are visited as the search progresses
        private List<int> m_Visited;

        //this holds the route taken to the target. Given a node index, the value
        //at that index is the node's parent. ie if the path to the target is
        //3-8-27, then m_Route[8] will hold 3 and m_Route[27] will hold 8.
        private List<int> m_Route;

        //As the search progresses, this will hold all the edges the algorithm has
        //examined. THIS IS NOT NECESSARY FOR THE SEARCH, IT IS HERE PURELY
        //TO PROVIDE THE USER WITH SOME VISUAL FEEDBACK
        private List<NavGraphEdge> m_SpanningTree;

        public Graph_SearchBFS(SparseGraph graph, int source, int target)
            : base(graph, source, target)
        {
            int numNodes = m_Graph.NumNodes();

            m_Visited = new List<int>(numNodes);
            m_Route = new List<int>(numNodes);

            for (int i = 0; i < numNodes; i++)
            {
                m_Visited.Add((int)NodeState.unvisited);
                m_Route.Add((int)NodeState.no_parent_assigned);
            }

            m_SpanningTree = new List<NavGraphEdge>();

            m_bFound = Search();
        }

        //returns a list containing all the edges the search has examined
        public override List<NavGraphEdge> GetSearchTree() { return m_SpanningTree; }

        public override bool Search()
        {
            //create a std stack of edges
            Queue<NavGraphEdge> Q = new Queue<NavGraphEdge>();

            //create a dummy edge and put on the Queue
            NavGraphEdge Dummy = new NavGraphEdge(m_iSource, m_iSource, 0);

            Q.Enqueue(Dummy);

            //mark the source node as visited
            m_Visited[m_iSource] = (int)NodeState.visited;

            //while there are edges in the stack keep searching
            while (Q.Count > 0)
            {
                //grab and remove the next edge
                NavGraphEdge NextEdge = Q.Dequeue();

                //make a note of the parent of the node this edge points to
                m_Route[NextEdge.To] = NextEdge.From;

                //put it on the tree. (making sure the dummy edge is not placed on the tree)
                if (NextEdge != Dummy)
                {
                    m_SpanningTree.Add(NextEdge);
                }

                //if the target has been found the method can return success
                if (NextEdge.To == m_iTarget)
                {
                    return true;
                }

                //push the edges leading from the node this edge points to onto
                //the queue 

                SparseGraph.EdgeIterator EdgeItr = new SparseGraph.EdgeIterator(m_Graph, NextEdge.To);

                while (EdgeItr.MoveNext())
                {
                    if (m_Visited[EdgeItr.Current.To] == (int)NodeState.unvisited)
                    {
                        Q.Enqueue(EdgeItr.Current);

                        //and mark it visited
                        m_Visited[EdgeItr.Current.To] = (int)NodeState.visited;
                    }
                }
            }

            //no path to target
            return false;
        }

        public override List<int> GetPathToTarget()
        {
            List<int> path = new List<int>();

            //just return an empty path if no path to target found or if
            //no target has been specified
            if (!m_bFound || m_iTarget < 0) return path;

            int nd = m_iTarget;

            path.Add(nd);

            while (nd != m_iSource)
            {
                nd = m_Route[nd];

                path.Insert(0, nd); // Adds an element to the beginning of a list.
            }

            return path;
        }

        public override double GetCostToTarget()
        {
            return 0;
        }
    }

    #endregion

    #region " Dijkstra's algorithm "

    public class Graph_SearchDijkstra : BaseGraphSearchAlgo
    {
        //this vector contains the edges that comprise the shortest path tree -
        //a directed subtree of the graph that encapsulates the best paths from 
        //every node on the SPT to the source node.
        private List<NavGraphEdge> m_ShortestPathTree;

        //this is indexed into by node index and holds the total cost of the best
        //path found so far to the given node. For example, m_CostToThisNode[5]
        //will hold the total cost of all the edges that comprise the best path
        //to node 5, found so far in the search (if node 5 is present and has 
        //been visited)
        private List<double> m_CostToThisNode;

        //this is an indexed (by node) list of 'parent' edges leading to nodes 
        //connected to the SPT but that have not been added to the SPT yet. This is
        //a little like the stack or queue used in BST and DST searches.
        private List<NavGraphEdge> m_SearchFrontier;

        public Graph_SearchDijkstra(SparseGraph graph, int source, int target)
            : base(graph, source, target)
        {
            int numNodes = m_Graph.NumNodes();

            m_ShortestPathTree = new List<NavGraphEdge>(numNodes);
            m_SearchFrontier = new List<NavGraphEdge>(numNodes);
            m_CostToThisNode = new List<double>(numNodes);

            for (int i = 0; i < numNodes; i++)
            {
                m_ShortestPathTree.Add(null);
                m_SearchFrontier.Add(null);
                m_CostToThisNode.Add(0);
            }            

            m_bFound = Search();
        }

        //returns the list of edges that defines the SPT. If a target was given
        //in the constructor then this will be an SPT comprising of all the nodes
        //examined before the target was found, else it will contain all the nodes
        //in the graph.
        public override List<NavGraphEdge> GetSearchTree() { return m_ShortestPathTree; }

        public override bool Search()
        {
            //create an indexed priority queue that sorts smallest to largest
            //(front to back).Note that the maximum number of elements the iPQ
            //may contain is N. This is because no node can be represented on the 
            //queue more than once.
            IndexedPriorityQLow pq = new IndexedPriorityQLow(m_CostToThisNode, m_Graph.NumNodes());

            //put the source node on the queue
            pq.insert(m_iSource);

            //while the queue is not empty
            while (!pq.empty())
            {
                //get lowest cost node from the queue. Don't forget, the return value
                //is a *node index*, not the node itself. This node is the node not already
                //on the SPT that is the closest to the source node
                int NextClosestNode = pq.Pop();

                //move this edge from the frontier to the shortest path tree
                m_ShortestPathTree[NextClosestNode] = m_SearchFrontier[NextClosestNode];

                //if the target has been found exit
                if (NextClosestNode == m_iTarget) return true;

                //now to relax the edges.
                SparseGraph.EdgeIterator EdgeItr = new SparseGraph.EdgeIterator(m_Graph, NextClosestNode);

                while (EdgeItr.MoveNext())
                {
                    //the total cost to the node this edge points to is the cost to the
                    //current node plus the cost of the edge connecting them.
                    double NewCost = m_CostToThisNode[NextClosestNode] + EdgeItr.Current.Cost;

                    //if this edge has never been on the frontier make a note of the cost
                    //to get to the node it points to, then add the edge to the frontier
                    //and the destination node to the PQ.
                    if (NavGraphEdge.IsNull(m_SearchFrontier[EdgeItr.Current.To]))
                    {
                        m_CostToThisNode[EdgeItr.Current.To] = NewCost;

                        pq.insert(EdgeItr.Current.To);

                        m_SearchFrontier[EdgeItr.Current.To] = EdgeItr.Current;
                    }

                    //else test to see if the cost to reach the destination node via the
                    //current node is cheaper than the cheapest cost found so far. If
                    //this path is cheaper, we assign the new cost to the destination
                    //node, update its entry in the PQ to reflect the change and add the
                    //edge to the frontier
                    else if ((NewCost < m_CostToThisNode[EdgeItr.Current.To]) &&
                              NavGraphEdge.IsNull(m_ShortestPathTree[EdgeItr.Current.To]))
                    {
                        m_CostToThisNode[EdgeItr.Current.To] = NewCost;

                        //because the cost is less than it was previously, the PQ must be
                        //re-sorted to account for this.
                        pq.ChangePriority(EdgeItr.Current.To);

                        m_SearchFrontier[EdgeItr.Current.To] = EdgeItr.Current;
                    }
                }
            }
            return false;
        }

        public override List<int> GetPathToTarget()
        {
            List<int> path = new List<int>();

            //just return an empty path if no path to target found or if
            //no target has been specified
            if (!m_bFound || m_iTarget < 0) return path;

            int nd = m_iTarget;

            path.Add(nd);

            while (nd != m_iSource && !(NavGraphEdge.IsNull(m_ShortestPathTree[nd])))
            {
                nd = m_ShortestPathTree[nd].From;

                path.Insert(0, nd); // Adds an element to the beginning of a list.
            }

            return path;
        }

        public override double GetCostToTarget()
        {
            return m_CostToThisNode[m_iTarget];
        }

        //returns the total cost to the given node
        public double GetCostToNode(int nd)
        {
            return m_CostToThisNode[nd];
        }
    }

    #endregion
}
