using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using ThinkSharp.Steering;

namespace TestApp
{
    class SteeringScenario
    {
        //local copy of client window dimensions
        private int m_cxClient, m_cyClient;

        private List<MovingEntity> m_Vehicles = new List<MovingEntity>();

        //any obstacles
        private List<BaseGameEntity> m_Obstacles = new List<BaseGameEntity>();

        //container containing any walls in the environment
        private List<Wall2D> m_Walls = new List<Wall2D>();

        private CellSpacePartition m_pCellSpace;

        //any path we may create for the vehicles to follow
        private Path2D m_pPath;

        private Pen objVehiclePen, objDarkPen, objWallPen, objObstaclePen, objTargetPen, objRedPen, objCellPen, objGrayPen, objPathPen;

        private int m_intSharkieID, m_intVictimID = 0;

        private int m_bordersize = 10;

        private Font mFont = new Font("Arial", 8);

        private bool m_blnRenderAids = false;
        private bool m_blnNonePenetrationOn = true;

        public SteeringScenario(int cx, int cy)
        {
            m_cxClient = cx;
            m_cyClient = cy;

            GameWorld.Instance.cxClient = m_cxClient;
            GameWorld.Instance.cyClient = m_cyClient;

            GameWorld.Instance.Wrap = true;
            GameWorld.Instance.SpacePartitioningOn = true;

            objVehiclePen = new Pen(Color.Black);
            objDarkPen = new Pen(Color.SeaGreen, 2);
            objWallPen = new Pen(Color.DarkOrange);
            objObstaclePen = new Pen(Color.Orange);
            objTargetPen = new Pen(Color.Blue, 1);
            objRedPen = new Pen(Color.Red, 2);
            objCellPen = new Pen(Color.LightSkyBlue, 1);
            objGrayPen = new Pen(Color.LightSlateGray, 2);
            objPathPen = new Pen(Color.Blue, 2);

            objPathPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            objPathPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            m_pCellSpace = new CellSpacePartition(m_cxClient, m_cyClient, SteerParams.Instance.NumCellsX, SteerParams.Instance.NumCellsY, SteerParams.Instance.NumAgents);

            CreateObstacles();
            CreateWalls();            
            CreatePath();            

            //setup the agents
            for (int a = 0; a < SteerParams.Instance.NumAgents; ++a)
            {
                //determine a random starting position
                Vector2D SpawnPos = new Vector2D(cx / 2.0 + Utils.RandomClamped() * cx / 2.0, cy / 2.0 + Utils.RandomClamped() * cy / 2.0);

                double rotation = Utils.RandFloat() * Utils.TwoPi;

                MovingEntity pVehicle = new MovingEntity(SpawnPos,  //initial position
                                  SteerParams.Instance.VehicleScale,
                                  new Vector2D(0, 0),
                                  SteerParams.Instance.MaxSpeed,
                                  new Vector2D(Math.Sin(rotation), -Math.Cos(rotation)),
                                  SteerParams.Instance.VehicleMass,
                                  new Vector2D(SteerParams.Instance.VehicleScale, SteerParams.Instance.VehicleScale),
                                  SteerParams.Instance.MaxTurnRatePerSecond,
                                  SteerParams.Instance.AppliedMaxSteeringForce());

                pVehicle.Steering().FlockingOn();
                pVehicle.Steering().WallAvoidanceOn();
                pVehicle.Steering().ObstacleAvoidanceOn();

                m_Vehicles.Add(pVehicle);

                //add it to the cell subdivision
                m_pCellSpace.AddEntity(pVehicle);
            }

            // Turn the last one into a shark!
            int sharkie = SteerParams.Instance.NumAgents - 1;

            m_Vehicles[sharkie].Steering().FlockingOff();
            m_Vehicles[sharkie].SetScale(new Vector2D(6, 6));            
            m_Vehicles[sharkie].MaxSpeed = 80;
            m_Vehicles[sharkie].Steering().GetPath().Set(m_pPath);

            m_intSharkieID = m_Vehicles[sharkie].ID();

            setNextPursuitTarget();

            for (int a = 0; a < SteerParams.Instance.NumAgents - 1; ++a)
            {
                m_Vehicles[a].Steering().EvadeOn(m_Vehicles[sharkie]);
            }

            GameWorld.Instance.Agents = m_Vehicles;    
            GameWorld.Instance.Walls = m_Walls;
            GameWorld.Instance.Obstacles = m_Obstacles;
            GameWorld.Instance.CellSpaces = m_pCellSpace;
        }

        public void Update(double time_elapsed)
        {
            GameWorld.Instance.Update(time_elapsed);            
        }

        public void Render(Graphics objGraphics)
        {
            if (isFollowPathOn())
            {
                RenderPath2D(m_pPath, objGraphics, objPathPen);
            }

            foreach (MovingEntity objVehicle in m_Vehicles)
            {
                Pen objDrawPen = objVehiclePen;

                if (isPursuitOn() && (objVehicle.ID() == m_intVictimID))
                {
                    objDrawPen = objTargetPen;
                }

                if (m_blnNonePenetrationOn)
                {
                    List<MovingEntity> ListTouched = Utils.EnforceNonPenetrationConstraint(objVehicle, m_Vehicles);

                    if (ListTouched.Count > 0)
                    {
                        if (objVehicle.ID() == m_intSharkieID)
                        {
                            objDrawPen = objRedPen;
                        }
                        else
                        {
                            objDrawPen = objDarkPen;
                        }
                    }
                }               

                renderVehicle(objVehicle, objGraphics, objDrawPen);

                if (m_blnRenderAids)
               {
                   if ((objVehicle.ID() == m_intSharkieID))
                   {
                       Vector2D vecForce = (Vector2D)(objVehicle.Steering().Force() / SteerParams.Instance.SteeringForceTweaker);
                       vecForce.X = vecForce.X * objVehicle.Scale().X;
                       vecForce.Y = vecForce.Y * objVehicle.Scale().Y;

                       objGraphics.DrawLine(objRedPen, (PointF)objVehicle.Pos, (PointF)(objVehicle.Pos + vecForce));

                       objGraphics.DrawEllipse(objVehiclePen, (int)(objVehicle.Pos.X - SteerParams.Instance.ViewDistance), (int)(objVehicle.Pos.Y - SteerParams.Instance.ViewDistance),
                           (int)SteerParams.Instance.ViewDistance * 2, (int)SteerParams.Instance.ViewDistance * 2);
                   }
                   else if ((objVehicle.ID() == m_intVictimID) && GameWorld.Instance.SpacePartitioningOn && isPursuitOn())
                   {
                        InvertedAABBox2D box = new InvertedAABBox2D(objVehicle.Pos - new Vector2D(SteerParams.Instance.ViewDistance, SteerParams.Instance.ViewDistance),
                            objVehicle.Pos + new Vector2D(SteerParams.Instance.ViewDistance, SteerParams.Instance.ViewDistance));

                        renderBox(box, objGraphics, objTargetPen);

                        GameWorld.Instance.CellSpaces.CalculateNeighbors(objVehicle.Pos, SteerParams.Instance.ViewDistance);

                        foreach (MovingEntity objNeighbour in GameWorld.Instance.CellSpaces.ListOfNeighbours())
                        {
                            if (objNeighbour.ID() != m_intSharkieID)
                            {
                                RenderObstacle(objNeighbour, objGraphics, objGrayPen);
                            }
                        }

                        objGraphics.DrawEllipse(objGrayPen, (int)(objVehicle.Pos.X - SteerParams.Instance.ViewDistance), (int)(objVehicle.Pos.Y - SteerParams.Instance.ViewDistance),
                                        (int)SteerParams.Instance.ViewDistance * 2, (int)SteerParams.Instance.ViewDistance * 2);   
                   }
               }
            }

            if (GameWorld.Instance.SpacePartitioningOn)
            {
                foreach (Cell objCell in m_pCellSpace.ListOfCells())
                {
                    renderCell(objCell, objGraphics, objCellPen, true);
                }
            }

            foreach (Wall2D objWall in m_Walls)
            {
                RenderWall2D(objWall, objGraphics, objWallPen, true);
            }

            foreach (BaseGameEntity objObs in m_Obstacles)
            {
                RenderObstacle(objObs, objGraphics, objObstaclePen);
            }

            if (!isPursuitOn() && !isFollowPathOn())
            {
                RenderTarget(GameWorld.Instance.TargetPos, objGraphics, objTargetPen);
            }
        }

        public Vector2D getTarget()
        {
            return GameWorld.Instance.TargetPos;
        }

        public void setTarget(int x, int y)
        {
            GameWorld.Instance.TargetPos = new Vector2D(x, y);

            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            objSharkie.Steering().PursuitOff();
            objSharkie.Steering().FollowPathOff();

            objSharkie.Steering().ArriveOn();            
        }

        public void setFollowPathOn()
        {
            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            objSharkie.Steering().PursuitOff();
            objSharkie.Steering().ArriveOff();

            objSharkie.Steering().FollowPathOn();
        }

        public void setNextPursuitTarget()
        {
            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            objSharkie.Steering().ArriveOff();
            objSharkie.Steering().FollowPathOff();

            if (m_Vehicles.Count > 1)
            {
                bool blnSearch = true;

                while (blnSearch)
                {
                    int tempNum = Utils.RandInt(0, (m_Vehicles.Count - 1));

                    if (m_Vehicles[tempNum].ID() != m_intSharkieID)
                    {
                        m_intVictimID = m_Vehicles[tempNum].ID();
                        blnSearch = false;
                    }
                }

                MovingEntity objVictim = getVehicleByID(m_intVictimID);
                objSharkie.Steering().PursuitOn(objVictim);
            }
        }

        public bool isPursuitOn()
        {
            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            return objSharkie.Steering().isPursuitOn();
        }

        public bool isFollowPathOn()
        {
            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            return objSharkie.Steering().isFollowPathOn();
        }

        private MovingEntity getVehicleByID(int intID)
        {
            foreach (MovingEntity objVehicle in m_Vehicles)
            {
                if (objVehicle.ID() == intID) { return objVehicle; }
            }

            return null;
        }

        public void ReCreatePath()
        {
            CreatePath();
            MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
            objSharkie.Steering().GetPath().Set(m_pPath);
        }

        private void CreatePath()
        {
            m_pPath = new Path2D(true);

            List<Vector2D> listWayPoints = new List<Vector2D>();
	
	        int NumWaypoints = 8;
            double MinX = m_bordersize;
            double MinY = m_bordersize;
            double MaxX = m_cxClient - m_bordersize;
            double MaxY = m_cyClient - m_bordersize;

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

                bool bOverlapped = false;

                foreach (BaseGameEntity objObs in m_Obstacles)
                {
                    if (Utils.PointInCircle(objObs.Pos, objObs.BRadius, temp))
                    {
                        bOverlapped = true;
                        break;
                    }
                }

                if (!bOverlapped) listWayPoints.Add(temp);

            }

            m_pPath.Set(listWayPoints);
        }

        //--------------------------- CreateWalls --------------------------------
        //
        //  creates some walls that form an enclosure for the steering agents.
        //  used to demonstrate several of the steering behaviors
        //------------------------------------------------------------------------
        private void CreateWalls()
        {
            //create the walls  
            double CornerSize = 0.2;
            double vDist = m_cyClient - 2 * m_bordersize;
            double hDist = m_cxClient - 2 * m_bordersize;

            const int NumWallVerts = 8;

            Vector2D[] walls = new Vector2D[] {new Vector2D(hDist*CornerSize+m_bordersize, m_bordersize),
                                           new Vector2D(m_cxClient-m_bordersize-hDist*CornerSize, m_bordersize),
                                            new Vector2D(m_cxClient-m_bordersize, m_bordersize+vDist*CornerSize),
                                            new Vector2D(m_cxClient-m_bordersize, m_cyClient-m_bordersize-vDist*CornerSize),                                                 
                                            new Vector2D(m_cxClient-m_bordersize-hDist*CornerSize, m_cyClient-m_bordersize),
                                            new Vector2D(hDist*CornerSize+m_bordersize, m_cyClient-m_bordersize),
                                            new Vector2D(m_bordersize, m_cyClient-m_bordersize-vDist*CornerSize),
                                            new Vector2D(m_bordersize, m_bordersize+vDist*CornerSize)};

            for (int w = 0; w < NumWallVerts - 1; ++w)
            {
                m_Walls.Add(new Wall2D(walls[w], walls[w + 1]));
            }

            m_Walls.Add(new Wall2D(walls[NumWallVerts - 1], walls[0]));
        }

        //--------------------------- CreateObstacles -----------------------------
        //
        //  Sets up the vector of obstacles with random positions and sizes. Makes
        //  sure the obstacles do not overlap
        //------------------------------------------------------------------------
        private void CreateObstacles()
        {
            //create a number of randomly sized tiddlywinks
            for (int o = 0; o < SteerParams.Instance.NumObstacles; ++o)
            {
                bool bOverlapped = true;

                //keep creating tiddlywinks until we find one that doesn't overlap
                //any others.Sometimes this can get into an endless loop because the
                //obstacle has nowhere to fit. We test for this case and exit accordingly

                int NumTrys = 0; int NumAllowableTrys = 2000;

                while (bOverlapped)
                {
                    NumTrys++;

                    if (NumTrys > NumAllowableTrys) return;

                    int radius = Utils.RandInt((int)SteerParams.Instance.MinObstacleRadius, (int)SteerParams.Instance.MaxObstacleRadius);

                    const int border = 10;
                    const int MinGapBetweenObstacles = 20;

                    Vector2D randomPos = new Vector2D(Utils.RandInt(radius + border, m_cxClient - radius - border), Utils.RandInt(radius + border, m_cyClient - radius - 30 - border));

                    BaseGameEntity ob = new BaseGameEntity(0, randomPos, radius);

                    if (!Utils.Overlapped(ob, m_Obstacles, MinGapBetweenObstacles))
                    {
                        //its not overlapped so we can add it
                        m_Obstacles.Add(ob);

                        bOverlapped = false;
                    }

                    else
                    {
                        ob = null;
                    }
                }
            }
        }

        public bool UseWalls
        {
            get { return (m_Walls.Count > 0); }
            set 
            {
                if (value) { CreateWalls(); }
                else { m_Walls.Clear(); }
            }
        }

        public bool UseObstacles
        {
            get { return (m_Obstacles.Count > 0); }
            set
            {
                if (value) { CreateObstacles(); }
                else { m_Obstacles.Clear(); }
            }
        }

        public bool UseRenderAids
        {
            get { return m_blnRenderAids; }
            set { m_blnRenderAids = value; }
        }

        public bool UseSpacePartitioning
        {
            get { return GameWorld.Instance.SpacePartitioningOn; }
            set{ GameWorld.Instance.SpacePartitioningOn = value; }
        }

        public bool UseNonPenetration
        {
            get { return m_blnNonePenetrationOn; }
            set { m_blnNonePenetrationOn = value; }
        } 

        public bool UseSmoothing
        {
            get { return GameWorld.Instance.SmoothingOn; }
            set { GameWorld.Instance.SmoothingOn = value; }
        }

        public int SmoothingSamples
        {
            get { return SteerParams.Instance.NumSamplesForSmoothing; }
            set { SteerParams.Instance.NumSamplesForSmoothing = value; }
        }

        public double SharkMaxForce
        {
            get
            {
                MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
                return objSharkie.MaxForce / SteerParams.Instance.SteeringForceTweaker;
            }
            set
            {
                MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
                objSharkie.MaxForce = value * SteerParams.Instance.SteeringForceTweaker;
            }
        }

        public double SharkMaxSpeed
        {
            get 
            {
                MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
                return objSharkie.MaxSpeed / SteerParams.Instance.SteeringForceTweaker;            
            }
            set 
            {
                MovingEntity objSharkie = getVehicleByID(m_intSharkieID);
                objSharkie.MaxSpeed = value * SteerParams.Instance.SteeringForceTweaker;
            }
        }

        public double SeparationWeight
        {
            get { return SteerParams.Instance.SeparationWeight; }
            set 
            {
                SteerParams.Instance.SeparationWeight = value;

                foreach (MovingEntity objVehicle in m_Vehicles)
                {
                    objVehicle.Steering().SeparationWeight = SteerParams.Instance.AppliedSeparationWeight();
                }
            }
        }

        public double AlignmentWeight
        {
            get { return SteerParams.Instance.AlignmentWeight; }
            set 
            { 
                SteerParams.Instance.AlignmentWeight = value;

                foreach (MovingEntity objVehicle in m_Vehicles)
                {
                    objVehicle.Steering().AlignmentWeight = SteerParams.Instance.AppliedAlignmentWeight();
                }
            }
        }

        public double CohesionWeight
        {
            get { return SteerParams.Instance.CohesionWeight; }
            set 
            { 
                SteerParams.Instance.CohesionWeight = value;

                foreach (MovingEntity objVehicle in m_Vehicles)
                {
                    objVehicle.Steering().CohesionWeight = SteerParams.Instance.AppliedCohesionWeight();
                }
            }
        }

        private void RenderTarget(Vector2D objVector2D, Graphics objGraphics, Pen objTargetPen)
        {
            objGraphics.DrawLine(objTargetPen, (int)(objVector2D.X - 4.0), (int)(objVector2D.Y - 4.0), (int)(objVector2D.X + 4.0), (int)(objVector2D.Y + 4.0));
            objGraphics.DrawLine(objTargetPen, (int)(objVector2D.X + 4.0), (int)(objVector2D.Y - 4.0), (int)(objVector2D.X - 4.0), (int)(objVector2D.Y + 4.0));
        }

        private void renderVehicle(MovingEntity objVehicle, Graphics objGraphics, Pen objPen)
        {
            PointF pntLeft,pntFront,pntRight;

            if (UseSmoothing)
            {
                Vector2D SmoothedPerp = objVehicle.SmoothedHeading().Perp();

                pntLeft = (PointF)(objVehicle.Pos + (SmoothedPerp * objVehicle.BRadius));
                pntFront = (PointF)(objVehicle.Pos + (objVehicle.SmoothedHeading() * (objVehicle.BRadius * 2)));
                pntRight = (PointF)(objVehicle.Pos - (SmoothedPerp * objVehicle.BRadius));
            }
            else
            {
                 pntLeft = (PointF)(objVehicle.Pos + (objVehicle.Side() * objVehicle.BRadius));
                 pntFront = (PointF)(objVehicle.Pos + (objVehicle.Heading() * (objVehicle.BRadius * 2)));
                 pntRight = (PointF)(objVehicle.Pos - (objVehicle.Side() * objVehicle.BRadius));
            }

            PointF[] points = new PointF[4];

            points[0] = pntLeft;
            points[1] = pntFront;
            points[2] = pntRight;
            points[3] = pntLeft;

            objGraphics.DrawPolygon(objPen, points);
        }

        private void renderCell(Cell objCell, Graphics objGraphics, Pen objPen, bool showStats)
        {
            renderBox(objCell.BBox, objGraphics, objPen);

            if (showStats)
            {
                    objGraphics.DrawString(objCell.Members.Count.ToString(), mFont, Brushes.Black, (float)objCell.BBox.Left(), (float)objCell.BBox.Top());
            }
        }

        private void renderBox(InvertedAABBox2D objBox, Graphics objGraphics, Pen objPen)
        {
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Top(), (int)objBox.Right(), (int)objBox.Top());
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Bottom(), (int)objBox.Right(), (int)objBox.Bottom());
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Top(), (int)objBox.Left(), (int)objBox.Bottom());
            objGraphics.DrawLine(objPen, (int)objBox.Right(), (int)objBox.Top(), (int)objBox.Right(), (int)objBox.Bottom());
        }

        private void RenderWall2D(Wall2D objWall2D, Graphics objGraphics, Pen objPen, bool RenderNormals)
        {
            objGraphics.DrawLine(objPen, (PointF)objWall2D.From, (PointF)objWall2D.To);

            //render the normals if rqd
            if (RenderNormals)
            {
                Vector2D middle = objWall2D.Center();
                objGraphics.DrawLine(objPen, (int)middle.X, (int)middle.Y, (int)(middle.X + (objWall2D.Normal.X * 10)), (int)(middle.Y + (objWall2D.Normal.Y * 10)));
            }
        }

        private void RenderObstacle(BaseGameEntity objObstacle, Graphics objGraphics, Pen objPen)
        {
            objGraphics.DrawEllipse(objPen, (int)(objObstacle.Pos.X - objObstacle.BRadius), (int)(objObstacle.Pos.Y - objObstacle.BRadius),
                (int)objObstacle.BRadius * 2, (int)objObstacle.BRadius * 2);            
        }

        private void RenderPath2D(Path2D objPath2D, Graphics objGraphics, Pen objPen)
        {
            if (objPath2D.GetPath().Count > 1)
            {
                IEnumerator<Vector2D> nextVec = objPath2D.GetPath().GetEnumerator();
                nextVec.MoveNext();

                int intCount = 0;

                foreach (Vector2D objVec in objPath2D.GetPath())
                {
                    intCount = intCount + 1;

                    if (!nextVec.MoveNext() && objPath2D.Loop)
                    {
                        nextVec.Reset();
                        nextVec.MoveNext();           
                    }

                    if (Vector2D.IsNull(nextVec.Current)){ break;}

                    objGraphics.DrawLine(objPen, (PointF)objVec, (PointF)nextVec.Current);
                    objGraphics.DrawString(intCount.ToString(), mFont, Brushes.YellowGreen, (float)objVec.X, (float)objVec.Y);

                }
            }
        }

    }
}
