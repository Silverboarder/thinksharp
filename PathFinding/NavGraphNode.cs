using System;
using ThinkSharp.Common;

namespace ThinkSharp.PathFinding
{
    public class NavGraphNode
    {
        protected int m_iIndex;
        public const int invalid_node_index = -1;

        //the node's position
        Vector2D m_vPosition;

        //often you will require a navgraph node to contain additional information.
        //For example a node might represent a pickup such as armor in which
        //case m_ExtraInfo could be an enumerated value denoting the pickup type,
        //thereby enabling a search algorithm to search a graph for specific items.
        //Going one step further, m_ExtraInfo could be a pointer to the instance of
        //the item type the node is twinned with. This would allow a search algorithm
        //to test the status of the pickup during the search. 
        Object m_ExtraInfo;

        public NavGraphNode()
        {
            m_iIndex = invalid_node_index;
        }

        public NavGraphNode(Object ExtraInfo)
        {
            m_iIndex = invalid_node_index;
            m_ExtraInfo = ExtraInfo;
        }

        public NavGraphNode(int idx)
        {
            m_iIndex = idx;
        }

        public NavGraphNode(int idx, Vector2D pos)
        {
            m_iIndex = idx;
            m_vPosition=pos;
        }

        public int Index
        {
            get { return m_iIndex; }
            set { m_iIndex = value; }
        }

        public Vector2D Pos
        {
            get { return m_vPosition; }
            set { m_vPosition = value; }
        }

        public Object ExtraInfo
        {
            get { return m_ExtraInfo; }
            set { m_ExtraInfo = value; }
        }
    }   

}
