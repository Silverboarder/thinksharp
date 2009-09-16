using System;

namespace ThinkSharp.PathFinding
{
    public class NavGraphEdge
    {
        //An edge connects two nodes. Valid node indices are always positive.
        protected int m_iFrom;
        protected int m_iTo;

        //the cost of traversing the edge
        protected double m_dCost;        

        //ctors
        public NavGraphEdge(int from, int to, double cost)
        {
            m_dCost = cost;
            m_iFrom = from;
            m_iTo = to;
        }

        public NavGraphEdge(int from, int to)
        {
            m_dCost = 1.0;
            m_iFrom = from;
            m_iTo = to;
        }

        public NavGraphEdge()
        {
            m_dCost = 1.0;
            m_iFrom = SparseGraph.invalid_node_index;
            m_iTo = SparseGraph.invalid_node_index;
        }

        public int From
        {
            get { return m_iFrom; }
            set { m_iFrom = value; }
        }

        public int To
        {
            get { return m_iTo; }
            set { m_iTo = value; }
        }

        public double Cost
        {
            get { return m_dCost; }
            set { m_dCost = value; }
        }

        public static bool operator ==(NavGraphEdge lhs, NavGraphEdge rhs)
        {
            return (lhs.m_iFrom == rhs.m_iFrom &&
                   lhs.m_iTo == rhs.m_iTo &&
                   lhs.m_dCost == rhs.m_dCost);
        }

        public static bool operator !=(NavGraphEdge lhs, NavGraphEdge rhs)
        {
            return !(lhs == rhs);
        }

        public static bool IsNull(NavGraphEdge o)
        {
            if ((object)o == null) return true;
            return false;
        }

    }
}
