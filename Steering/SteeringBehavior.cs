using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using ThinkSharp.Common;

namespace ThinkSharp.Steering
{
    public class SteeringBehavior
    {
        //the radius of the constraining circle for the wander behavior
        const double WanderRad = 1.2;
        //distance the wander circle is projected in front of the agent
        const double WanderDist = 2.0;
        //the maximum amount of displacement along the circle each frame
        const double WanderJitterPerSec = 80.0;
        
        const double WaypointSeekDist = 3;

        public enum summing_method { weighted_average, prioritized, dithered };

        private enum behavior_type
        {
            none = 0x00000,
            seek = 0x00002,
            flee = 0x00004,
            arrive = 0x00008,
            wander = 0x00010,
            cohesion = 0x00020,
            separation = 0x00040,
            allignment = 0x00080,
            obstacle_avoidance = 0x00100,
            wall_avoidance = 0x00200,
            follow_path = 0x00400,
            pursuit = 0x00800,
            evade = 0x01000,
            interpose = 0x02000,
            hide = 0x04000,
            flock = 0x08000,
            offset_pursuit = 0x10000,
        };

        //a pointer to the owner of this instance
        private MovingEntity m_parentMovingEntity;

        //the steering force created by the combined effect of all the selected behaviors
        private Vector2D m_vSteeringForce;

        //these can be used to keep track of friends, pursuers, or prey
        private MovingEntity m_pTargetAgent1;
        private MovingEntity m_pTargetAgent2;

        //length of the 'detection box' utilized in obstacle avoidance
        private double m_dDBoxLength;

        //a vertex buffer to contain the feelers rqd for wall avoidance  
        private List<Vector2D> m_Feelers;

        //the length of the 'feeler/s' used in wall detection
        private double m_dWallDetectionFeelerLength;

        //the current position on the wander circle the agent is attempting to steer towards
        private Vector2D m_vWanderTarget;

        //explained above
        private double m_dWanderJitter;
        private double m_dWanderRadius;
        private double m_dWanderDistance;

        //multipliers. These can be adjusted to effect strength of the  
        //appropriate behavior. Useful to get flocking the way you require for example.
        private double m_dWeightSeparation;
        private double m_dWeightCohesion;
        private double m_dWeightAlignment;
        private double m_dWeightWander;
        private double m_dWeightObstacleAvoidance;
        private double m_dWeightWallAvoidance;
        private double m_dWeightSeek;
        private double m_dWeightFlee;
        private double m_dWeightArrive;
        private double m_dWeightPursuit;
        private double m_dWeightOffsetPursuit;
        private double m_dWeightInterpose;
        private double m_dWeightHide;
        private double m_dWeightEvade;
        private double m_dWeightFollowPath;

        //how far the agent can 'see'
        private double m_dViewDistance;

        //pointer to any current path
        private Path2D m_pPath;

        //the distance (squared) a vehicle has to be from a path waypoint before
        //it starts seeking to the next waypoint
        private double m_dWaypointSeekDistSq;

        //any offset used for formations or offset pursuit
        private Vector2D m_vOffset;

        //binary flags to indicate whether or not a behavior should be active
        private int m_iFlags;

        //Arrive makes use of these to determine how quickly a vehicle
        //should decelerate to its target
        private enum Deceleration { slow = 3, normal = 2, fast = 1 };

        //default
        private Deceleration m_Deceleration;

        //what type of method is used to sum any active behavior
        private summing_method m_SummingMethod;

        //this function tests if a specific bit of m_iFlags is set
        private bool On(behavior_type bt) { return (m_iFlags & (int)bt) == (int)bt; }

        public SteeringBehavior(MovingEntity agent)
        {
            m_parentMovingEntity = agent;
            m_iFlags = 0;
            m_dDBoxLength = SteerParams.Instance.MinDetectionBoxLength;
            m_dWeightCohesion = SteerParams.Instance.AppliedCohesionWeight();
            m_dWeightAlignment = SteerParams.Instance.AppliedAlignmentWeight();
            m_dWeightSeparation = SteerParams.Instance.AppliedSeparationWeight();
            m_dWeightObstacleAvoidance = SteerParams.Instance.AppliedObstacleAvoidanceWeight();
            m_dWeightWander = SteerParams.Instance.AppliedWanderWeight();
            m_dWeightWallAvoidance = SteerParams.Instance.AppliedWallAvoidanceWeight();
            m_dViewDistance = SteerParams.Instance.ViewDistance;
            m_dWallDetectionFeelerLength = SteerParams.Instance.WallDetectionFeelerLength;
            m_Feelers = new List<Vector2D>(3);
            m_Deceleration = Deceleration.normal;
            m_pTargetAgent1 = null;
            m_pTargetAgent2 = null;
            m_dWanderDistance = WanderDist;
            m_dWanderJitter = WanderJitterPerSec;
            m_dWanderRadius = WanderRad;
            m_dWaypointSeekDistSq = WaypointSeekDist * WaypointSeekDist;
            m_dWeightSeek = SteerParams.Instance.AppliedSeekWeight();
            m_dWeightFlee = SteerParams.Instance.AppliedFleeWeight();
            m_dWeightArrive = SteerParams.Instance.AppliedArriveWeight();
            m_dWeightPursuit = SteerParams.Instance.AppliedPursuitWeight();
            m_dWeightOffsetPursuit = SteerParams.Instance.AppliedOffsetPursuitWeight();
            m_dWeightInterpose = SteerParams.Instance.AppliedInterposeWeight();
            m_dWeightHide = SteerParams.Instance.AppliedHideWeight();
            m_dWeightEvade = SteerParams.Instance.AppliedEvadeWeight();
            m_dWeightFollowPath = SteerParams.Instance.AppliedFollowPathWeight();
            m_SummingMethod = summing_method.prioritized;

            //stuff for the wander behavior
            double theta = Utils.RandFloat() * Utils.TwoPi;

            //create a vector to a target position on the wander circle
            m_vWanderTarget = new Vector2D(m_dWanderRadius * Math.Cos(theta), m_dWanderRadius * Math.Sin(theta));

            //create a Path
            m_pPath = new Path2D(true);

            m_vSteeringForce = new Vector2D();

        }

        public void SetTargetAgent1(MovingEntity Agent) { m_pTargetAgent1 = Agent; }
        public void SetTargetAgent2(MovingEntity Agent) { m_pTargetAgent2 = Agent; }

        public void SetOffset(Vector2D offset) { m_vOffset = offset; }
        public Vector2D GetOffset() { return m_vOffset; }

        public void SetPath(List<Vector2D> new_path) { m_pPath.Set(new_path); }
        public Path2D GetPath() { return m_pPath; }

        public void CreateRandomPath(int num_waypoints, int mx, int my, int cx, int cy)
        { m_pPath.CreateRandomPath(num_waypoints, mx, my, cx, cy); }

        public Vector2D Force() { return m_vSteeringForce; }

        public void SetSummingMethod(summing_method sm) { m_SummingMethod = sm; }

        public void FleeOn() { m_iFlags |= (int)behavior_type.flee; }
        public void SeekOn() { m_iFlags |= (int)behavior_type.seek; }
        public void ArriveOn() { m_iFlags |= (int)behavior_type.arrive; }
        public void WanderOn() { m_iFlags |= (int)behavior_type.wander; }
        public void PursuitOn(MovingEntity v) { m_iFlags |= (int)behavior_type.pursuit; m_pTargetAgent1 = v; }
        public void EvadeOn(MovingEntity v) { m_iFlags |= (int)behavior_type.evade; m_pTargetAgent1 = v; }
        public void CohesionOn() { m_iFlags |= (int)behavior_type.cohesion; }
        public void SeparationOn() { m_iFlags |= (int)behavior_type.separation; }
        public void AlignmentOn() { m_iFlags |= (int)behavior_type.allignment; }
        public void ObstacleAvoidanceOn() { m_iFlags |= (int)behavior_type.obstacle_avoidance; }
        public void WallAvoidanceOn() { m_iFlags |= (int)behavior_type.wall_avoidance; }
        public void FollowPathOn() { m_iFlags |= (int)behavior_type.follow_path; }
        public void InterposeOn(MovingEntity v1, MovingEntity v2) { m_iFlags |= (int)behavior_type.interpose; m_pTargetAgent1 = v1; m_pTargetAgent2 = v2; }
        public void HideOn(MovingEntity v) { m_iFlags |= (int)behavior_type.hide; m_pTargetAgent1 = v; }
        public void OffsetPursuitOn(MovingEntity v1, Vector2D offset) { m_iFlags |= (int)behavior_type.offset_pursuit; m_vOffset = offset; m_pTargetAgent1 = v1; }
        public void FlockingOn() { CohesionOn(); AlignmentOn(); SeparationOn(); WanderOn(); }

        public void FleeOff() { if (On(behavior_type.flee))   m_iFlags ^= (int)behavior_type.flee; }
        public void SeekOff() { if (On(behavior_type.seek))   m_iFlags ^= (int)behavior_type.seek; }
        public void ArriveOff() { if (On(behavior_type.arrive)) m_iFlags ^= (int)behavior_type.arrive; }
        public void WanderOff() { if (On(behavior_type.wander)) m_iFlags ^= (int)behavior_type.wander; }
        public void PursuitOff() { if (On(behavior_type.pursuit)) m_iFlags ^= (int)behavior_type.pursuit; }
        public void EvadeOff() { if (On(behavior_type.evade)) m_iFlags ^= (int)behavior_type.evade; }
        public void CohesionOff() { if (On(behavior_type.cohesion)) m_iFlags ^= (int)behavior_type.cohesion; }
        public void SeparationOff() { if (On(behavior_type.separation)) m_iFlags ^= (int)behavior_type.separation; }
        public void AlignmentOff() { if (On(behavior_type.allignment)) m_iFlags ^= (int)behavior_type.allignment; }
        public void ObstacleAvoidanceOff() { if (On(behavior_type.obstacle_avoidance)) m_iFlags ^= (int)behavior_type.obstacle_avoidance; }
        public void WallAvoidanceOff() { if (On(behavior_type.wall_avoidance)) m_iFlags ^= (int)behavior_type.wall_avoidance; }
        public void FollowPathOff() { if (On(behavior_type.follow_path)) m_iFlags ^= (int)behavior_type.follow_path; }
        public void InterposeOff() { if (On(behavior_type.interpose)) m_iFlags ^= (int)behavior_type.interpose; }
        public void HideOff() { if (On(behavior_type.hide)) m_iFlags ^= (int)behavior_type.hide; }
        public void OffsetPursuitOff() { if (On(behavior_type.offset_pursuit)) m_iFlags ^= (int)behavior_type.offset_pursuit; }
        public void FlockingOff() { CohesionOff(); AlignmentOff(); SeparationOff(); WanderOff(); }

        public bool isFleeOn() { return On(behavior_type.flee); }
        public bool isSeekOn() { return On(behavior_type.seek); }
        public bool isArriveOn() { return On(behavior_type.arrive); }
        public bool isWanderOn() { return On(behavior_type.wander); }
        public bool isPursuitOn() { return On(behavior_type.pursuit); }
        public bool isEvadeOn() { return On(behavior_type.evade); }
        public bool isCohesionOn() { return On(behavior_type.cohesion); }
        public bool isSeparationOn() { return On(behavior_type.separation); }
        public bool isAlignmentOn() { return On(behavior_type.allignment); }
        public bool isObstacleAvoidanceOn() { return On(behavior_type.obstacle_avoidance); }
        public bool isWallAvoidanceOn() { return On(behavior_type.wall_avoidance); }
        public bool isFollowPathOn() { return On(behavior_type.follow_path); }
        public bool isInterposeOn() { return On(behavior_type.interpose); }
        public bool isHideOn() { return On(behavior_type.hide); }
        public bool isOffsetPursuitOn() { return On(behavior_type.offset_pursuit); }

        public double DBoxLength() { return m_dDBoxLength; }
        public List<Vector2D> GetFeelers() { return m_Feelers; }

        public double WanderJitter() { return m_dWanderJitter; }
        public double WanderDistance() { return m_dWanderDistance; }
        public double WanderRadius() { return m_dWanderRadius; }

        public double SeparationWeight
        {
            get { return m_dWeightSeparation; }
            set { m_dWeightSeparation = value; }
        }

        public double AlignmentWeight
        {
            get { return m_dWeightAlignment; }
            set { m_dWeightAlignment = value; }
        }

        public double CohesionWeight
        {
            get { return m_dWeightCohesion; }
            set { m_dWeightCohesion = value; }
        }

        //----------------------- Calculate --------------------------------------
        //
        //  calculates the accumulated steering force according to the method set
        //  in m_SummingMethod
        //------------------------------------------------------------------------
        public Vector2D Calculate()
        {
            //reset the steering force
            m_vSteeringForce.Zero();

            //use space partitioning to calculate the neighbours of this vehicle
            //if switched on. If not, use the standard tagging system
            if (!GameWorld.Instance.SpacePartitioningOn)
            {
                //tag neighbors if any of the following 3 group behaviors are switched on
                if (On(behavior_type.separation) || On(behavior_type.allignment) || On(behavior_type.cohesion))
                {
                    GameWorld.Instance.TagAgentsWithinViewRange(m_parentMovingEntity, m_dViewDistance);
                }
            }
            else
            {
                //calculate neighbours in cell-space if any of the following 3 group
                //behaviors are switched on
                if (On(behavior_type.separation) || On(behavior_type.allignment) || On(behavior_type.cohesion))
                {
                    GameWorld.Instance.CellSpaces.CalculateNeighbors(m_parentMovingEntity.Pos, m_dViewDistance);
                }
            }

            switch (m_SummingMethod)
            {
                case summing_method.weighted_average:

                    m_vSteeringForce = CalculateWeightedSum(); break;

                case summing_method.prioritized:

                    m_vSteeringForce = CalculatePrioritized(); break;

                case summing_method.dithered:

                    m_vSteeringForce = CalculateDithered(); break;

                default:

                    m_vSteeringForce = new Vector2D(0, 0); break;

            }//end switch

            return m_vSteeringForce;
        }

        //------------------------- ForwardComponent -----------------------------
        //
        //  returns the forward oomponent of the steering force
        //------------------------------------------------------------------------
        public double ForwardComponent()
        {
            return m_parentMovingEntity.Heading().Dot(m_vSteeringForce);
        }

        //--------------------------- SideComponent ------------------------------
        //  returns the side component of the steering force
        //------------------------------------------------------------------------
        public double SideComponent()
        {
            return m_parentMovingEntity.Side().Dot(m_vSteeringForce);
        }

        //--------------------- AccumulateForce ----------------------------------
        //
        //  This function calculates how much of its max steering force the 
        //  vehicle has left to apply and then applies that amount of the
        //  force to add.
        //------------------------------------------------------------------------
        public bool AccumulateForce(ref Vector2D RunningTot, Vector2D ForceToAdd)
        {

            //calculate how much steering force the vehicle has used so far
            double MagnitudeSoFar = RunningTot.Length();

            //calculate how much steering force remains to be used by this vehicle
            double MagnitudeRemaining = m_parentMovingEntity.MaxForce - MagnitudeSoFar;

            //return false if there is no more force left to use
            if (MagnitudeRemaining <= 0.0) return false;

            //calculate the magnitude of the force we want to add
            double MagnitudeToAdd = ForceToAdd.Length();

            //if the magnitude of the sum of ForceToAdd and the running total
            //does not exceed the maximum force available to this vehicle, just
            //add together. Otherwise add as much of the ForceToAdd vector is
            //possible without going over the max.
            if (MagnitudeToAdd < MagnitudeRemaining)
            {
                RunningTot += ForceToAdd;
            }
            else
            {
                //add it to the steering force
                RunningTot += (Vector2D.Vec2DNormalize(ForceToAdd) * MagnitudeRemaining);
            }

            return true;
        }


        //---------------------- CalculatePrioritized ----------------------------
        //
        //  this method calls each active steering behavior in order of priority
        //  and acumulates their forces until the max steering force magnitude
        //  is reached, at which time the function returns the steering force 
        //  accumulated to that  point
        //------------------------------------------------------------------------
        public Vector2D CalculatePrioritized()
        {
            Vector2D force;

            if (On(behavior_type.wall_avoidance))
            {
                force = WallAvoidance(GameWorld.Instance.Walls) * m_dWeightWallAvoidance;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.obstacle_avoidance))
            {
                force = ObstacleAvoidance(GameWorld.Instance.Obstacles) * m_dWeightObstacleAvoidance;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.evade))
            {
                Debug.Assert(m_pTargetAgent1 != null, "Evade target not assigned");

                force = Evade(m_pTargetAgent1) * m_dWeightEvade;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.flee))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");

                force = Flee(GameWorld.Instance.TargetPos) * m_dWeightFlee;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            //these next three can be combined for flocking behavior (wander is
            //also a good behavior to add into this mix)
            if (!GameWorld.Instance.SpacePartitioningOn)
            {
                if (On(behavior_type.separation))
                {
                    force = Separation(GameWorld.Instance.Agents) * m_dWeightSeparation;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(behavior_type.allignment))
                {
                    force = Alignment(GameWorld.Instance.Agents) * m_dWeightAlignment;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(behavior_type.cohesion))
                {
                    force = Cohesion(GameWorld.Instance.Agents) * m_dWeightCohesion;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }
            }

            else
            {

                if (On(behavior_type.separation))
                {
                    force = SeparationPlus(GameWorld.Instance.Agents) * m_dWeightSeparation;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(behavior_type.allignment))
                {
                    force = AlignmentPlus(GameWorld.Instance.Agents) * m_dWeightAlignment;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }

                if (On(behavior_type.cohesion))
                {
                    force = CohesionPlus(GameWorld.Instance.Agents) * m_dWeightCohesion;

                    if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
                }
            }

            if (On(behavior_type.seek))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");

                force = Seek(GameWorld.Instance.TargetPos) * m_dWeightSeek;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }


            if (On(behavior_type.arrive))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");

                force = Arrive(GameWorld.Instance.TargetPos, (int)m_Deceleration) * m_dWeightArrive;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.wander))
            {
                force = Wander() * m_dWeightWander;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.pursuit))
            {
                Debug.Assert(m_pTargetAgent1 != null, "pursuit target not assigned");

                force = Pursuit(m_pTargetAgent1) * m_dWeightPursuit;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.offset_pursuit))
            {
                Debug.Assert(m_pTargetAgent1 != null, "pursuit target not assigned");
                Debug.Assert(!Vector2D.IsNull(m_vOffset), "No offset assigned");

                force = OffsetPursuit(m_pTargetAgent1, m_vOffset);

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.interpose))
            {
                Debug.Assert(m_pTargetAgent1 != null && m_pTargetAgent2 != null, "Interpose agents not assigned");

                force = Interpose(m_pTargetAgent1, m_pTargetAgent2) * m_dWeightInterpose;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.hide))
            {
                Debug.Assert(m_pTargetAgent1 != null, "Hide target not assigned");

                force = Hide(m_pTargetAgent1, GameWorld.Instance.Obstacles) * m_dWeightHide;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            if (On(behavior_type.follow_path))
            {
                force = FollowPath() * m_dWeightFollowPath;

                if (!AccumulateForce(ref m_vSteeringForce, force)) return m_vSteeringForce;
            }

            return m_vSteeringForce;
        }


        //---------------------- CalculateWeightedSum ----------------------------
        //
        //  this simply sums up all the active behaviors X their weights and 
        //  truncates the result to the max available steering force before 
        //  returning
        //------------------------------------------------------------------------
        public Vector2D CalculateWeightedSum()
        {
            if (On(behavior_type.wall_avoidance))
            {
                m_vSteeringForce += WallAvoidance(GameWorld.Instance.Walls) * m_dWeightWallAvoidance;
            }

            if (On(behavior_type.obstacle_avoidance))
            {
                m_vSteeringForce += ObstacleAvoidance(GameWorld.Instance.Obstacles) * m_dWeightObstacleAvoidance;
            }

            if (On(behavior_type.evade))
            {
                Debug.Assert(m_pTargetAgent1 != null, "Evade target not assigned");

                m_vSteeringForce += Evade(m_pTargetAgent1) * m_dWeightEvade;
            }

            //these next three can be combined for flocking behavior (wander is
            //also a good behavior to add into this mix)
            if (!GameWorld.Instance.SpacePartitioningOn)
            {
                if (On(behavior_type.separation))
                {
                    m_vSteeringForce += Separation(GameWorld.Instance.Agents) * m_dWeightSeparation;
                }

                if (On(behavior_type.allignment))
                {
                    m_vSteeringForce += Alignment(GameWorld.Instance.Agents) * m_dWeightAlignment;
                }

                if (On(behavior_type.cohesion))
                {
                    m_vSteeringForce += Cohesion(GameWorld.Instance.Agents) * m_dWeightCohesion;
                }
            }
            else
            {
                if (On(behavior_type.separation))
                {
                    m_vSteeringForce += SeparationPlus(GameWorld.Instance.Agents) * m_dWeightSeparation;
                }

                if (On(behavior_type.allignment))
                {
                    m_vSteeringForce += AlignmentPlus(GameWorld.Instance.Agents) * m_dWeightAlignment;
                }

                if (On(behavior_type.cohesion))
                {
                    m_vSteeringForce += CohesionPlus(GameWorld.Instance.Agents) * m_dWeightCohesion;
                }
            }

            if (On(behavior_type.wander))
            {
                m_vSteeringForce += Wander() * m_dWeightWander;
            }

            if (On(behavior_type.seek))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Seek(GameWorld.Instance.TargetPos) * m_dWeightSeek;
            }

            if (On(behavior_type.flee))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Flee(GameWorld.Instance.TargetPos) * m_dWeightFlee;
            }

            if (On(behavior_type.arrive))
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Arrive(GameWorld.Instance.TargetPos, (int)m_Deceleration) * m_dWeightArrive;
            }

            if (On(behavior_type.pursuit))
            {
                Debug.Assert(m_pTargetAgent1 != null, "pursuit target not assigned");

                m_vSteeringForce += Pursuit(m_pTargetAgent1) * m_dWeightPursuit;
            }

            if (On(behavior_type.offset_pursuit))
            {
                Debug.Assert(m_pTargetAgent1 != null, "pursuit target not assigned");
                Debug.Assert(!Vector2D.IsNull(m_vOffset), "No offset assigned");

                m_vSteeringForce += OffsetPursuit(m_pTargetAgent1, m_vOffset) * m_dWeightOffsetPursuit;
            }

            if (On(behavior_type.interpose))
            {
                Debug.Assert(m_pTargetAgent1 != null && m_pTargetAgent2 != null, "Interpose agents not assigned");

                m_vSteeringForce += Interpose(m_pTargetAgent1, m_pTargetAgent2) * m_dWeightInterpose;
            }

            if (On(behavior_type.hide))
            {
                Debug.Assert(m_pTargetAgent1 != null, "Hide target not assigned");

                m_vSteeringForce += Hide(m_pTargetAgent1, GameWorld.Instance.Obstacles) * m_dWeightHide;
            }

            if (On(behavior_type.follow_path))
            {
                m_vSteeringForce += FollowPath() * m_dWeightFollowPath;
            }

            m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

            return m_vSteeringForce;
        }


        //---------------------- CalculateDithered ----------------------------
        //
        //  this method sums up the active behaviors by assigning a probabilty
        //  of being calculated to each behavior. It then tests the first priority
        //  to see if it should be calcukated this simulation-step. If so, it
        //  calculates the steering force resulting from this behavior. If it is
        //  more than zero it returns the force. If zero, or if the behavior is
        //  skipped it continues onto the next priority, and so on.
        //
        //  NOTE: Not all of the behaviors have been implemented in this method,
        //        just a few, so you get the general idea
        //------------------------------------------------------------------------
        public Vector2D CalculateDithered()
        {
            //reset the steering force
            m_vSteeringForce.Zero();

            if (On(behavior_type.wall_avoidance) && Utils.RandFloat() < SteerParams.Instance.PrWallAvoidance)
            {
                m_vSteeringForce = WallAvoidance(GameWorld.Instance.Walls) *
                                     m_dWeightWallAvoidance / SteerParams.Instance.PrWallAvoidance;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            if (On(behavior_type.obstacle_avoidance) && Utils.RandFloat() < SteerParams.Instance.PrObstacleAvoidance)
            {
                m_vSteeringForce += ObstacleAvoidance(GameWorld.Instance.Obstacles) *
                        m_dWeightObstacleAvoidance / SteerParams.Instance.PrObstacleAvoidance;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            if (!GameWorld.Instance.SpacePartitioningOn)
            {
                if (On(behavior_type.separation) && Utils.RandFloat() < SteerParams.Instance.PrSeparation)
                {
                    m_vSteeringForce += Separation(GameWorld.Instance.Agents) *
                                        m_dWeightSeparation / SteerParams.Instance.PrSeparation;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }
            }

            else
            {
                if (On(behavior_type.separation) && Utils.RandFloat() < SteerParams.Instance.PrSeparation)
                {
                    m_vSteeringForce += SeparationPlus(GameWorld.Instance.Agents) *
                                        m_dWeightSeparation / SteerParams.Instance.PrSeparation;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }
            }


            if (On(behavior_type.flee) && Utils.RandFloat() < SteerParams.Instance.PrFlee)
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Flee(GameWorld.Instance.TargetPos) * m_dWeightFlee / SteerParams.Instance.PrFlee;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            if (On(behavior_type.evade) && Utils.RandFloat() < SteerParams.Instance.PrEvade)
            {
                Debug.Assert(m_pTargetAgent1 != null, "Evade target not assigned");

                m_vSteeringForce += Evade(m_pTargetAgent1) * m_dWeightEvade / SteerParams.Instance.PrEvade;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }


            if (!GameWorld.Instance.SpacePartitioningOn)
            {
                if (On(behavior_type.allignment) && Utils.RandFloat() < SteerParams.Instance.PrAlignment)
                {
                    m_vSteeringForce += Alignment(GameWorld.Instance.Agents) *
                                        m_dWeightAlignment / SteerParams.Instance.PrAlignment;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }

                if (On(behavior_type.cohesion) && Utils.RandFloat() < SteerParams.Instance.PrCohesion)
                {
                    m_vSteeringForce += Cohesion(GameWorld.Instance.Agents) *
                                        m_dWeightCohesion / SteerParams.Instance.PrCohesion;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }
            }
            else
            {
                if (On(behavior_type.allignment) && Utils.RandFloat() < SteerParams.Instance.PrAlignment)
                {
                    m_vSteeringForce += AlignmentPlus(GameWorld.Instance.Agents) *
                                        m_dWeightAlignment / SteerParams.Instance.PrAlignment;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }

                if (On(behavior_type.cohesion) && Utils.RandFloat() < SteerParams.Instance.PrCohesion)
                {
                    m_vSteeringForce += CohesionPlus(GameWorld.Instance.Agents) *
                                        m_dWeightCohesion / SteerParams.Instance.PrCohesion;

                    if (!m_vSteeringForce.isZero())
                    {
                        m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                        return m_vSteeringForce;
                    }
                }
            }

            if (On(behavior_type.wander) && Utils.RandFloat() < SteerParams.Instance.PrWander)
            {
                m_vSteeringForce += Wander() * m_dWeightWander / SteerParams.Instance.PrWander;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            if (On(behavior_type.seek) && Utils.RandFloat() < SteerParams.Instance.PrSeek)
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Seek(GameWorld.Instance.TargetPos) * m_dWeightSeek / SteerParams.Instance.PrSeek;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            if (On(behavior_type.arrive) && Utils.RandFloat() < SteerParams.Instance.PrArrive)
            {
                Debug.Assert(!Vector2D.IsNull(GameWorld.Instance.TargetPos), "TargetPos not assigned");
                m_vSteeringForce += Arrive(GameWorld.Instance.TargetPos, (int)m_Deceleration) *
                                    m_dWeightArrive / SteerParams.Instance.PrArrive;

                if (!m_vSteeringForce.isZero())
                {
                    m_vSteeringForce.Truncate(m_parentMovingEntity.MaxForce);

                    return m_vSteeringForce;
                }
            }

            return m_vSteeringForce;
        }



        /////////////////////////////////////////////////////////////////////////////// START OF BEHAVIORS

        //------------------------------- Seek -----------------------------------
        //
        //  Given a target, this behavior returns a steering force which will
        //  direct the agent towards the target
        //------------------------------------------------------------------------
        public Vector2D Seek(Vector2D TargetPos)
        {
            Vector2D DesiredVelocity = Vector2D.Vec2DNormalize(TargetPos - m_parentMovingEntity.Pos)
                                      * m_parentMovingEntity.MaxSpeed;

            return (DesiredVelocity - m_parentMovingEntity.Velocity);
        }

        //----------------------------- Flee -------------------------------------
        //
        //  Does the opposite of Seek
        //------------------------------------------------------------------------
        public Vector2D Flee(Vector2D TargetPos)
        {
            //only flee if the target is within 'panic distance'. Work in distance
            //squared space.
            /* double PanicDistanceSq = 100.0f * 100.0;
             if (Vec2DDistanceSq(m_parentMovingEntity.Pos, target) > PanicDistanceSq)
             {
               return Vector2D(0,0);
             }
             */

            Vector2D DesiredVelocity = Vector2D.Vec2DNormalize(m_parentMovingEntity.Pos - TargetPos)
                                      * m_parentMovingEntity.MaxSpeed;

            return (DesiredVelocity - m_parentMovingEntity.Velocity);
        }

        //--------------------------- Arrive -------------------------------------
        //
        //  This behavior is similar to seek but it attempts to arrive at the
        //  target with a zero velocity
        //------------------------------------------------------------------------
        public Vector2D Arrive(Vector2D TargetPos, int decelerationStep)
        {
            Vector2D ToTarget = TargetPos - m_parentMovingEntity.Pos;

            //calculate the distance to the target
            double dist = ToTarget.Length();

            if (dist > 0)
            {
                //because Deceleration is enumerated as an int, this value is required
                //to provide fine tweaking of the deceleration..
                double DecelerationTweaker = 0.3;

                //calculate the speed required to reach the target given the desired
                //deceleration
                double speed = dist / ((double)decelerationStep * DecelerationTweaker);

                //make sure the velocity does not exceed the max
                speed = Math.Min(speed, m_parentMovingEntity.MaxSpeed);

                //from here proceed just like Seek except we don't need to normalize 
                //the ToTarget vector because we have already gone to the trouble
                //of calculating its length: dist. 
                Vector2D DesiredVelocity = (ToTarget * speed / dist);

                return (DesiredVelocity - m_parentMovingEntity.Velocity);
            }

            return new Vector2D(0, 0);
        }

        //------------------------------ Pursuit ---------------------------------
        //
        //  this behavior creates a force that steers the agent towards the 
        //  evader
        //------------------------------------------------------------------------
        public Vector2D Pursuit(MovingEntity evader)
        {
            //if the evader is ahead and facing the agent then we can just seek
            //for the evader's current position.
            Vector2D ToEvader = evader.Pos - m_parentMovingEntity.Pos;

            double RelativeHeading = m_parentMovingEntity.Heading().Dot(evader.Heading());

            if ((ToEvader.Dot(m_parentMovingEntity.Heading()) > 0) && (RelativeHeading < -0.95))  //acos(0.95)=18 degs
            {
                return Seek(evader.Pos);
            }

            //Not considered ahead so we predict where the evader will be.

            //the lookahead time is propotional to the distance between the evader
            //and the pursuer; and is inversely proportional to the sum of the
            //agent's velocities
            double LookAheadTime = ToEvader.Length() /
                                  (m_parentMovingEntity.MaxSpeed + evader.Speed());

            //now seek to the predicted future position of the evader
            return Seek(evader.Pos + evader.Velocity * LookAheadTime);
        }


        //----------------------------- Evade ------------------------------------
        //
        //  similar to pursuit except the agent Flees from the estimated future
        //  position of the pursuer
        //------------------------------------------------------------------------
        public Vector2D Evade(MovingEntity pursuer)
        {
            /* Not necessary to include the check for facing direction this time */

            Vector2D ToPursuer = (pursuer.Pos - m_parentMovingEntity.Pos);

            //uncomment the following two lines to have Evade only consider pursuers 
            //within a 'threat range'
            double ThreatRange = 100.0;
            if (ToPursuer.LengthSq() > ThreatRange * ThreatRange) return new Vector2D(0.0, 0.0);

            //the lookahead time is propotional to the distance between the pursuer
            //and the pursuer; and is inversely proportional to the sum of the
            //agents' velocities
            double LookAheadTime = ToPursuer.Length() /
                                   (m_parentMovingEntity.MaxSpeed + pursuer.Speed());

            //now flee away from predicted future position of the pursuer
            return Flee(pursuer.Pos + pursuer.Velocity * LookAheadTime);
        }


        //--------------------------- Wander -------------------------------------
        //
        //  This behavior makes the agent wander about randomly
        //------------------------------------------------------------------------
        public Vector2D Wander()
        {
            //this behavior is dependent on the update rate, so this line must
            //be included when using time independent framerate.
            double JitterThisTimeSlice = m_dWanderJitter * GameWorld.Instance.getTimeElapsed();

            //first, add a small random vector to the target's position
            m_vWanderTarget += new Vector2D(Utils.RandomClamped() * JitterThisTimeSlice,
                                        Utils.RandomClamped() * JitterThisTimeSlice);

            //reproject this new vector back on to a unit circle
            m_vWanderTarget.Normalize();

            //increase the length of the vector to the same as the radius
            //of the wander circle
            m_vWanderTarget *= m_dWanderRadius;

            //move the target into a position WanderDist in front of the agent
            Vector2D target = (m_vWanderTarget + new Vector2D(m_dWanderDistance, 0));

            //project the target into world space
            Vector2D Target = (Utils.PointToWorldSpace(target,
                                                 m_parentMovingEntity.Heading(),
                                                 m_parentMovingEntity.Side(),
                                                 m_parentMovingEntity.Pos));

            //and steer towards it
            return Target - m_parentMovingEntity.Pos;
        }


        //---------------------- ObstacleAvoidance -------------------------------
        //
        //  Given a vector of Obstacles, this method returns a steering force
        //  that will prevent the agent colliding with the closest obstacle
        //------------------------------------------------------------------------
        public Vector2D ObstacleAvoidance(List<BaseGameEntity> obstacles)
        {
            //the detection box length is proportional to the agent's velocity
            m_dDBoxLength = SteerParams.Instance.MinDetectionBoxLength +
                            (m_parentMovingEntity.Speed() / m_parentMovingEntity.MaxSpeed) * SteerParams.Instance.MinDetectionBoxLength;

            //tag all obstacles within range of the box for processing
            GameWorld.Instance.TagObstaclesWithinViewRange(m_parentMovingEntity, m_dDBoxLength);

            //this will keep track of the closest intersecting obstacle (CIB)
            BaseGameEntity ClosestIntersectingObstacle = null;

            //this will be used to track the distance to the CIB
            double DistToClosestIP = Double.MaxValue;

            //this will record the transformed local coordinates of the CIB
            Vector2D ClosestObstacleLocalPos = new Vector2D();

            foreach (BaseGameEntity curOb in obstacles)
            {
                //if the obstacle has been tagged within range proceed
                if (curOb.IsTagged())
                {
                    //calculate this obstacle's position in local space
                    Vector2D LocalPos = Utils.PointToLocalSpace(curOb.Pos,
                                                           m_parentMovingEntity.Heading(),
                                                           m_parentMovingEntity.Side(),
                                                           m_parentMovingEntity.Pos);

                    //if the local position has a negative x value then it must lay
                    //behind the agent. (in which case it can be ignored)
                    if (LocalPos.X >= 0)
                    {
                        //if the distance from the x axis to the object's position is less
                        //than its radius + half the width of the detection box then there
                        //is a potential intersection.
                        double ExpandedRadius = curOb.BRadius + m_parentMovingEntity.BRadius;

                        if (Math.Abs(LocalPos.Y) < ExpandedRadius)
                        {
                            //now to do a line/circle intersection test. The center of the 
                            //circle is represented by (cX, cY). The intersection points are 
                            //given by the formula x = cX +/-sqrt(r^2-cY^2) for y=0. 
                            //We only need to look at the smallest positive value of x because
                            //that will be the closest point of intersection.
                            double cX = LocalPos.X;
                            double cY = LocalPos.Y;

                            //we only need to calculate the sqrt part of the above equation once
                            double SqrtPart = Math.Sqrt(ExpandedRadius * ExpandedRadius - cY * cY);

                            double ip = cX - SqrtPart;

                            if (ip <= 0.0)
                            {
                                ip = cX + SqrtPart;
                            }

                            //test to see if this is the closest so far. If it is keep a
                            //record of the obstacle and its local coordinates
                            if (ip < DistToClosestIP)
                            {
                                DistToClosestIP = ip;

                                ClosestIntersectingObstacle = curOb;

                                ClosestObstacleLocalPos = LocalPos;
                            }
                        }
                    }
                }
            }

            //if we have found an intersecting obstacle, calculate a steering 
            //force away from it
            Vector2D SteeringForce = new Vector2D(0.0, 0.0);

            if (ClosestIntersectingObstacle != null)
            {
                //the closer the agent is to an object, the stronger the 
                //steering force should be
                double multiplier = 1.0 + (m_dDBoxLength - ClosestObstacleLocalPos.X) / m_dDBoxLength;

                //calculate the lateral force                                

                // Check to see if we could use a hint on choosing a more 
                // efficient direction to turn when avoiding the obstacle
                Vector2D targetLoc = null;

                if (On(behavior_type.follow_path) && (!m_pPath.Finished()))
                {
                    targetLoc = m_pPath.CurrentWaypoint();
                }
                else if ((On(behavior_type.seek) || On(behavior_type.arrive)) && (!Vector2D.IsNull(GameWorld.Instance.TargetPos)))
                {
                    targetLoc = GameWorld.Instance.TargetPos;
                }
                else if ((On(behavior_type.pursuit) || On(behavior_type.offset_pursuit)) && (m_pTargetAgent1 != null))
                {
                    targetLoc = m_pTargetAgent1.Pos;
                }

                if (!Vector2D.IsNull(targetLoc))
                {
                    // Get normalised direction to obstacle from current location
                    Vector2D dirToObs = ClosestIntersectingObstacle.Pos - m_parentMovingEntity.Pos;
                    dirToObs.Normalize();

                    // Calculate the two "apex choices" on the obstacles sphere
                    Vector2D interceptRightHand = Vector2D.ProjectedPerp(ClosestIntersectingObstacle.Pos,
                                                                                    dirToObs,
                                                                                    ClosestIntersectingObstacle.BRadius,
                                                                                    false);

                    Vector2D interceptLeftHand = Vector2D.ProjectedPerp(ClosestIntersectingObstacle.Pos,
                                                                                    dirToObs,
                                                                                    ClosestIntersectingObstacle.BRadius,
                                                                                    true);

                    // Calculate and compare the distances to determine the preferred "side" of the sphere.
                    double distRightHand = interceptRightHand.DistanceSq(targetLoc);
                    double distLeftHand = interceptLeftHand.DistanceSq(targetLoc);

                    if (distLeftHand < distRightHand)
                    {
                        multiplier = multiplier * -1; // We will travel on the left hand side of the sphere.
                    }
                }

                SteeringForce.Y = ClosestIntersectingObstacle.BRadius * multiplier;

                //apply a braking force proportional to the obstacles distance from
                //the vehicle. 
                double BrakingWeight = 0.2;

                SteeringForce.X = (ClosestIntersectingObstacle.BRadius -
                                   ClosestObstacleLocalPos.X) *
                                   BrakingWeight;
            }

            //finally, convert the steering vector from local to world space
            Vector2D vecReturn = Utils.VectorToWorldSpace(SteeringForce,
                                      m_parentMovingEntity.Heading(),
                                      m_parentMovingEntity.Side());

            return vecReturn;
        }


        //--------------------------- WallAvoidance --------------------------------
        //
        //  This returns a steering force that will keep the agent away from any
        //  walls it may encounter
        //------------------------------------------------------------------------
        public Vector2D WallAvoidance(List<Wall2D> walls)
        {
            //the feelers are contained in a List, m_Feelers
            CreateFeelers();

            double DistToThisIP = 0.0;
            double DistToClosestIP = Double.MaxValue;

            //this will hold an index into the vector of walls
            Wall2D ClosestWall = null;

            Vector2D SteeringForce = new Vector2D();
            Vector2D point = new Vector2D();         //used for storing temporary info
            Vector2D ClosestPoint = new Vector2D();  //holds the closest intersection point

            //examine each feeler in turn
            foreach (Vector2D flr in m_Feelers)
            {
                //run through each wall checking for any intersection points
                foreach (Wall2D w in walls)
                {
                    if (Utils.LineIntersection2D(m_parentMovingEntity.Pos,
                                           flr,
                                           w.From,
                                           w.To,
                                           ref DistToThisIP,
                                           ref point))
                    {
                        //is this the closest found so far? If so keep a record
                        if (DistToThisIP < DistToClosestIP)
                        {
                            DistToClosestIP = DistToThisIP;

                            ClosestWall = w;

                            ClosestPoint = point;
                        }
                    }
                }//next wall

                //if an intersection point has been detected, calculate a force  
                //that will direct the agent away
                if (ClosestWall != null)
                {
                    //calculate by what distance the projected position of the agent
                    //will overshoot the wall
                    Vector2D OverShoot = (flr - ClosestPoint);

                    //create a force in the direction of the wall normal, with a 
                    //magnitude of the overshoot
                    SteeringForce = (ClosestWall.Normal * OverShoot.Length());
                }

            }//next feeler

            return SteeringForce;
        }

        //------------------------------- CreateFeelers --------------------------
        //
        //  Creates the antenna utilized by WallAvoidance
        //------------------------------------------------------------------------
        public void CreateFeelers()
        {
            m_Feelers.Clear();

            //feeler pointing straight in front
            m_Feelers.Add(m_parentMovingEntity.Heading() * m_dWallDetectionFeelerLength + m_parentMovingEntity.Pos);

            //feeler to left
            Vector2D temp = m_parentMovingEntity.Heading();
            Utils.Vec2DRotateAroundOrigin(temp, Utils.HalfPi * 3.5f);
            m_Feelers.Add(temp * m_dWallDetectionFeelerLength * 0.5f + m_parentMovingEntity.Pos);

            //feeler to right
            temp = m_parentMovingEntity.Heading();
            Utils.Vec2DRotateAroundOrigin(temp, Utils.HalfPi * 0.5f);
            m_Feelers.Add(temp * m_dWallDetectionFeelerLength * 0.5f + m_parentMovingEntity.Pos);
        }


        //---------------------------- Separation --------------------------------
        //
        // this calculates a force repelling from the other neighbors
        //------------------------------------------------------------------------
        public Vector2D Separation(List<MovingEntity> neighbors)
        {
            Vector2D SteeringForce = new Vector2D(0.0, 0.0);

            foreach (MovingEntity neighbor in neighbors)
            {
                //make sure this agent isn't included in the calculations and that
                //the agent being examined is close enough. 
                //***also make sure it doesn't include the evade target ***
                if ((neighbor != m_parentMovingEntity) && neighbor.IsTagged() &&
                (neighbor != m_pTargetAgent1))
                {
                    Vector2D ToAgent = (m_parentMovingEntity.Pos - neighbor.Pos);

                    //scale the force inversely proportional to the agents distance from its neighbor.
                    SteeringForce += Vector2D.Vec2DNormalize(ToAgent) / ToAgent.Length();
                }
            }

            return SteeringForce;
        }


        //---------------------------- Alignment ---------------------------------
        //
        //  returns a force that attempts to align this agents heading with that
        //  of its neighbors
        //------------------------------------------------------------------------
        public Vector2D Alignment(List<MovingEntity> neighbors)
        {
            //used to record the average heading of the neighbors
            Vector2D AverageHeading = new Vector2D(0.0, 0.0);

            //used to count the number of vehicles in the neighborhood
            int NeighborCount = 0;

            //iterate through all the tagged vehicles and sum their heading vectors  
            foreach (MovingEntity neighbor in neighbors)
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined  is close enough ***also make sure it doesn't
                //include any evade target ***
                if ((neighbor != m_parentMovingEntity) && neighbor.IsTagged() &&
                (neighbor != m_pTargetAgent1))
                {
                    AverageHeading += neighbor.Heading();

                    ++NeighborCount;
                }
            }

            //if the neighborhood contained one or more vehicles, average their heading vectors.
            if (NeighborCount > 0)
            {
                AverageHeading /= (double)NeighborCount;

                AverageHeading -= m_parentMovingEntity.Heading();
            }

            return AverageHeading;
        }

        //-------------------------------- Cohesion ------------------------------
        //
        //  returns a steering force that attempts to move the agent towards the
        //  center of mass of the agents in its immediate area
        //------------------------------------------------------------------------
        public Vector2D Cohesion(List<MovingEntity> neighbors)
        {
            //first find the center of mass of all the agents
            Vector2D CenterOfMass = new Vector2D(0.0, 0.0);
            Vector2D SteeringForce = new Vector2D(0.0, 0.0);

            int NeighborCount = 0;

            //iterate through the neighbors and sum up all the position vectors
            foreach (MovingEntity neighbor in neighbors)
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined is close enough ***also make sure it doesn't
                //include the evade target ***
                if ((neighbor != m_parentMovingEntity) && neighbor.IsTagged() &&
                  (neighbor != m_pTargetAgent1))
                {
                    CenterOfMass += neighbor.Pos;

                    ++NeighborCount;
                }
            }

            if (NeighborCount > 0)
            {
                //the center of mass is the average of the sum of positions
                CenterOfMass /= (double)NeighborCount;

                //now seek towards that position
                SteeringForce = Seek(CenterOfMass);
            }

            //the magnitude of cohesion is usually much larger than separation or
            //allignment so it usually helps to normalize it.
            return Vector2D.Vec2DNormalize(SteeringForce);
        }


        /* NOTE: the next three behaviors are the same as the above three, except
                  that they use a cell-space partition to find the neighbors
        */


        //---------------------------- Separation --------------------------------
        //
        // this calculates a force repelling from the other neighbors
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        public Vector2D SeparationPlus(List<MovingEntity> neighbors)
        {
            Vector2D SteeringForce = new Vector2D(0.0, 0.0);

            //iterate through the neighbors and sum up all the position vectors
            foreach (MovingEntity pV in GameWorld.Instance.CellSpaces.ListOfNeighbours())
            {
                //make sure this agent isn't included in the calculations and that
                //the agent being examined is close enough
                if (pV != m_parentMovingEntity)
                {
                    Vector2D ToAgent = (m_parentMovingEntity.Pos - pV.Pos);

                    //scale the force inversely proportional to the agents distance  
                    //from its neighbor.
                    SteeringForce += Vector2D.Vec2DNormalize(ToAgent) / ToAgent.Length();
                }

            }

            return SteeringForce;
        }
        //---------------------------- Alignment ---------------------------------
        //
        //  returns a force that attempts to align this agents heading with that
        //  of its neighbors
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        public Vector2D AlignmentPlus(List<MovingEntity> neighbors)
        {
            //This will record the average heading of the neighbors
            Vector2D AverageHeading = new Vector2D();

            //This count the number of vehicles in the neighborhood
            double NeighborCount = 0.0;

            //iterate through the neighbors and sum up all the position vectors
            foreach (MovingEntity pV in GameWorld.Instance.CellSpaces.ListOfNeighbours())
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined  is close enough
                if (pV != m_parentMovingEntity)
                {
                    AverageHeading += pV.Heading();

                    ++NeighborCount;
                }

            }

            //if the neighborhood contained one or more vehicles, average their
            //heading vectors.
            if (NeighborCount > 0.0)
            {
                AverageHeading /= NeighborCount;

                AverageHeading -= m_parentMovingEntity.Heading();
            }

            return AverageHeading;
        }

        //-------------------------------- Cohesion ------------------------------
        //
        //  returns a steering force that attempts to move the agent towards the
        //  center of mass of the agents in its immediate area
        //
        //  USES SPACIAL PARTITIONING
        //------------------------------------------------------------------------
        public Vector2D CohesionPlus(List<MovingEntity> neighbors)
        {
            //first find the center of mass of all the agents
            Vector2D CenterOfMass = new Vector2D();
            Vector2D SteeringForce = new Vector2D();

            int NeighborCount = 0;

            //iterate through the neighbors and sum up all the position vectors
            foreach (MovingEntity pV in GameWorld.Instance.CellSpaces.ListOfNeighbours())
            {
                //make sure *this* agent isn't included in the calculations and that
                //the agent being examined is close enough
                if (pV != m_parentMovingEntity)
                {
                    CenterOfMass += pV.Pos;

                    ++NeighborCount;
                }
            }

            if (NeighborCount > 0)
            {
                //the center of mass is the average of the sum of positions
                CenterOfMass /= (double)NeighborCount;

                //now seek towards that position
                SteeringForce = Seek(CenterOfMass);
            }

            //the magnitude of cohesion is usually much larger than separation or
            //allignment so it usually helps to normalize it.
            return Vector2D.Vec2DNormalize(SteeringForce);
        }


        //--------------------------- Interpose ----------------------------------
        //
        //  Given two agents, this method returns a force that attempts to 
        //  position the vehicle between them
        //------------------------------------------------------------------------
        public Vector2D Interpose(MovingEntity AgentA, MovingEntity AgentB)
        {
            //first we need to figure out where the two agents are going to be at 
            //time T in the future. This is approximated by determining the time
            //taken to reach the mid way point at the current time at at max speed.
            Vector2D MidPoint = (AgentA.Pos + AgentB.Pos) * 0.5;

            double TimeToReachMidPoint = Vector2D.Vec2DDistance(m_parentMovingEntity.Pos, MidPoint) /
                                         m_parentMovingEntity.MaxSpeed;

            //now we have T, we assume that agent A and agent B will continue on a
            //straight trajectory and extrapolate to get their future positions
            Vector2D APos = AgentA.Pos + AgentA.Velocity * TimeToReachMidPoint;
            Vector2D BPos = AgentB.Pos + AgentB.Velocity * TimeToReachMidPoint;

            //calculate the mid point of these predicted positions
            MidPoint = (APos + BPos) / 2.0;

            //then steer to Arrive at it
            return Arrive(MidPoint, (int)Deceleration.fast);
        }

        //--------------------------- Hide ---------------------------------------
        //
        //------------------------------------------------------------------------
        public Vector2D Hide(MovingEntity hunter, List<BaseGameEntity> obstacles)
        {
            double DistToClosest = Double.MaxValue;
            Vector2D BestHidingSpot = new Vector2D(0.0, 0.0);

            foreach (BaseGameEntity curOb in obstacles)
            {
                //calculate the position of the hiding spot for this obstacle
                Vector2D HidingSpot = GetHidingPosition(curOb.Pos, curOb.BRadius, hunter.Pos);

                //work in distance-squared space to find the closest hiding
                //spot to the agent
                double dist = Vector2D.Vec2DDistanceSq(HidingSpot, m_parentMovingEntity.Pos);

                if (dist < DistToClosest)
                {
                    DistToClosest = dist;

                    BestHidingSpot = HidingSpot;
                }

            }//end while

            //if no suitable obstacles found then Evade the hunter
            if (DistToClosest == Double.MaxValue)
            {
                return Evade(hunter);
            }

            //else use Arrive on the hiding spot
            return Arrive(BestHidingSpot, (int)Deceleration.fast);
        }

        //------------------------- GetHidingPosition ----------------------------
        //
        //  Given the position of a hunter, and the position and radius of
        //  an obstacle, this method calculates a position DistanceFromBoundary 
        //  away from its bounding radius and directly opposite the hunter
        //------------------------------------------------------------------------
        public Vector2D GetHidingPosition(Vector2D posOb, double radiusOb, Vector2D posHunter)
        {
            //calculate how far away the agent is to be from the chosen obstacle's
            //bounding radius
            double DistanceFromBoundary = 30.0;
            double DistAway = radiusOb + DistanceFromBoundary;

            //calculate the heading toward the object from the hunter
            Vector2D ToOb = Vector2D.Vec2DNormalize(posOb - posHunter);

            //scale it to size and add to the obstacles position to get
            //the hiding spot.
            return (ToOb * DistAway) + posOb;
        }


        //------------------------------- FollowPath -----------------------------
        //
        //  Given a series of Vector2Ds, this method produces a force that will
        //  move the agent along the waypoints in order. The agent uses the
        // 'Seek' behavior to move to the next waypoint - unless it is the last
        //  waypoint, in which case it 'Arrives'
        //------------------------------------------------------------------------
        public Vector2D FollowPath()
        {
            if (m_pPath.Finished())
            {
                if (m_pPath.Loop)
                {
                    m_pPath.SetNextWaypoint();
                }
                else
                {
                    return new Vector2D();
                }
            }

            //move to next target if close enough to current target (working in
            //distance squared space)
            if (Vector2D.Vec2DDistanceSq(m_pPath.CurrentWaypoint(), m_parentMovingEntity.Pos) <
               m_dWaypointSeekDistSq)
            {
                m_pPath.SetNextWaypoint();
            }

            if (m_pPath.Finished())
            {
                return new Vector2D();
            }
            else if (!m_pPath.Finishing())
            {
                return Seek(m_pPath.CurrentWaypoint());
            }
            else
            {
                return Arrive(m_pPath.CurrentWaypoint(), (int)Deceleration.normal);
            }

        }

        //------------------------- Offset Pursuit -------------------------------
        //
        //  Produces a steering force that keeps a vehicle at a specified offset
        //  from a leader vehicle
        //------------------------------------------------------------------------
        public Vector2D OffsetPursuit(MovingEntity leader, Vector2D offset)
        {
            //calculate the offset's position in world space
            Vector2D WorldOffsetPos = Utils.PointToWorldSpace(offset,
                                                             leader.Heading(),
                                                             leader.Side(),
                                                             leader.Pos);

            Vector2D ToOffset = WorldOffsetPos - m_parentMovingEntity.Pos;

            //the lookahead time is propotional to the distance between the leader
            //and the pursuer; and is inversely proportional to the sum of both
            //agent's velocities
            double LookAheadTime = ToOffset.Length() /
                                  (m_parentMovingEntity.MaxSpeed + leader.Speed());

            //now Arrive at the predicted future position of the offset
            return Arrive(WorldOffsetPos + leader.Velocity * LookAheadTime, (int)Deceleration.fast);
        }

        //

    }
}
