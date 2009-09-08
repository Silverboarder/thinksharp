using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ThinkSharp.Common;

namespace ThinkSharp.Steering
{
    public class Path2D
    {
        protected List<Vector2D> m_WayPoints;

        //points to the current waypoint
        private IEnumerator<Vector2D> m_curWaypoint;

        //flag to indicate if the path should be looped
        //(The last waypoint connected to the first)
        private bool m_bLooped;

        public Path2D(bool looped)
        {
            initialise();
            m_bLooped = looped;
        }

        public Path2D(int NumWaypoints,
		    double MinX,
		    double MinY,
		    double MaxX,
		    double MaxY,
		    bool looped)
	    {
            initialise();	

            m_bLooped = false;

		    CreateRandomPath(NumWaypoints, MinX, MinY, MaxX, MaxY);
	    }

        private void initialise()
        {
            m_WayPoints = new List<Vector2D>();
            m_curWaypoint = m_WayPoints.GetEnumerator();
        }

        public void CreateRandomPath(int   NumWaypoints,
								           double MinX,
								           double MinY,
								           double MaxX,
								           double MaxY)
        {

            m_WayPoints.Clear();

            double midX = (MaxX+MinX)/2.0;
            double midY = (MaxY+MinY)/2.0;

            double smaller = Math.Min(midX, midY);

            double spacing = Utils.TwoPi / (double)NumWaypoints;

            for (int i=0; i<NumWaypoints; ++i)
            {
                double RadialDist = Utils.RandInRange(smaller*0.2f, smaller);

                Vector2D temp = new Vector2D(RadialDist, 0.0f);

                Utils.Vec2DRotateAroundOrigin(temp, i * spacing);

                temp.X += midX; 
                temp.Y += midY;

                m_WayPoints.Add(temp);

            }            

            m_curWaypoint = m_WayPoints.GetEnumerator();
            m_curWaypoint.MoveNext(); // set the first item ready to go
        }

	    //returns the current waypoint
        public Vector2D CurrentWaypoint() 
        {
            Debug.Assert(m_curWaypoint != null);
            return m_curWaypoint.Current;
        }

	    //returns true if the end of the list has been reached
	    public bool Finished()
        {
            return (Vector2D.IsNull(m_curWaypoint.Current));
        }

        //returns true if the current target is the final way point
        public bool Finishing()
        {
            if (!Finished())
            {
                if (m_curWaypoint.Current == m_WayPoints[m_WayPoints.Count - 1])
                {
                    return true;
                }
            }

            return false;
        }

        public bool Loop
        {
            get { return m_bLooped; }
            set { m_bLooped = value; }
        }

        public List<Vector2D> GetPath() {return m_WayPoints;}

        public void Clear() { m_WayPoints.Clear(); }

	    public void Set(List<Vector2D> new_path)
        {
            m_WayPoints = new_path;
            m_curWaypoint = m_WayPoints.GetEnumerator();            

            m_curWaypoint.MoveNext(); // set the first item ready to go
        }

	    public void Set(Path2D path)
        {
            m_WayPoints = path.GetPath();
            m_curWaypoint = m_WayPoints.GetEnumerator();

            m_bLooped = path.Loop;

            m_curWaypoint.MoveNext(); // set the first item ready to go
        }

        public void SetNextWaypoint()
        {
	        Debug.Assert(m_WayPoints.Count > 0);

            if (!m_curWaypoint.MoveNext())
            {
                if (m_bLooped)
                {
                    m_curWaypoint.Reset();
                    m_curWaypoint.MoveNext(); // set the first item ready to go
                }
            }
        }  

    }
}
