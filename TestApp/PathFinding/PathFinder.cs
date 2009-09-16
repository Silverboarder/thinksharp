using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ThinkSharp.Common;
using ThinkSharp.PathFinding;

namespace TestApp
{
    public class PathFinder
    {
        public enum brush_type
        {
            normal = 0,
            obstacle = 1,
            water = 2,
            mud = 3,
            source = 4,
            target = 5
        };

        public enum algorithm_type
        {
            none,
            search_astar,
            search_bfs,
            search_dfs,
            search_dijkstra
        };

        //the terrain type of each cell
        private List<int> m_TerrainType;

        //this list will store any path returned from a graph search
        private List<int> m_Path;

        private SparseGraph m_Graph;

        //this list of edges is used to store any subtree returned from 
        //any of the graph algorithms (such as an SPT)
        private List<NavGraphEdge> m_SubTree;

        //the total cost of the path from target to source
        private double m_dCostToTarget;

        //the currently selected algorithm
        private algorithm_type m_CurrentAlgorithm;

        //the current terrain brush
        private brush_type m_CurrentTerrainBrush;

        //the dimensions of the cells
        private double m_dCellWidth;
        private double m_dCellHeight;

        //number of cells vertically and horizontally
        private int m_iCellsX;
        private int m_iCellsY;

        //local record of the client area
        private int m_icxClient;
        private int m_icyClient;

        //the indices of the source and target cells
        private int m_iSourceCell;
        private int m_iTargetCell;

        //should the graph (nodes and GraphEdges) be rendered?
        private bool m_bShowGraph;

        //should the tile outlines be rendered
        private bool m_bShowTiles;

        //holds the time taken for the most currently used algorithm to complete
        private double m_dTimeTaken;

        private Pen m_ThickBlack, m_ThickBlue;

        public PathFinder()
        {
            m_bShowGraph = false;
            m_bShowTiles = true;
            m_dCellWidth = 0;
            m_dCellHeight = 0;
            m_iCellsX = 0;
            m_iCellsY = 0;
            m_dTimeTaken = 0.0;
            m_CurrentTerrainBrush = brush_type.normal;
            m_iSourceCell = -1;
            m_iTargetCell = -1;
            m_icxClient = 0;
            m_icyClient = 0;
            m_dCostToTarget = 0.0;
            m_Graph = null;

            m_ThickBlack = new Pen(Color.Black, 2);
            m_ThickBlue = new Pen(Color.Blue, 2);
        }

        public bool ShowGraph
        {
            get { return m_bShowGraph; }
            set { m_bShowGraph = value; }
        }

        public bool ShowTiles
        {
            get { return m_bShowTiles; }
            set { m_bShowTiles = value; }
        }

        public brush_type CurrentTerrainBrush
        {
            get { return m_CurrentTerrainBrush; }
            set { m_CurrentTerrainBrush = value; }
        }

        public int SourceCell
        {
            get { return m_iSourceCell; }
            set { m_iSourceCell = value; }
        }

        public int TargetCell
        {
            get { return m_iTargetCell; }
            set { m_iTargetCell = value; }
        }

        public bool IsMandatoryCellsSet()
        {
            return (m_iSourceCell > -1 && m_iTargetCell > -1);
        }

        public string GetNameOfCurrentSearchAlgorithm()
        {
            switch (m_CurrentAlgorithm)
            {
                case algorithm_type.none: return "";
                case algorithm_type.search_astar: return "A Star";
                case algorithm_type.search_bfs: return "Breadth First";
                case algorithm_type.search_dfs: return "Depth First";
                case algorithm_type.search_dijkstra: return "Dijkstras";
                default: return "UNKNOWN!";
            }
        }

        public algorithm_type GetCurrentAlgorithm()
        {
            return m_CurrentAlgorithm;
        }

        public double GetCostToTarget()
        {
            return m_dCostToTarget;
        }

        public double GetTimeTaken()
        {
            return m_dTimeTaken;
        }

        public void InitialiseGraph(int CellsUp, int CellsAcross, int pWidth, int pHeight)
        {
            m_iCellsX = CellsAcross;
            m_iCellsY = CellsUp;

            m_icxClient = pWidth;
            m_icyClient = pHeight;

            //initialize the terrain vector with normal terrain
            int numNodes = CellsUp * CellsAcross;
            m_TerrainType = new List<int>(numNodes);

            for (int i = 0; i < numNodes; i++)
            {
                m_TerrainType.Add((int)brush_type.normal);
            }

            m_Path = new List<int>();
            m_SubTree = new List<NavGraphEdge>();

            m_dCellWidth = (double)m_icxClient / (double)CellsAcross;
            m_dCellHeight = (double)m_icyClient / (double)CellsUp;

            //create the graph
            m_Graph = new SparseGraph(false);//not a digraph

            SparseGraph.Helper_CreateGrid(m_Graph, m_icxClient, m_icyClient, CellsUp, CellsAcross);

            m_CurrentAlgorithm = algorithm_type.none;
            m_dTimeTaken = 0;
        }

        //initialize source and target indexes to mid top and bottom of grid 
        public void InitialiseSourceTargetIndexes()
        {            
            Vector2DToIndex(new Vector2D(m_icxClient / 2, m_dCellHeight * 2), ref m_iTargetCell);
            Vector2DToIndex(new Vector2D(m_icxClient / 2, m_icyClient - m_dCellHeight * 2), ref m_iSourceCell);
        }

        //--------------------- PointToIndex -------------------------------------
        //
        //  converts a Vector2D into an index into the graph
        //------------------------------------------------------------------------
        public bool Vector2DToIndex(Vector2D p, ref int NodeIndex)
        {
            //convert p to an index into the graph
            int x = (int)((double)(p.X) / m_dCellWidth);
            int y = (int)((double)(p.Y) / m_dCellHeight);

            //make sure the values are legal
            if ((x > m_iCellsX) || (y > m_iCellsY))
            {
                NodeIndex = -1;

                return false;
            }

            NodeIndex = y * m_iCellsX + x;

            return true;
        }

        //----------------- GetTerrainCost ---------------------------------------
        //
        //  returns the cost of the terrain represented by the current brush type
        //------------------------------------------------------------------------
        public double GetTerrainCost(brush_type brush)
        {
            const double cost_normal = 1.0;
            const double cost_water = 2.0;
            const double cost_mud = 1.5;

            switch (brush)
            {
                case brush_type.normal: return cost_normal;
                case brush_type.water: return cost_water;
                case brush_type.mud: return cost_mud;
                default: return Double.MaxValue;
            };
        }

        //----------------------- PaintTerrain -----------------------------------
        //
        //  this either changes the terrain at position p to whatever the current
        //  terrain brush is set to, or it adjusts the source/target cell
        //------------------------------------------------------------------------
        public void PaintTerrain(Point p)
        {
            int NodeIndex = -1;
            Vector2D newVec = new Vector2D(p.X, p.Y);

            //convert p to an index into the graph
            if (!Vector2DToIndex(newVec, ref NodeIndex)) return;

            int x = (int)((double)(p.X) / m_dCellWidth);
            int y = (int)((double)(p.Y) / m_dCellHeight);

            //reset path and tree records
            m_SubTree.Clear();
            m_Path.Clear();

            //if the current terrain brush is set to either source or target we
            //should change the appropriate node
            if ((m_CurrentTerrainBrush == brush_type.source) || (m_CurrentTerrainBrush == brush_type.target))
            {
                switch (m_CurrentTerrainBrush)
                {
                    case brush_type.source:

                        m_iSourceCell = NodeIndex;
                        break;

                    case brush_type.target:

                        m_iTargetCell = NodeIndex;
                        break;

                }//end switch
            }
            //otherwise, change the terrain at the current NodeIndex position
            else
            {
                UpdateGraphFromBrush((int)m_CurrentTerrainBrush, NodeIndex);
            }

            //update any currently selected algorithm
            UpdateAlgorithm();
        }

        //--------------------------- UpdateGraphFromBrush ----------------------------
        //
        //  given a brush and a node index, this method updates the graph appropriately
        //  (by removing/adding nodes or changing the costs of the node's edges)
        //-----------------------------------------------------------------------------
        public void UpdateGraphFromBrush(int brush, int CellIndex)
        {
            //set the terrain type in the terrain index
            m_TerrainType[CellIndex] = brush;

            //if current brush is an obstacle then this node must be removed
            //from the graph
            if (brush == (int)brush_type.obstacle)
            {
                m_Graph.RemoveNode(CellIndex);
            }

            else
            {
                //make the node active again if it is currently inactive
                if (!m_Graph.isNodePresent(CellIndex))
                {
                    int y = CellIndex / m_iCellsY;
                    int x = CellIndex - (y * m_iCellsY);

                    m_Graph.AddNode(new NavGraphNode(CellIndex, new Vector2D(x * m_dCellWidth + m_dCellWidth / 2.0,
                                                                             y * m_dCellHeight + m_dCellHeight / 2.0)));

                    SparseGraph.Helper_AddAllNeighboursToGridNode(m_Graph, y, x, m_iCellsX, m_iCellsY);
                }

                //set the edge costs in the graph
                SparseGraph.Helper_WeightNavGraphNodeEdges(m_Graph, CellIndex, GetTerrainCost((brush_type)brush));
            }
        }

        //--------------------------- UpdateAlgorithm ---------------------------------
        public void UpdateAlgorithm()
        {
            //update any current algorithm
            switch (m_CurrentAlgorithm)
            {
                case algorithm_type.none: break;

                case algorithm_type.search_dfs:

                    CreatePathDFS(); break;

                case algorithm_type.search_bfs:

                    CreatePathBFS(); break;

                case algorithm_type.search_dijkstra:

                    CreatePathDijkstra(); break;

                case algorithm_type.search_astar:

                    CreatePathAStar(); break;

                default: break;
            }
        }

        //------------------------- CreatePathDFS --------------------------------
        //
        //  uses DFS to find a path between the start and target cells.
        //  Stores the path as a series of node indexes in m_Path.
        //------------------------------------------------------------------------
        public void CreatePathDFS()
        {
            //set current algorithm
            m_CurrentAlgorithm = algorithm_type.search_dfs;

            //clear any existing path
            m_Path.Clear();
            m_SubTree.Clear();

            //create and start a timer
            HighResTimer tempTimer = new HighResTimer();

            tempTimer.Start();

            //do the search
            Graph_SearchDFS DFS = new Graph_SearchDFS(m_Graph, m_iSourceCell, m_iTargetCell);

            tempTimer.Stop();

            //record the time taken  
            m_dTimeTaken = tempTimer.RunningTime;

            //now grab the path (if one has been found)
            if (DFS.Found())
            {
                m_Path = DFS.GetPathToTarget();
            }

            m_SubTree = DFS.GetSearchTree();

            m_dCostToTarget = 0.0;
        }

        //------------------------- CreatePathBFS --------------------------------
        //
        //  uses BFS to find a path between the start and target cells.
        //  Stores the path as a series of node indexes in m_Path.
        //------------------------------------------------------------------------
        public void CreatePathBFS()
        {
            //
        }

        //  creates a path from m_iSourceCell to m_iTargetCell using Dijkstra's algorithm
        public void CreatePathDijkstra()
        {
//
        }

        public void CreatePathAStar()
        {
//

        }

        public void Save(string strFileName)
        {
            using (StreamWriter fileStream = new StreamWriter(strFileName))
            {
                Save(fileStream);
            }
        }

        public void Save(StreamWriter fileStream)
        {
            fileStream.WriteLine(m_iCellsX);
            fileStream.WriteLine(m_iCellsY);

            fileStream.WriteLine(m_iSourceCell);
            fileStream.WriteLine(m_iTargetCell);

            //save the terrain
            for (int t = 0; t < m_TerrainType.Count; ++t)
            {
                fileStream.WriteLine(m_TerrainType[t]);
            }
        }

        public void Load(string strFileName, int Width, int Height)
        {
            if (!File.Exists(strFileName))
            {
                throw new Exception("<PathFinder::Save>: strFileName does not exist");
            }

            using (StreamReader fileStream = new StreamReader(strFileName))
            {
                Load(fileStream, Width, Height);
            }
        }

        public void Load(StreamReader fileStream, int Width, int Height)
        {
            m_iCellsX = int.Parse(fileStream.ReadLine());
            m_iCellsY = int.Parse(fileStream.ReadLine());

            m_iSourceCell = int.Parse(fileStream.ReadLine());
            m_iTargetCell = int.Parse(fileStream.ReadLine());

            InitialiseGraph(m_iCellsY, m_iCellsX, Width, Height);

            int terrain;

            for (int t = 0; t < m_iCellsX * m_iCellsY; ++t)
            {
                String input = fileStream.ReadLine();

                if (input == null)
                {
                    throw new Exception("<PathFinder::Load>: unexpected file contents");
                }

                terrain = int.Parse(input);

                m_TerrainType[t] = terrain;

                UpdateGraphFromBrush(terrain, t);
            }
        }

        public void Render(Graphics objGraphics)
        {
            //render all the cells
            for (int nd = 0; nd < m_Graph.NumNodes(); ++nd)
            {
                int left = (int)(m_Graph.GetNode(nd).Pos.X - m_dCellWidth / 2.0);
                int top = (int)(m_Graph.GetNode(nd).Pos.Y - m_dCellHeight / 2.0);
                int right = (int)(1 + m_Graph.GetNode(nd).Pos.X + m_dCellWidth / 2.0);
                int bottom = (int)(1 + m_Graph.GetNode(nd).Pos.Y + m_dCellHeight / 2.0);

                Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);

                SolidBrush fillBrush = new SolidBrush(Color.Gray);

                switch (m_TerrainType[nd])
                {
                    case (int)brush_type.normal:
                        fillBrush.Color = Color.White;
                        break;

                    case (int)brush_type.obstacle:
                        fillBrush.Color = Color.Black;
                        break;

                    case (int)brush_type.water:
                        fillBrush.Color = Color.DodgerBlue;
                        break;

                    case (int)brush_type.mud:
                        fillBrush.Color = Color.Brown;
                        break;

                    default:
                        fillBrush.Color = Color.White;
                        break;

                }//end switch

                objGraphics.FillRectangle(fillBrush, rect);

                if (m_bShowTiles)
                {
                    objGraphics.DrawLine(Pens.Gray, rect.Location, new Point(rect.Right, rect.Top));
                    objGraphics.DrawLine(Pens.Gray, rect.Location, new Point(rect.Left, rect.Bottom));
                }

                if (nd == m_iTargetCell)
                {
                    rect.Inflate(-4, -4);
                    objGraphics.FillRectangle(Brushes.Red, rect);
                }
                else if (nd == m_iSourceCell)
                {
                    rect.Inflate(-4, -4);
                    objGraphics.FillRectangle(Brushes.LightGreen, rect);
                }

                //render dots at the corners of the cells
                objGraphics.DrawLine(m_ThickBlack, left, top, left + 1, top + 1);
            }

            //draw the graph nodes and edges if rqd
            if (m_bShowGraph)
            {
                if (m_Graph.NumNodes() > 0)
                {
                    SparseGraph.NodeIterator NodeItr = new SparseGraph.NodeIterator(m_Graph);

                    while (NodeItr.MoveNext())
                    {
                        objGraphics.DrawEllipse(Pens.LightGray, (int)(NodeItr.Current.Pos.X - 2), (int)(NodeItr.Current.Pos.Y - 2), (int)2 * 2, (int)2 * 2);

                        SparseGraph.EdgeIterator EdgeItr = new SparseGraph.EdgeIterator(m_Graph, NodeItr.Current.Index);

                        while (EdgeItr.MoveNext())
                        {
                            objGraphics.DrawLine(Pens.LightGray, (int)NodeItr.Current.Pos.X, (int)NodeItr.Current.Pos.Y, (int)m_Graph.GetNode(EdgeItr.Current.To).Pos.X, (int)m_Graph.GetNode(EdgeItr.Current.To).Pos.Y);
                        }

                    }
                }
            }

            //draw any tree retrieved from the algorithms
            for (int e = 0; e < m_SubTree.Count; ++e)
            {
                if (!NavGraphEdge.IsNull(m_SubTree[e]))
                {
                    Vector2D from = m_Graph.GetNode(m_SubTree[e].From).Pos;
                    Vector2D to = m_Graph.GetNode(m_SubTree[e].To).Pos;

                    objGraphics.DrawLine(Pens.Red, (int)from.X, (int)from.Y, (int)to.X, (int)to.Y);
                }
            }

            //draw the path (if any)  
            if (m_Path.Count > 0)
            {
                for (int i = 0; i < m_Path.Count; ++i)
                {
                    if (i > 0)
                    {
                        Point start = new Point((int)m_Graph.GetNode(m_Path[i - 1]).Pos.X, (int)m_Graph.GetNode(m_Path[i - 1]).Pos.Y);
                        Point end = new Point((int)m_Graph.GetNode(m_Path[i]).Pos.X, (int)m_Graph.GetNode(m_Path[i]).Pos.Y);

                        objGraphics.DrawLine(m_ThickBlue, start, end);
                    }
                }
            }
        }

    }
}
