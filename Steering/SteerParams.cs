using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    public sealed class SteerParams
    {
        private static readonly SteerParams instance = new SteerParams();

        public static SteerParams Instance
        {
            get{ return instance; }
        }

        private SteerParams() 
        {
            NumAgents = 100;

            NumObstacles = 7;
            MinObstacleRadius = 10;
            MaxObstacleRadius = 50;

            //number of horizontal cells used for spatial partitioning
            NumCellsX = 7;
            //number of vertical cells used for spatial partitioning
            NumCellsY = 7;

            //how many samples the smoother will use to average a value
            NumSamplesForSmoothing = 10;

            //this is used to multiply the steering force AND all the multipliers
            //found in SteeringBehavior
            SteeringForceTweaker = 200.0;

            MaxSteeringForce = 2.0f * SteeringForceTweaker;
            MaxSpeed = 150.0f;
            VehicleMass = 1.0f;
            VehicleScale = 3.0f;

            //use these values to tweak the amount that each steering force
            //contributes to the total steering force
            SeparationWeight = 2.0 * SteeringForceTweaker;
            AlignmentWeight = 1.0 * SteeringForceTweaker;
            CohesionWeight = 2.0 * SteeringForceTweaker;
            ObstacleAvoidanceWeight = 10.0 * SteeringForceTweaker;
            WallAvoidanceWeight = 10.0 * SteeringForceTweaker;
            WanderWeight = 1.0 * SteeringForceTweaker;
            SeekWeight = 1.0 * SteeringForceTweaker;
            FleeWeight = 1.0 * SteeringForceTweaker;
            ArriveWeight = 1.0 * SteeringForceTweaker;
            PursuitWeight = 1.0 * SteeringForceTweaker;
            OffsetPursuitWeight = 1.0 * SteeringForceTweaker;
            InterposeWeight = 1.0 * SteeringForceTweaker;
            HideWeight = 1.0 * SteeringForceTweaker;
            EvadeWeight = 0.01 * SteeringForceTweaker;
            FollowPathWeight = 0.05 * SteeringForceTweaker;

            //how close a neighbour must be before an agent perceives it (considers it
            //to be within its neighborhood)
            ViewDistance = 50.0;

            //used in obstacle avoidance
            MinDetectionBoxLength = 40.0;

            //used in wall avoidance
            WallDetectionFeelerLength = 40.0;

            //these are the probabilities that a steering behavior will be used
            //when the Prioritized Dither calculate method is used to sum
            //combined behaviors
            prWallAvoidance = 0.5f;
            prObstacleAvoidance = 0.5f;
            prSeparation = 0.2f;
            prAlignment = 0.3f;
            prCohesion = 0.6f;
            prWander = 0.8f;
            prSeek = 0.8f;
            prFlee = 0.6f;
            prEvade = 1.0f;
            prHide = 0.8f;
            prArrive = 0.5f;

            MaxTurnRatePerSecond = Math.PI / 2;
        }

        public int NumAgents;
        public int NumObstacles;
        public double MinObstacleRadius;
        public double MaxObstacleRadius;

        //number of horizontal cells used for spatial partitioning
        public int NumCellsX;
        //number of vertical cells used for spatial partitioning
        public int NumCellsY;

        //how many samples the smoother will use to average a value
        public int NumSamplesForSmoothing;

        //used to tweak the combined steering force (simply altering the MaxSteeringForce
        //will NOT work!This tweaker affects all the steering force multipliers
        //too).
        public double SteeringForceTweaker;

        public double MaxSteeringForce;
        public double MaxSpeed;
        public double VehicleMass;

        public double VehicleScale;
        public double MaxTurnRatePerSecond;

        public double SeparationWeight;
        public double AlignmentWeight;
        public double CohesionWeight;
        public double ObstacleAvoidanceWeight;
        public double WallAvoidanceWeight;
        public double WanderWeight;
        public double SeekWeight;
        public double FleeWeight;
        public double ArriveWeight;
        public double PursuitWeight;
        public double OffsetPursuitWeight;
        public double InterposeWeight;
        public double HideWeight;
        public double EvadeWeight;
        public double FollowPathWeight;

        //how close a neighbour must be before an agent perceives it (considers it
        //to be within its neighborhood)
        public double ViewDistance;

        //used in obstacle avoidance
        public double MinDetectionBoxLength;

        //used in wall avoidance
        public double WallDetectionFeelerLength;

        //these are the probabilities that a steering behavior will be used
        //when the prioritized dither calculate method is used
        public double prWallAvoidance;
        public double prObstacleAvoidance;
        public double prSeparation;
        public double prAlignment;
        public double prCohesion;
        public double prWander;
        public double prSeek;
        public double prFlee;
        public double prEvade;
        public double prHide;
        public double prArrive;

    }

}


