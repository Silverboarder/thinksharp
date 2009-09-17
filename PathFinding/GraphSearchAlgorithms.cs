using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.PathFinding
{
    #region " Depth first search "

    public class Graph_SearchDFS
    {
        private enum NodeState { visited, unvisited, no_parent_assigned };

        //a reference to the graph to be searched
        private SparseGraph m_Graph;

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

        //the source and target node indices
        private int m_iSource;
        private int m_iTarget;

        //true if a path to the target has been found
        private bool m_bFound;

        public Graph_SearchDFS(SparseGraph graph, int source, int target)
        {
            m_Graph = graph;
            m_iSource = source;
            m_iTarget = target;
            m_bFound = false;

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
        public List<NavGraphEdge> GetSearchTree() { return m_SpanningTree; }

        //returns true if the target node has been located
        public bool Found() { return m_bFound; }

        private bool Search()
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

        public List<int> GetPathToTarget()
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

    }

    #endregion


}
