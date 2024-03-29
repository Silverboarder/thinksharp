
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using ThinkSharp.Common;

namespace ThinkSharp.Steering
{
    public class MovingEntity : BaseGameEntity
    {
        protected Vector2D m_vVelocity;

        //a normalized vector pointing in the direction the entity is heading. 
        protected Vector2D m_vHeading;

        protected Vector2D m_vSmoothedHeading;

        //a vector perpendicular to the heading vector
        protected Vector2D m_vSide;

        protected double m_dMass;

        //the maximum speed this entity may travel at.
        protected double m_dMaxSpeed;

        //the maximum force this entity can produce to power itself (think rockets and thrust)
        protected double m_dMaxForce;

        //the maximum rate (radians per second) this vehicle can rotate         
        protected double m_dMaxTurnRate;

        //the steering behavior class
        protected SteeringBehavior m_pSteering;        

        protected Vector2D m_OldPos;

        protected int m_intOldCellID;

        private List<Vector2D> m_HeadingHistory;
        private int m_intNextHeadingSlot;

        private const int NumAccelItems = 5;

        public MovingEntity(Vector2D position,
                double      radius,
                Vector2D    velocity,
                double      max_speed,
                Vector2D    heading,
                double      mass,
                Vector2D    scale,
                double      turn_rate,
               double       max_force) : base(0, position, radius)
        {
            m_vHeading = heading;
            m_vVelocity = velocity;
            m_dMass = mass;
            m_vSide = m_vHeading.Perp();
            m_dMaxSpeed = max_speed;
            m_dMaxTurnRate = turn_rate;
            m_dMaxForce = max_force;
            m_vScale = scale;

            m_OldPos = new Vector2D();
            m_vSmoothedHeading = new Vector2D();

            m_intOldCellID = -1;

            m_pSteering = new SteeringBehavior(this);

            m_HeadingHistory = new List<Vector2D>(SteerParams.Instance.NumSamplesForSmoothing);
            m_intNextHeadingSlot = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is MovingEntity && obj != null)
            {
                MovingEntity objMoveingEntity = (MovingEntity)obj;
                if (objMoveingEntity.ID() == this.ID())
                    return true;
            }
            return false;
        }

        //accessors
        public SteeringBehavior Steering() { return m_pSteering; }

        public Vector2D OldPos
        {
            get { return m_OldPos; }
            set { m_OldPos = value; }
        }

        public int OldCellID
        {
            get { return m_intOldCellID; }
            set { m_intOldCellID = value; }
        } 

        public Vector2D Velocity
        {
            get { return m_vVelocity; }
            set { m_vVelocity = value; }
        }

        public double Mass() {return m_dMass;}

        public Vector2D Side() {return m_vSide;}

        public double MaxSpeed
        {
            get { return m_dMaxSpeed; }
            set { m_dMaxSpeed = value; }
        }

        public double MaxForce
        {
            get { return m_dMaxForce; }
            set { m_dMaxForce = value; }
        }

        public bool IsSpeedMaxedOut(){return m_dMaxSpeed * m_dMaxSpeed >= m_vVelocity.LengthSq();}
        public double Speed(){return m_vVelocity.Length();}
        public double SpeedSq(){return m_vVelocity.LengthSq();}

        public Vector2D Heading() {return m_vHeading;}
        public Vector2D SmoothedHeading() { return m_vSmoothedHeading; }

        public double MaxTurnRate
        {
            get { return m_dMaxTurnRate; }
            set { m_dMaxTurnRate = value; }
        }

        //--------------------------- RotateHeadingToFacePosition ---------------------
        //
        //  given a target position, this method rotates the entity's heading and
        //  side vectors by an amount not greater than m_dMaxTurnRate until it
        //  directly faces the target.
        //
        //  returns true when the heading is facing in the desired direction
        //-----------------------------------------------------------------------------
        public bool RotateHeadingToFacePosition(Vector2D target)
        {
            Vector2D toTarget = Vector2D.Vec2DNormalize(target - m_vPos);

            //first determine the angle between the heading vector and the target
            double angle = Math.Acos(m_vHeading.Dot(toTarget));

            //return true if the player is facing the target
            if (angle < 0.00001) return true;

            //clamp the amount to turn to the max turn rate
            if (angle > m_dMaxTurnRate) angle = m_dMaxTurnRate;

            //The next few lines use a rotation matrix to rotate the player's heading
            //vector accordingly
            C2DMatrix RotationMatrix = new C2DMatrix();

            //notice how the direction of rotation has to be determined when creating
            //the rotation matrix
            RotationMatrix.Rotate(angle * m_vHeading.Sign(toTarget));

            RotationMatrix.TransformVector2D(m_vHeading); 
            RotationMatrix.TransformVector2D(m_vVelocity);

            //finally recreate m_vSide
            m_vSide = m_vHeading.Perp();

            return false;
        }

        //------------------------- SetHeading ----------------------------------------
        //
        //  first checks that the given heading is not a vector of zero length. If the
        //  new heading is valid this fumction sets the entity's heading and side 
        //  vectors accordingly
        //-----------------------------------------------------------------------------
        public void SetHeading(Vector2D new_heading)
        {
            Debug.Assert((new_heading.LengthSq() - 1.0) < 0.00001);

            m_vHeading = new_heading;

            //the side vector must always be perpendicular to the heading
            m_vSide = m_vHeading.Perp();
        }

        public void Update(double time_elapsed) 
        {
            //keep a record of its old position so we can update its cell later in this method
            m_OldPos = Pos;

            Vector2D SteeringForce;

            //calculate the combined force from each steering behavior in the vehicle's list
            SteeringForce = m_pSteering.Calculate();

            //Acceleration = Force/Mass
            Vector2D acceleration = SteeringForce / m_dMass;

            //update velocity
            m_vVelocity += acceleration * time_elapsed;

            //make sure vehicle does not exceed maximum velocity
            m_vVelocity.Truncate(m_dMaxSpeed);

            //update the position
            m_vPos += m_vVelocity * time_elapsed;

            //update the heading if the vehicle has a non zero velocity
            if (m_vVelocity.LengthSq() > 0.00000001)
            {
                m_vHeading = Vector2D.Vec2DNormalize(m_vVelocity);
                m_vSide = m_vHeading.Perp();
            }
        }

        public void PerformSmoothing()
        {
            //overwrite the oldest value with the newest
            if (m_HeadingHistory.Count < SteerParams.Instance.NumSamplesForSmoothing)
                m_HeadingHistory.Add(m_vHeading);
            else
                m_HeadingHistory[m_intNextHeadingSlot] = m_vHeading;

            m_intNextHeadingSlot = m_intNextHeadingSlot + 1;

            //make sure m_iNextUpdateSlot wraps around. 
            if (m_intNextHeadingSlot == SteerParams.Instance.NumSamplesForSmoothing) m_intNextHeadingSlot = 0;

            Vector2D sum = new Vector2D();

            foreach (Vector2D objVec in m_HeadingHistory)
            {
                sum = sum + objVec;
            }

            m_vSmoothedHeading = sum / (double)m_HeadingHistory.Count;
        }

        public void ResetSmoothing()
        {
            m_intNextHeadingSlot = 0;
            m_HeadingHistory.Clear();
            m_HeadingHistory = new List<Vector2D>(SteerParams.Instance.NumSamplesForSmoothing);
        }

        //------------------- EnforceNonPenetrationConstraint ---------------------
        //
        //  Given a pointer to an entity and a std container of pointers to nearby
        //  entities, this function checks to see if there is an overlap between
        //  entities. If there is, then the entities are moved away from each
        //  other
        //------------------------------------------------------------------------
        public static List<MovingEntity> EnforceNonPenetrationConstraint(BaseGameEntity entity, List<MovingEntity> ContainerOfEntities)
        {
            List<MovingEntity> ListTouched = new List<MovingEntity>();

            //iterate through all entities checking for any overlap of bounding radii
            foreach (MovingEntity curEntity in ContainerOfEntities)
            {
                //make sure we don't check against the individual
                if (curEntity == entity) continue;

                //calculate the distance between the positions of the entities
                Vector2D ToEntity = entity.Pos - curEntity.Pos;

                double DistFromEachOther = ToEntity.Length();

                //if this distance is smaller than the sum of their radii then this
                //entity must be moved away in the direction parallel to the
                //ToEntity vector   
                double AmountOfOverLap = curEntity.BRadius + entity.BRadius - DistFromEachOther;

                if (AmountOfOverLap >= 0)
                {
                    ListTouched.Add(curEntity);

                    //move the entity a distance away equivalent to the amount of overlap.
                    entity.Pos = entity.Pos + (ToEntity / DistFromEachOther) * AmountOfOverLap;
                }

            }//next entity

            return ListTouched;
        }

    }
}
