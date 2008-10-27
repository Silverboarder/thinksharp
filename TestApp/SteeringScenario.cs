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

        private Pen objVehiclePen, objWallPen, objObstaclePen, objTargetPen;

        public SteeringScenario(int cx, int cy)
        {
            m_cxClient = cx;
            m_cyClient = cy;

            GameWorld.Instance.cxClient = m_cxClient;
            GameWorld.Instance.cyClient = m_cyClient;

            objVehiclePen = new Pen(Color.Black);
            objWallPen = new Pen(Color.Orange);
            objObstaclePen = new Pen(Color.Red);
            objTargetPen = new Pen(Color.DarkGreen, 2);

            m_pCellSpace = new CellSpacePartition(m_cxClient, m_cyClient, SteerParams.Instance.NumCellsX, SteerParams.Instance.NumCellsY, SteerParams.Instance.NumAgents);

            double border = 50;
            m_pPath = new Path2D(5, border, border, cx - border, cy - border, true);

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
                                  SteerParams.Instance.MaxSteeringForce);

                //GameWorld.Instance.TargetPos = new Vector2D(200, 400);

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
            //m_Vehicles[sharkie].Steering().WallAvoidanceOff();
            //m_Vehicles[sharkie].Steering().ObstacleAvoidanceOff();
            m_Vehicles[sharkie].Steering().SeekOn();
            m_Vehicles[sharkie].SetScale(new Vector2D(10, 10));            
            m_Vehicles[sharkie].MaxSpeed = 70;

            for (int a = 0; a < SteerParams.Instance.NumAgents - 1; ++a)
            {
                m_Vehicles[a].Steering().EvadeOn(m_Vehicles[sharkie]);
            }

            CreateWalls();
            CreateObstacles();

            GameWorld.Instance.Walls = m_Walls;
            GameWorld.Instance.CellSpaces = m_pCellSpace;
            GameWorld.Instance.Obstacles = m_Obstacles;
            GameWorld.Instance.Agents = m_Vehicles;

            GameWorld.Instance.Wrap = true;
        }

        public void Update(double time_elapsed)
        {
            GameWorld.Instance.Update(time_elapsed);
            //EnforceNonPenetrationConstraint(this, World()->Agents());
        }

        public void Render(Graphics objGraphics)
        {
            foreach (MovingEntity objVehicle in m_Vehicles)
            {
                renderVehicle(objVehicle, objGraphics, objVehiclePen);
            }

            foreach (Wall2D objWall in m_Walls)
            {
                RenderWall2D(objWall, objGraphics, objWallPen, true);
            }

            foreach (BaseGameEntity objObs in m_Obstacles)
            {
                RenderObstacle(objObs, objGraphics, objObstaclePen);
            }            

            if (!GameWorld.Instance.TargetPos.isZero())
            {
                RenderTarget(GameWorld.Instance.TargetPos, objGraphics, objTargetPen);
            }
        }

        public void setTarget(int x, int y)
        {
            GameWorld.Instance.TargetPos = new Vector2D(x, y);
        }

        //--------------------------- CreateWalls --------------------------------
        //
        //  creates some walls that form an enclosure for the steering agents.
        //  used to demonstrate several of the steering behaviors
        //------------------------------------------------------------------------
        private void CreateWalls()
        {
            //create the walls  
            double bordersize = 20.0;
            double CornerSize = 0.2;
            double vDist = m_cyClient - 2 * bordersize;
            double hDist = m_cxClient - 2 * bordersize;

            const int NumWallVerts = 8;

            Vector2D[] walls = new Vector2D[] {new Vector2D(hDist*CornerSize+bordersize, bordersize),
                                           new Vector2D(m_cxClient-bordersize-hDist*CornerSize, bordersize),
                                            new Vector2D(m_cxClient-bordersize, bordersize+vDist*CornerSize),
                                            new Vector2D(m_cxClient-bordersize, m_cyClient-bordersize-vDist*CornerSize),                                                 
                                            new Vector2D(m_cxClient-bordersize-hDist*CornerSize, m_cyClient-bordersize),
                                            new Vector2D(hDist*CornerSize+bordersize, m_cyClient-bordersize),
                                            new Vector2D(bordersize, m_cyClient-bordersize-vDist*CornerSize),
                                            new Vector2D(bordersize, bordersize+vDist*CornerSize)};

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

        private void RenderTarget(Vector2D objVector2D, Graphics objGraphics, Pen objTargetPen)
        {
            objGraphics.DrawLine(objTargetPen, (int)(objVector2D.X - 4.0), (int)(objVector2D.Y - 4.0), (int)(objVector2D.X + 4.0), (int)(objVector2D.Y + 4.0));
            objGraphics.DrawLine(objTargetPen, (int)(objVector2D.X + 4.0), (int)(objVector2D.Y - 4.0), (int)(objVector2D.X - 4.0), (int)(objVector2D.Y + 4.0));
        }

        private void renderVehicle(MovingEntity objVehicle, Graphics objGraphics, Pen objPen)
        {
            PointF pntLeft = (PointF)(objVehicle.Pos + (objVehicle.Side() * objVehicle.BRadius));
            PointF pntFront = (PointF)(objVehicle.Pos + (objVehicle.Heading() * (objVehicle.BRadius * 2)));
            PointF pntRight = (PointF)(objVehicle.Pos - (objVehicle.Side() * objVehicle.BRadius));

            PointF[] points = new PointF[4];

            points[0] = pntLeft;
            points[1] = pntFront;
            points[2] = pntRight;
            points[3] = pntLeft;

            objGraphics.DrawPolygon(objPen, points);
        }

        private void renderIAABBox2D(InvertedAABBox2D objBox, Graphics objGraphics, Pen objPen, bool RenderCenter)
        {
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Top(), (int)objBox.Right(), (int)objBox.Top());
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Bottom(), (int)objBox.Right(), (int)objBox.Bottom());
            objGraphics.DrawLine(objPen, (int)objBox.Left(), (int)objBox.Top(), (int)objBox.Left(), (int)objBox.Bottom());
            objGraphics.DrawLine(objPen, (int)objBox.Right(), (int)objBox.Top(), (int)objBox.Right(), (int)objBox.Bottom());

            if (RenderCenter)
            {
                objGraphics.DrawEllipse(objPen, objBox.Rect());
            }
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

            //objGraphics.DrawString(objObstacle.BRadius.ToString(), new Font(FontFamily.GenericSerif, 10), Brushes.Blue, (PointF) objObstacle.Pos);
        }

        private void RenderPath2D(Path2D objPath2D, Graphics objGraphics, Pen objPen)
        {
            if (objPath2D.GetPath().Count > 1)
            {
                IEnumerator<Vector2D> nextVec = objPath2D.GetPath().GetEnumerator();
                nextVec.MoveNext();
                nextVec.MoveNext();

                foreach (Vector2D objVec in objPath2D.GetPath())
                {
                    objGraphics.DrawLine(objPen, (PointF)objVec, (PointF)nextVec.Current);

                    if (!nextVec.MoveNext())
                    {
                        if (objPath2D.Loop)
                        {
                            nextVec.Reset();
                            nextVec.MoveNext();

                            objGraphics.DrawLine(objPen, (PointF)objVec, (PointF)nextVec.Current);
                        }

                        break;
                    }
                }
            }
        }

        //
    }
}
