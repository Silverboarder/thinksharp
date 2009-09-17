using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.PathFinding
{
    public abstract class BaseGraphSearchAlgo
    {
        protected SparseGraph m_Graph; //a reference to the graph to be searched
        protected int m_iSource;
        protected int m_iTarget;        
        protected bool m_bFound;

        //returns true if the target node has been located
        public bool Found() { return m_bFound; }

        public abstract bool Search();

        public abstract List<NavGraphEdge> GetSearchTree();

        public abstract List<int> GetPathToTarget();

        public abstract double GetCostToTarget(); 

        public BaseGraphSearchAlgo(SparseGraph graph, int source, int target)
        {
            m_Graph = graph;
            m_iSource = source;
            m_iTarget = target;
            m_bFound = false;
        }
    }
}
