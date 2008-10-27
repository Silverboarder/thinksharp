using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    public sealed class GameWorld
    {
        private static readonly GameWorld instance = new GameWorld();

        public static GameWorld Instance
        {
            get { return instance; }
        }

        //a container of all the moving entities
        private List<MovingEntity> m_Agents;

        //any obstacles
        private List<BaseGameEntity> m_Obstacles;

        //container containing any walls in the environment
        private List<Wall2D> m_Walls;

        private CellSpacePartition m_pCellSpace;

        //set true to pause the motion
        private bool m_bPaused;

        //set true to wrap around
        private bool m_bWrap;

        private int m_cxClient, m_cyClient;

        //keeps a track of the most recent update time. (some of the
        //steering behaviors make use of this - see Wander)
        double m_dTimeElapsed;
        
        //The position of the target/crosshair
        //Used by the Arrive, Flee and Seek behaviours
        Vector2D m_vTargetPos;

        private GameWorld()
        {
            m_bPaused = false;
            m_bWrap = false;

            m_dTimeElapsed = 0.0;

            m_Walls = null;
            m_pCellSpace = null;
            m_Obstacles = null;
            m_Agents = null;

            m_cxClient = 0;
            m_cyClient = 0;

            m_vTargetPos = new Vector2D();
        }

        public List<Wall2D> Walls
        {
            get { return m_Walls; }
            set { m_Walls = value; }
        }

        public CellSpacePartition CellSpaces
        {
            get { return m_pCellSpace; }
            set { m_pCellSpace = value; }
        }

        public List<BaseGameEntity> Obstacles
        {
            get { return m_Obstacles; }
            set { m_Obstacles = value; }
        }

        public List<MovingEntity> Agents
        {
            get { return m_Agents; }
            set { m_Agents = value; }
        }

        public bool Paused
        {
            get { return m_bPaused; }
            set { m_bPaused = value; }
        }

        public Vector2D TargetPos
        {
            get { return m_vTargetPos; }
            set { m_vTargetPos = value; }
        }

        public bool Wrap
        {
            get { return m_bWrap; }
            set { m_bWrap = value; }
        } //
        
        public int cxClient
        {
            get { return m_cxClient; }
            set { m_cxClient = value; }
        }

        public int cyClient
        {
            get { return m_cyClient; }
            set { m_cyClient = value; }
        }

        public double getTimeElapsed() { return m_dTimeElapsed; }

        //----------------------------- Update -----------------------------------
        public void Update(double time_elapsed)
        {
            if (m_bPaused) return;

            m_dTimeElapsed = time_elapsed;

            if (m_Agents != null)
            {
                //update the vehicles
                foreach (MovingEntity objVehicle in m_Agents)
                {
                    objVehicle.Update(m_dTimeElapsed);

                    if (m_bWrap)
                    {
                        //treat the screen as a toroid
                        Vector2D.WrapAround(objVehicle.Pos, m_cxClient, m_cyClient);
                    }

                    if (objVehicle.Steering().isSpacePartitioningOn())
                    {
                        m_pCellSpace.UpdateEntity(objVehicle, objVehicle.OldPos);
                    }
                }
            }
        }

        public void NonPenetrationContraint(MovingEntity v) { Utils.EnforceNonPenetrationConstraint(v, m_Agents); }

        public void TagAgentsWithinViewRange(BaseGameEntity pEntity, double radius)
        {
            //  tags any entities contained that are within the
            //  radius of the single entity parameter

            //iterate through all entities checking for range
            foreach (MovingEntity curEntity in m_Agents)
            {
                //first clear any current tag
                curEntity.UnTag();

                Vector2D to = curEntity.Pos - pEntity.Pos;

                //the bounding radius of the other is taken into account by adding it 
                //to the range
                double range = radius + curEntity.BRadius;

                //if entity within range, tag for further consideration. (working in
                //distance-squared space to avoid sqrts)
                if ((curEntity != pEntity) && (to.LengthSq() < range * range))
                {
                    curEntity.Tag();
                }

            }//next entity
        
        }

        public void TagObstaclesWithinViewRange(BaseGameEntity pEntity, double radius)
        {
            //iterate through all entities checking for range
            foreach (BaseGameEntity curEntity in m_Obstacles)
            {

                //first clear any current tag
                curEntity.UnTag();

                Vector2D to = curEntity.Pos - pEntity.Pos;

                //the bounding radius of the other is taken into account by adding it 
                //to the range
                double range = radius + curEntity.BRadius;

                //if entity within range, tag for further consideration. (working in
                //distance-squared space to avoid sqrts)
                if ((curEntity != pEntity) && (to.LengthSq() < range * range))
                {
                    curEntity.Tag();
                }

            }//next entity
        }

    }
}
