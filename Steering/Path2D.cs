using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    public class Path2D
    {
        protected List<Vector2D> m_WayPoints;

        //points to the current waypoint
        private IEnumerator<Vector2D> curWaypoint;

        //flag to indicate if the path should be looped
        //(The last waypoint connected to the first)
        protected bool m_bLooped;

        public Path2D()
        {
            initialise();		
            m_bLooped = false;
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

            // set the first item ready to go
            curWaypoint.MoveNext();
	    }

        private void initialise()
        {
            m_WayPoints = new List<Vector2D>();
            curWaypoint = m_WayPoints.GetEnumerator();
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

            curWaypoint = m_WayPoints.GetEnumerator();
        }

	    //returns the current waypoint
        public Vector2D CurrentWaypoint() 
        {
            System.Diagnostics.Debug.Assert(curWaypoint != null);
            return curWaypoint.Current;
        }

	    //returns true if the end of the list has been reached
	    public bool Finished()
        {
            return (curWaypoint.Current == null);
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
            curWaypoint = m_WayPoints.GetEnumerator();

            curWaypoint.MoveNext(); // set the first item ready to go
        }

	    public void Set(Path2D path)
        {
            m_WayPoints = path.GetPath();
            curWaypoint = m_WayPoints.GetEnumerator();

            curWaypoint.MoveNext(); // set the first item ready to go
        }

        public void SetNextWaypoint()
        {
	        System.Diagnostics.Debug.Assert(m_WayPoints.Count > 0);

            if (!curWaypoint.MoveNext())
            {
                if (m_bLooped)
                {
                    curWaypoint.Reset();
                    curWaypoint.MoveNext(); // set the first item ready to go
                }
            }
        }  

    }
}
