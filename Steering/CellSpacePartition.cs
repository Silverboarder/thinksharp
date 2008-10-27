using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    //  defines a cell containing a list of pointers to entities
   public class Cell
    {
        //all the entities inhabiting this cell
       public List<MovingEntity> Members;

        //the cell's bounding box (it's inverted because the Window's default
        //co-ordinate system has a y axis that increases as it descends)
        public InvertedAABBox2D BBox;

        public Cell(Vector2D topleft, Vector2D botright)
        {
            BBox = new InvertedAABBox2D(topleft, botright);
            Members = new List<MovingEntity>();
        }
    };

    public class CellSpacePartition
    {
        //the required amount of cells in the space
        private List<Cell> m_Cells;

        //this is used to store any valid neighbors when an agent searches its neighboring space
        private List<MovingEntity> m_Neighbors;

        //the width and height of the world space the entities inhabit
        private double  m_dSpaceWidth;
        private double  m_dSpaceHeight;

        //the number of cells the space is going to be divided up into
        private int    m_iNumCellsX;
        private int    m_iNumCellsY;

        private double  m_dCellSizeX;
        private double  m_dCellSizeY;

        public CellSpacePartition(double width,     //width of the environment
                     double height,       //height ...
                     int cellsX,       //number of cells horizontally
                     int cellsY,       //number of cells vertically
                     int MaxEntitys)  //maximum number of entities to add
        {
            m_dSpaceWidth = width;
            m_dSpaceHeight = height;
            m_iNumCellsX = cellsX;
            m_iNumCellsY = cellsY;
            m_Neighbors = new List<MovingEntity>(MaxEntitys);

            //calculate bounds of each cell
            m_dCellSizeX = width / cellsX;
            m_dCellSizeY = height / cellsY;

            m_Cells = new List<Cell>(m_iNumCellsX * m_iNumCellsY);

            //create the cells
            for (int y = 0; y < m_iNumCellsY; ++y)
            {
                for (int x = 0; x < m_iNumCellsX; ++x)
                {
                    double left = x * m_dCellSizeX;
                    double right = left + m_dCellSizeX;
                    double top = y * m_dCellSizeY;
                    double bot = top + m_dCellSizeY;

                    m_Cells.Add(new Cell(new Vector2D(left, top), new Vector2D(right, bot)));
                }
            }
        }

        public List<MovingEntity> ListOfNeighbours() { return m_Neighbors; }

        //given a position in the game space this method determines the relevant cell's index
        private int PositionToIndex(Vector2D pos)
        {
            int idx = (int)(m_iNumCellsX * pos.X / m_dSpaceWidth) + 
                    ((int)((m_iNumCellsY) * pos.Y / m_dSpaceHeight) * m_iNumCellsX);

            //if the entity's position is equal to vector2d(m_dSpaceWidth, m_dSpaceHeight)
            //then the index will overshoot. We need to check for this and adjust
            if (idx > m_Cells.Count - 1) idx = m_Cells.Count - 1;

            return idx;
        }

        //----------------------- CalculateNeighbors ----------------------------
        //
        //  This must be called to create the collection of neighbors. This method 
        //  examines each cell within range of the target, If the 
        //  cells contain entities then they are tested to see if they are situated
        //  within the target's neighborhood region. If they are they are added to
        //  neighbor list
        //------------------------------------------------------------------------
        public void CalculateNeighbors(Vector2D TargetPos, double QueryRadius)
        {
            //create the query box that is the bounding box of the target's query area
            InvertedAABBox2D QueryBox = new InvertedAABBox2D(TargetPos - new Vector2D(QueryRadius, QueryRadius),
                    TargetPos + new Vector2D(QueryRadius, QueryRadius));

            m_Neighbors.Clear();

            Double dblDoubleRadius = QueryRadius * QueryRadius;

            //iterate through each cell and test to see if its bounding box overlaps
            //with the query box. If it does and it also contains entities then make further proximity tests.

            foreach (Cell curCell in m_Cells)
            {
                if ((curCell.Members.Count == 0) && curCell.BBox.isOverlappedWith(QueryBox))
                {
                    //add any entities found within query radius to the neighbor list
                    foreach (MovingEntity objEntity in curCell.Members)
                    {
                        if (Vector2D.Vec2DDistanceSq(objEntity.Pos, TargetPos) < dblDoubleRadius)
                        {
                            m_Neighbors.Add(objEntity);
                        }
                    }

                }
            } // next cell

        }

        //--------------------------- Empty --------------------------------------
        //
        //  clears the cells of all entities
        //------------------------------------------------------------------------
        public void EmptyCells()
        {
            foreach (Cell curCell in m_Cells)
            {
                curCell.Members.Clear();
            }
        }

        //----------------------- AddEntity --------------------------------------
        //
        //  Used to add the entitys to the data structure
        //------------------------------------------------------------------------
        public void AddEntity(MovingEntity ent)
        {
            System.Diagnostics.Debug.Assert(ent != null);

            int idx = PositionToIndex(ent.Pos);

            m_Cells[idx].Members.Add(ent);
        }

        //----------------------- UpdateEntity -----------------------------------
        //
        //  Checks to see if an entity has moved cells. If so the data structure
        //  is updated accordingly
        //------------------------------------------------------------------------
        public void UpdateEntity(MovingEntity ent, Vector2D OldPos)
        {
            System.Diagnostics.Debug.Assert(ent != null);

	        //if the index for the old pos and the new pos are not equal then
	        //the entity has moved to another cell.
	        int OldIdx = PositionToIndex(OldPos);
	        int NewIdx = PositionToIndex(ent.Pos);

	        if (NewIdx == OldIdx) return;

	        //the entity has moved into another cell so delete from current cell
	        //and add to new one
	        m_Cells[OldIdx].Members.Remove(ent);
	        m_Cells[NewIdx].Members.Add(ent);
        }

    } // end class CellSpacePartition
}
