using System;
using System.Collections.Generic;
using System.Text;
using ThinkSharp.Common;

namespace ThinkSharp.Steering
{
    public sealed class SteerParams
    {
        private static readonly SteerParams instance = new SteerParams();

        public static SteerParams Instance
        {
            get { return instance; }
        }

        #region Member variables and Properties

        private int m_NumAgents;
        private int m_NumObstacles;
        private double m_MinObstacleRadius;
        private double m_MaxObstacleRadius;

        //number of horizontal cells used for spatial partitioning
        private int m_NumCellsX;

        //number of vertical cells used for spatial partitioning
        private int m_NumCellsY;

        //how many samples the smoother will use to average a value
        private int m_NumSamplesForSmoothing;

        //used to tweak the combined steering force (simply altering the MaxSteeringForce
        //will NOT work!This tweaker affects all the steering force multipliers
        //too).
        private double m_SteeringForceTweaker;

        //how close a neighbour must be before an agent perceives it (considers it
        //to be within its neighborhood)
        private double m_ViewDistance;

        //used in obstacle avoidance
        private double m_MinDetectionBoxLength;

        //used in wall avoidance
        private double m_WallDetectionFeelerLength;

        //these are the probabilities that a steering behavior will be used
        //when the prioritized dither calculate method is used
        private double m_prWallAvoidance;

        private double m_SeparationWeight;
        private double m_AlignmentWeight;
        private double m_CohesionWeight;
        private double m_ObstacleAvoidanceWeight;
        private double m_WallAvoidanceWeight;
        private double m_WanderWeight;
        private double m_SeekWeight;
        private double m_FleeWeight;
        private double m_ArriveWeight;
        private double m_PursuitWeight;
        private double m_OffsetPursuitWeight;
        private double m_InterposeWeight;
        private double m_HideWeight;
        private double m_EvadeWeight;
        private double m_FollowPathWeight;

        private double m_MaxSteeringForce;
        private double m_MaxSpeed;
        private double m_VehicleMass;
        private double m_VehicleScale;

        private double m_prObstacleAvoidance;
        private double m_prSeparation;
        private double m_prAlignment;
        private double m_prCohesion;
        private double m_prWander;
        private double m_prSeek;
        private double m_prFlee;
        private double m_prEvade;
        private double m_prHide;
        private double m_prArrive;

        private double m_MaxTurnRatePerSecond;

        public int NumAgents
        {
            get { return m_NumAgents; }
            set { m_NumAgents = value; }
        }

        public int NumObstacles
        {
            get { return m_NumObstacles; }
            set { m_NumObstacles = value; }
        }

        public double MinObstacleRadius
        {
            get { return m_MinObstacleRadius; }
            set { m_MinObstacleRadius = value; }
        }

        public double MaxObstacleRadius
        {
            get { return m_MaxObstacleRadius; }
            set { m_MaxObstacleRadius = value; }
        }

        public int NumCellsX
        {
            get { return m_NumCellsX; }
            set { m_NumCellsX = value; }
        }

        public int NumCellsY
        {
            get { return m_NumCellsY; }
            set { m_NumCellsY = value; }
        }

        public int NumSamplesForSmoothing
        {
            get { return m_NumSamplesForSmoothing; }
            set { m_NumSamplesForSmoothing = value; }
        }

        public double SteeringForceTweaker
        {
            get { return m_SteeringForceTweaker; }
            set { m_SteeringForceTweaker = value; }
        }        

        public double MaxSteeringForce
        {
            get { return m_MaxSteeringForce; }
            set { m_MaxSteeringForce = value; }
        }

        public double MaxSpeed
        {
            get { return m_MaxSpeed; }
            set { m_MaxSpeed = value; }
        }
       
        public double VehicleMass
        {
            get { return m_VehicleMass; }
            set { m_VehicleMass = value; }
        }       

        public double VehicleScale
        {
            get { return m_VehicleScale; }
            set { m_VehicleScale = value; }
        }        

        public double MaxTurnRatePerSecond
        {
            get { return m_MaxTurnRatePerSecond; }
            set { m_MaxTurnRatePerSecond = value; }
        }

        public double SeparationWeight
        {
            get { return m_SeparationWeight; }
            set { m_SeparationWeight = value; }
        }

        public double AlignmentWeight
        {
            get { return m_AlignmentWeight; }
            set { m_AlignmentWeight = value; }
        }

        public double CohesionWeight
        {
            get { return m_CohesionWeight; }
            set { m_CohesionWeight = value; }
        }

        public double ObstacleAvoidanceWeight
        {
            get { return m_ObstacleAvoidanceWeight; }
            set { m_ObstacleAvoidanceWeight = value; }
        }

        public double WallAvoidanceWeight
        {
            get { return m_WallAvoidanceWeight; }
            set { m_WallAvoidanceWeight = value; }
        }

        public double WanderWeight
        {
            get { return m_WanderWeight; }
            set { m_WanderWeight = value; }
        }

        public double SeekWeight
        {
            get { return m_SeekWeight; }
            set { m_SeekWeight = value; }
        }

        public double FleeWeight
        {
            get { return m_FleeWeight; }
            set { m_FleeWeight = value; }
        }

        public double ArriveWeight
        {
            get { return m_ArriveWeight; }
            set { m_ArriveWeight = value; }
        }

        public double PursuitWeight
        {
            get { return m_PursuitWeight; }
            set { m_PursuitWeight = value; }
        }

        public double OffsetPursuitWeight
        {
            get { return m_OffsetPursuitWeight; }
            set { m_OffsetPursuitWeight = value; }
        }

        public double InterposeWeight
        {
            get { return m_InterposeWeight; }
            set { m_InterposeWeight = value; }
        }

        public double HideWeight
        {
            get { return m_HideWeight; }
            set { m_HideWeight = value; }
        }

        public double EvadeWeight
        {
            get { return m_EvadeWeight; }
            set { m_EvadeWeight = value; }
        }

        public double FollowPathWeight
        {
            get { return m_FollowPathWeight; }
            set { m_FollowPathWeight = value; }
        }       

        public double ViewDistance
        {
            get { return m_ViewDistance; }
            set { m_ViewDistance = value; }
        }

        public double MinDetectionBoxLength
        {
            get { return m_MinDetectionBoxLength; }
            set { m_MinDetectionBoxLength = value; }
        }

        public double WallDetectionFeelerLength
        {
            get { return m_WallDetectionFeelerLength; }
            set { m_WallDetectionFeelerLength = value; }
        }

        public double PrWallAvoidance
        {
            get { return m_prWallAvoidance; }
            set { m_prWallAvoidance = value; }
        }
        
        public double PrObstacleAvoidance
        {
            get { return m_prObstacleAvoidance; }
            set { m_prObstacleAvoidance = value; }
        }

        public double PrSeparation
        {
            get { return m_prSeparation; }
            set { m_prSeparation = value; }
        }
        
        public double PrAlignment
        {
            get { return m_prAlignment; }
            set { m_prAlignment = value; }
        }
        
        public double PrCohesion
        {
            get { return m_prCohesion; }
            set { m_prCohesion = value; }
        }
        
        public double PrWander
        {
            get { return m_prWander; }
            set { m_prWander = value; }
        }
       
        public double PrSeek
        {
            get { return m_prSeek; }
            set { m_prSeek = value; }
        }
       
        public double PrFlee
        {
            get { return m_prFlee; }
            set { m_prFlee = value; }
        }
       
        public double PrEvade
        {
            get { return m_prEvade; }
            set { m_prEvade = value; }
        }
      
        public double PrHide
        {
            get { return m_prHide; }
            set { m_prHide = value; }
        }

        public double PrArrive
        {
            get { return m_prArrive; }
            set { m_prArrive = value; }
        }
        #endregion

        public double AppliedMaxSteeringForce()
        {
            return m_MaxSteeringForce * m_SteeringForceTweaker;
        }
        public double AppliedSeparationWeight()
        {
            return m_SeparationWeight * m_SteeringForceTweaker;
        }
        public double AppliedAlignmentWeight()
        {
            return m_AlignmentWeight * m_SteeringForceTweaker;
        }
        public double AppliedCohesionWeight()
        {
            return m_CohesionWeight * m_SteeringForceTweaker;
        }
        public double AppliedObstacleAvoidanceWeight()
        {
            return m_ObstacleAvoidanceWeight * m_SteeringForceTweaker;
        }
        public double AppliedWallAvoidanceWeight()
        {
            return m_WallAvoidanceWeight * m_SteeringForceTweaker;
        }
        public double AppliedWanderWeight()
        {
            return m_WanderWeight * m_SteeringForceTweaker;
        }
        public double AppliedSeekWeight()
        {
            return m_SeekWeight * m_SteeringForceTweaker;
        }
        public double AppliedFleeWeight()
        {
            return m_FleeWeight * m_SteeringForceTweaker;
        }
        public double AppliedArriveWeight()
        {
            return m_ArriveWeight * m_SteeringForceTweaker;
        }
        public double AppliedPursuitWeight()
        {
            return m_PursuitWeight * m_SteeringForceTweaker;
        }
        public double AppliedOffsetPursuitWeight()
        {
            return m_OffsetPursuitWeight * m_SteeringForceTweaker;
        }
        public double AppliedInterposeWeight()
        {
            return m_InterposeWeight * m_SteeringForceTweaker;
        }
        public double AppliedHideWeight()
        {
            return m_HideWeight * m_SteeringForceTweaker;
        }
        public double AppliedEvadeWeight()
        {
            return m_EvadeWeight * m_SteeringForceTweaker;
        }
        public double AppliedFollowPathWeight()
        {
            return m_FollowPathWeight * m_SteeringForceTweaker;
        }

        private SteerParams()
        {
            m_NumAgents = 100;

            m_NumObstacles = 7;
            m_MinObstacleRadius = 10;
            m_MaxObstacleRadius = 40;

            //number of horizontal cells used for spatial partitioning
            m_NumCellsX = 7;
            //number of vertical cells used for spatial partitioning
            m_NumCellsY = 7;

            //how many samples the smoother will use to average a value
            m_NumSamplesForSmoothing = 5;

            //this is used to multiply the steering force AND all the multipliers
            //found in SteeringBehavior
            m_SteeringForceTweaker = 200.0;

            m_MaxSteeringForce = 2.0f;
            m_MaxSpeed = 150.0f;
            m_VehicleMass = 1.0f;
            m_VehicleScale = 3.0f;

            //use these values to tweak the amount that each steering force
            //contributes to the total steering force
            m_SeparationWeight = 1.0;
            m_AlignmentWeight = 1.0;
            m_CohesionWeight = 2.0;
            m_ObstacleAvoidanceWeight = 8.0;
            m_WallAvoidanceWeight = 10.0;
            m_WanderWeight = 1.0;
            m_SeekWeight = 1.0;
            m_FleeWeight = 1.0;
            m_ArriveWeight = 1.0;
            m_PursuitWeight = 1.0;
            m_OffsetPursuitWeight = 1.0;
            m_InterposeWeight = 1.0;
            m_HideWeight = 1.0;
            m_EvadeWeight = 0.01;
            m_FollowPathWeight = 0.05;

            //how close a neighbour must be before an agent perceives it (considers it
            //to be within its neighborhood)
            m_ViewDistance = 50.0;

            //used in obstacle avoidance
            m_MinDetectionBoxLength = 30.0;

            //used in wall avoidance
            m_WallDetectionFeelerLength = 40.0;

            //these are the probabilities that a steering behavior will be used
            //when the Prioritized Dither calculate method is used to sum
            //combined behaviors
            m_prWallAvoidance = 0.5f;
            m_prObstacleAvoidance = 0.5f;
            m_prSeparation = 0.2f;
            m_prAlignment = 0.3f;
            m_prCohesion = 0.6f;
            m_prWander = 0.8f;
            m_prSeek = 0.8f;
            m_prFlee = 0.6f;
            m_prEvade = 1.0f;
            m_prHide = 0.8f;
            m_prArrive = 0.5f;

            m_MaxTurnRatePerSecond = Utils.DegsToRads(45);
        }


    }

}


