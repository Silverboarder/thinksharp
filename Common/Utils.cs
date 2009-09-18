using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ThinkSharp.Common
{
    public struct Utils
    {
        public const double TwoPi = Math.PI * 2;
        public const double HalfPi = Math.PI / 2;
        public const double QuarterPi = Math.PI / 4;

        private static double y2;
        private static int use_last = 0;
        private static Random fixRand = new Random();

        public static double DegsToRads(double degs)
        {
            return TwoPi * (degs / 360.0);
        }

        private static float getNormalizedSine(int x, double halfY, float maxX)
        {
            Double factor = TwoPi / maxX;
            Double dblReturn = (Math.Sin(x * factor) * halfY) + halfY;

            return (float)dblReturn;
        }

        //returns true if the third parameter is in the range described by the first two
        public static bool InRange(double start, double end, double val)
        {
            if (start < end)
            {
                if ((val > start) && (val < end)) return true;
                else return false;
            }

            else
            {
                if ((val < start) && (val > end)) return true;
                else return false;
            }
        }

        //----------------------------------------------------------------------------
        //  some random number functions.
        //----------------------------------------------------------------------------

        //returns a random integer between x and y
        public static int RandInt(int min, int max)
        {
            max = max + 1;
            Debug.Assert(max >= min, "<RandInt>: max is less than min");
            return fixRand.Next(min, max);
        }

        //returns a random double between zero and 1
        public static double RandFloat()
        {
            return fixRand.NextDouble();
        }

        public static double RandInRange(double x, double y)
        {
            return x + RandFloat() * (y - x);
        }

        //returns a random bool
        public static bool RandBool()
        {
            if (RandFloat() > 0.5) return true;
            else return false;
        }

        //returns a random double in the range -1 < n < 1
        public static double RandomClamped() { return RandFloat() - RandFloat(); }

        public static double RandGaussian()
        {
            return RandGaussian(0.0, 1.0);
        }

        //returns a random number with a normal distribution. See method at
        //http://www.taygeta.com/random/gaussian.html
        public static double RandGaussian(double mean, double standard_deviation)
        {
            double x1, x2, w, y1;

            if (use_last > 0)		        /* use value from previous call */
            {
                y1 = y2;
                use_last = 0;
            }
            else
            {
                do
                {
                    x1 = 2.0 * RandFloat() - 1.0;
                    x2 = 2.0 * RandFloat() - 1.0;
                    w = x1 * x1 + x2 * x2;
                }
                while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                use_last = 1;
            }

            return (mean + y1 * standard_deviation);
        }

        //-----------------------------------------------------------------------
        //  
        //  some handy little functions
        //-----------------------------------------------------------------------

        //public double Sigmoid(double input, double response = 1.0)
        public static double Sigmoid(double input, double response)
        {
            return (1.0 / (1.0 + Math.Exp(-input / response)));
        }

        //rounds a double up or down depending on its value
        public static int Rounded(double val)
        {
            int integral = (int)val;
            double mantissa = val - integral;

            if (mantissa < 0.5)
            {
                return integral;
            }

            else
            {
                return integral + 1;
            }
        }

        //rounds a double up or down depending on whether its 
        //mantissa is higher or lower than offset
        public static int RoundUnderOffset(double val, double offset)
        {
            int integral = (int)val;
            double mantissa = val - integral;

            if (mantissa < offset)
            {
                return integral;
            }

            else
            {
                return integral + 1;
            }
        }

        //compares two real numbers. Returns true if they are equal
        public static bool isEqual(float a, float b)
        {
            if (Math.Abs(a - b) < 1E-12)
            {
                return true;
            }

            return false;
        }

        public static bool isEqual(double a, double b)
        {
            if (Math.Abs(a - b) < 1E-12)
            {
                return true;
            }

            return false;
        }

        #region " Transformation Functions "

        //--------------------------- WorldTransform -----------------------------
        //
        //  given a std::vector of 2D vectors, a position, orientation and scale,
        //  this function transforms the 2D vectors into the object's world space
        //------------------------------------------------------------------------
        public static List<Vector2D> WorldTransform(List<Vector2D> points,
                                                    Vector2D pos,
                                                    Vector2D forward,
                                                    Vector2D side,
                                                    Vector2D scale)
        {
            //copy the original vertices into the buffer about to be transformed
            List<Vector2D> TranVector2Ds = points;

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //scale
            if ((scale.X != 1.0) || (scale.Y != 1.0))
            {
                matTransform.Scale(scale.X, scale.Y);
            }

            //rotate
            matTransform.Rotate(forward, side);

            //and translate
            matTransform.Translate(pos.X, pos.Y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds(TranVector2Ds);

            return TranVector2Ds;
        }

        //--------------------------- WorldTransform -----------------------------
        //
        //  given a std::vector of 2D vectors, a position and  orientation
        //  this function transforms the 2D vectors into the object's world space
        //------------------------------------------------------------------------
        public static List<Vector2D> WorldTransform(List<Vector2D> points,
                                         Vector2D pos,
                                         Vector2D forward,
                                         Vector2D side)
        {
            //copy the original vertices into the buffer about to be transformed
            List<Vector2D> TranVector2Ds = points;

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //rotate
            matTransform.Rotate(forward, side);

            //and translate
            matTransform.Translate(pos.X, pos.Y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds( TranVector2Ds);

            return TranVector2Ds;
        }

        //--------------------- PointToWorldSpace --------------------------------
        //
        //  Transforms a point from the agent's local space into world space
        //------------------------------------------------------------------------
        public static Vector2D PointToWorldSpace(Vector2D point,
                                            Vector2D AgentHeading,
                                            Vector2D AgentSide,
                                            Vector2D AgentPosition)
        {
            //make a copy of the point
            Vector2D TransPoint = new Vector2D(point.X, point.Y); ;

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //rotate
            matTransform.Rotate(AgentHeading, AgentSide);

            //and translate
            matTransform.Translate(AgentPosition.X, AgentPosition.Y);

            //now transform the vertices
            matTransform.TransformVector2D(TransPoint);

            return TransPoint;
        }

        //--------------------- VectorToWorldSpace --------------------------------
        //
        //  Transforms a vector from the agent's local space into world space
        //------------------------------------------------------------------------
        public static Vector2D VectorToWorldSpace(Vector2D vec,
                                             Vector2D AgentHeading,
                                             Vector2D AgentSide)
        {
            //make a copy of the point
            Vector2D TransVec = new Vector2D(vec.X, vec.Y); ;

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //rotate
            matTransform.Rotate(AgentHeading, AgentSide);

            //now transform the vertices
            matTransform.TransformVector2D( TransVec);

            return TransVec;
        }


        //--------------------- PointToLocalSpace --------------------------------
        //
        //------------------------------------------------------------------------
        public static Vector2D PointToLocalSpace(Vector2D point,
                                     Vector2D AgentHeading,
                                     Vector2D AgentSide,
                                      Vector2D AgentPosition)
        {

            //make a copy of the point
            Vector2D TransPoint = new Vector2D(point.X, point.Y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            double Tx = -AgentPosition.Dot(AgentHeading);
            double Ty = -AgentPosition.Dot(AgentSide);

            //create the transformation matrix
            matTransform._11(AgentHeading.X); matTransform._12(AgentSide.X);
            matTransform._21(AgentHeading.Y); matTransform._22(AgentSide.Y);
            matTransform._31(Tx); matTransform._32(Ty);

            //now transform the vertices
            matTransform.TransformVector2D( TransPoint);

            return TransPoint;
        }

        //--------------------- VectorToLocalSpace --------------------------------
        //
        //------------------------------------------------------------------------
        public static Vector2D VectorToLocalSpace(Vector2D vec,
                                     Vector2D AgentHeading,
                                     Vector2D AgentSide)
        {

            //make a copy of the point
            Vector2D TransPoint = new Vector2D(vec.X, vec.Y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //create the transformation matrix
            matTransform._11(AgentHeading.X); matTransform._12(AgentSide.X);
            matTransform._21(AgentHeading.Y); matTransform._22(AgentSide.Y);

            //now transform the vertices
            matTransform.TransformVector2D( TransPoint);

            return TransPoint;
        }

        //-------------------------- Vec2DRotateAroundOrigin --------------------------
        //
        //  rotates a vector ang rads around the origin
        //-----------------------------------------------------------------------------
        public static void Vec2DRotateAroundOrigin(Vector2D v, double ang)
        {
            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();

            //rotate
            matTransform.Rotate(ang);

            //now transform the object's vertices
            matTransform.TransformVector2D( v);
        }

        //------------------------ CreateWhiskers ------------------------------------
        //
        //  given an origin, a facing direction, a 'field of view' describing the 
        //  limit of the outer whiskers, a whisker length and the number of whiskers
        //  this method returns a vector containing the end positions of a series
        //  of whiskers radiating away from the origin and with equal distance between
        //  them. (like the spokes of a wheel clipped to a specific segment size)
        //----------------------------------------------------------------------------
        public static List<Vector2D> CreateWhiskers(int NumWhiskers,
                                                    double WhiskerLength,
                                                    double fov,
                                                    Vector2D facing,
                                                    Vector2D origin)
        {

            //this is the magnitude of the angle separating each whisker
            double SectorSize = fov / (double)(NumWhiskers - 1);

            List<Vector2D> whiskers = new List<Vector2D>();
            Vector2D temp;
            double angle = -fov * 0.5;

            for (int w = 0; w < NumWhiskers; ++w)
            {
                //create the whisker extending outwards at this angle
                temp = facing;
                Vec2DRotateAroundOrigin(temp, angle);

                whiskers.Add(origin + temp * WhiskerLength);

                angle += SectorSize;
            }

            return whiskers;
        }

        #endregion

        #region " Geometry Functions "

        //given a plane and a ray this function determins how far along the ray 
        //an interestion occurs. Returns negative if the ray is parallel
        public static double DistanceToRayPlaneIntersection(Vector2D RayOrigin,
                                                     Vector2D RayHeading,
                                                     Vector2D PlanePoint,  //any point on the plane
                                                     Vector2D PlaneNormal)
        {

            double d = -PlaneNormal.Dot(PlanePoint);
            double numer = PlaneNormal.Dot(RayOrigin) + d;
            double denom = PlaneNormal.Dot(RayHeading);

            // normal is parallel to vector
            if ((denom < 0.000001) && (denom > -0.000001))
            {
                return (-1.0);
            }

            return -(numer / denom);
        }

        //------------------------- WhereIsPoint --------------------------------------
        public enum Enum_Span_Type { plane_backside, plane_front, on_plane };

        public static Enum_Span_Type WhereIsPoint(Vector2D point,
                                      Vector2D PointOnPlane, //any point on the plane
                                      Vector2D PlaneNormal)
        {
            Vector2D dir = PointOnPlane - point;

            double d = dir.Dot(PlaneNormal);

            if (d < -0.000001)
            {
                return Enum_Span_Type.plane_front;
            }

            else if (d > 0.000001)
            {
                return Enum_Span_Type.plane_backside;
            }

            return Enum_Span_Type.on_plane;
        }


        //-------------------------- GetRayCircleIntersec -----------------------------
        public static double GetRayCircleIntersect(Vector2D RayOrigin,
                                            Vector2D RayHeading,
                                            Vector2D CircleOrigin,
                                            double radius)
        {

            Vector2D ToCircle = CircleOrigin - RayOrigin;
            double length = ToCircle.Length();
            double v = ToCircle.Dot(RayHeading);
            double d = radius * radius - (length * length - v * v);

            // If there was no intersection, return -1
            if (d < 0.0) return (-1.0);

            // Return the distance to the [first] intersecting point
            return (v - Math.Sqrt(d));
        }

        //----------------------------- DoRayCircleIntersect --------------------------
        public static bool DoRayCircleIntersect(Vector2D RayOrigin,
                                         Vector2D RayHeading,
                                         Vector2D CircleOrigin,
                                         double radius)
        {

            Vector2D ToCircle = CircleOrigin - RayOrigin;
            double length = ToCircle.Length();
            double v = ToCircle.Dot(RayHeading);
            double d = radius * radius - (length * length - v * v);

            // If there was no intersection, return -1
            return (d < 0.0);
        }


        //------------------------------------------------------------------------
        //  Given a point P and a circle of radius R centered at C this function
        //  determines the two points on the circle that intersect with the 
        //  tangents from P to the circle. Returns false if P is within the circle.
        //
        //  thanks to Dave Eberly for this one.
        //------------------------------------------------------------------------
        public static bool GetTangentPoints(Vector2D C, double R, Vector2D P, ref Vector2D T1, ref Vector2D T2)
        {
            Vector2D PmC = P - C;
            double SqrLen = PmC.LengthSq();
            double RSqr = R * R;

            if (SqrLen <= RSqr)
            {
                // P is inside or on the circle
                return false;
            }

            double InvSqrLen = 1 / SqrLen;
            double Root = Math.Sqrt(Math.Abs(SqrLen - RSqr));

            T1.X = C.X + R * (R * PmC.X - PmC.Y * Root) * InvSqrLen;
            T1.Y = C.Y + R * (R * PmC.Y + PmC.X * Root) * InvSqrLen;
            T2.X = C.X + R * (R * PmC.X + PmC.Y * Root) * InvSqrLen;
            T2.Y = C.Y + R * (R * PmC.Y - PmC.X * Root) * InvSqrLen;

            return true;
        }




        //------------------------- DistToLineSegment ----------------------------
        //
        //  given a line segment AB and a point P, this function calculates the 
        //  perpendicular distance between them
        //------------------------------------------------------------------------
        public static double DistToLineSegment(Vector2D A,
                                        Vector2D B,
                                        Vector2D P)
        {
            //if the angle is obtuse between PA and AB is obtuse then the closest
            //vertex must be A
            double dotA = (P.X - A.X) * (B.X - A.X) + (P.Y - A.Y) * (B.Y - A.Y);

            if (dotA <= 0) return Vector2D.Vec2DDistance(A, P);

            //if the angle is obtuse between PB and AB is obtuse then the closest
            //vertex must be B
            double dotB = (P.X - B.X) * (A.X - B.X) + (P.Y - B.Y) * (A.Y - B.Y);

            if (dotB <= 0) return Vector2D.Vec2DDistance(B, P);

            //calculate the point along AB that is the closest to P
            Vector2D Point = A + ((B - A) * dotA) / (dotA + dotB);

            //calculate the distance P-Point
            return Vector2D.Vec2DDistance(P, Point);
        }

        //------------------------- DistToLineSegmentSq ----------------------------
        //
        //  as above, but avoiding Math.Sqrt
        //------------------------------------------------------------------------
        public static double DistToLineSegmentSq(Vector2D A,
                                         Vector2D B,
                                         Vector2D P)
        {
            //if the angle is obtuse between PA and AB is obtuse then the closest
            //vertex must be A
            double dotA = (P.X - A.X) * (B.X - A.X) + (P.Y - A.Y) * (B.Y - A.Y);

            if (dotA <= 0) return Vector2D.Vec2DDistanceSq(A, P);

            //if the angle is obtuse between PB and AB is obtuse then the closest
            //vertex must be B
            double dotB = (P.X - B.X) * (A.X - B.X) + (P.Y - B.Y) * (A.Y - B.Y);

            if (dotB <= 0) return Vector2D.Vec2DDistanceSq(B, P);

            //calculate the point along AB that is the closest to P
            Vector2D Point = A + ((B - A) * dotA) / (dotA + dotB);

            //calculate the distance P-Point
            return Vector2D.Vec2DDistanceSq(P, Point);
        }


        //--------------------LineIntersection2D-------------------------
        //
        //	Given 2 lines in 2D space AB, CD this returns true if an 
        //	intersection occurs.
        //
        //----------------------------------------------------------------- 

        public static bool LineIntersection2D(Vector2D A,
                                       Vector2D B,
                                       Vector2D C,
                                       Vector2D D)
        {
            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

            double Bot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if (Bot == 0)//parallel
            {
                return false;
            }

            double invBot = 1.0 / Bot;
            double r = rTop * invBot;
            double s = sTop * invBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                //lines intersect
                return true;
            }

            //lines do not intersect
            return false;
        }

        //--------------------LineIntersection2D-------------------------
        //
        //	Given 2 lines in 2D space AB, CD this returns true if an 
        //	intersection occurs and sets dist to the distance the intersection
        //  occurs along AB
        //
        //----------------------------------------------------------------- 
        public static bool LineIntersection2D(Vector2D A,
                                Vector2D B,
                                Vector2D C,
                                Vector2D D,
                                ref double dist)
        {

            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);

            double Bot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);


            if (Bot == 0)//parallel
            {
                if (isEqual(rTop, 0) && isEqual(sTop, 0))
                {
                    return true;
                }
                return false;
            }

            double r = rTop / Bot;
            double s = sTop / Bot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2D.Vec2DDistance(A, B) * r;

                return true;
            }

            else
            {
                dist = 0;

                return false;
            }
        }

        //-------------------- LineIntersection2D-------------------------
        //
        //	Given 2 lines in 2D space AB, CD this returns true if an 
        //	intersection occurs and sets dist to the distance the intersection
        //  occurs along AB. Also sets the 2d vector point to the point of
        //  intersection
        //----------------------------------------------------------------- 
        public static bool LineIntersection2D(Vector2D A,
                                       Vector2D B,
                                       Vector2D C,
                                       Vector2D D,
                                       ref double dist,
                                       ref Vector2D point)
        {

            double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
            double rBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);
            double sBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

            if ((rBot == 0) || (sBot == 0))
            {
                //lines are parallel
                return false;
            }

            double r = rTop / rBot;
            double s = sTop / sBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2D.Vec2DDistance(A, B) * r;

                point = A + (B - A) * r;

                return true;
            }

            else
            {
                dist = 0;

                return false;
            }
        }

        //----------------------- ObjectIntersection2D ---------------------------
        //
        //  tests two polygons for intersection. *Does not check for enclosure*
        //------------------------------------------------------------------------
        public static bool ObjectIntersection2D(List<Vector2D> object1,
                                          List<Vector2D> object2)
        {

            //test each line segment of object1 against each segment of object2
            for (int r = 0; r < object1.Count - 1; ++r)
            {
                for (int t = 0; t < object2.Count - 1; ++t)
                {
                    if (LineIntersection2D(object2[t],
                                           object2[t + 1],
                                           object1[r],
                                           object1[r + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //----------------------- SegmentObjectIntersection2D --------------------
        //
        //  tests a line segment against a polygon for intersection
        //  *Does not check for enclosure*
        //------------------------------------------------------------------------
        public static bool SegmentObjectIntersection2D(Vector2D A,
                                            Vector2D B,
                                            List<Vector2D> object1)
        {
            //test AB against each segment of object
            for (int r = 0; r < object1.Count - 1; ++r)
            {
                if (LineIntersection2D(A, B, object1[r], object1[r + 1]))
                {
                    return true;
                }
            }

            return false;
        }


        //----------------------------- TwoCirclesOverlapped ---------------------
        //
        //  Returns true if the two circles overlap
        //------------------------------------------------------------------------
        public static bool TwoCirclesOverlapped(double x1, double y1, double r1,
                                  double x2, double y2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((x1 - x2) * (x1 - x2) +
                                              (y1 - y2) * (y1 - y2));

            if ((DistBetweenCenters < (r1 + r2)) || (DistBetweenCenters < Math.Abs(r1 - r2)))
            {
                return true;
            }

            return false;
        }

        //----------------------------- TwoCirclesOverlapped ---------------------
        //
        //  Returns true if the two circles overlap
        //------------------------------------------------------------------------
        public static bool TwoCirclesOverlapped(Vector2D c1, double r1,
                                  Vector2D c2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((c1.X - c2.X) * (c1.X - c2.X) +
                                              (c1.Y - c2.Y) * (c1.Y - c2.Y));

            if ((DistBetweenCenters < (r1 + r2)) || (DistBetweenCenters < Math.Abs(r1 - r2)))
            {
                return true;
            }

            return false;
        }

        //--------------------------- TwoCirclesEnclosed ---------------------------
        //
        //  returns true if one circle encloses the other
        //-------------------------------------------------------------------------
        public static bool TwoCirclesEnclosed(double x1, double y1, double r1,
                                double x2, double y2, double r2)
        {
            double DistBetweenCenters = Math.Sqrt((x1 - x2) * (x1 - x2) +
                                              (y1 - y2) * (y1 - y2));

            if (DistBetweenCenters < Math.Abs(r1 - r2))
            {
                return true;
            }

            return false;
        }

        //------------------------ TwoCirclesIntersectionPoints ------------------
        //
        //  Given two circles this function calculates the intersection points
        //  of any overlap.
        //
        //  returns false if no overlap found
        //
        // see http://astronomy.swin.edu.au/~pbourke/geometry/2circle/
        //------------------------------------------------------------------------ 
        public static bool TwoCirclesIntersectionPoints(double x1, double y1, double r1,
                                          double x2, double y2, double r2,
                                          ref double p3X, ref double p3Y,
                                          ref double p4X, ref double p4Y)
        {
            //first check to see if they overlap
            if (!TwoCirclesOverlapped(x1, y1, r1, x2, y2, r2))
            {
                return false;
            }

            //calculate the distance between the circle centers
            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            //Now calculate the distance from the center of each circle to the center
            //of the line which connects the intersection points.
            double a = (r1 - r2 + (d * d)) / (2 * d);
            double b = (r2 - r1 + (d * d)) / (2 * d);


            //MAYBE A TEST FOR EXACT OVERLAP? 

            //calculate the point P2 which is the center of the line which 
            //connects the intersection points
            double p2X, p2Y;

            p2X = x1 + a * (x2 - x1) / d;
            p2Y = y1 + a * (y2 - y1) / d;

            //calculate first point
            double h1 = Math.Sqrt((r1 * r1) - (a * a));

            p3X = p2X - h1 * (y2 - y1) / d;
            p3Y = p2Y + h1 * (x2 - x1) / d;


            //calculate second point
            double h2 = Math.Sqrt((r2 * r2) - (a * a));

            p4X = p2X + h2 * (y2 - y1) / d;
            p4Y = p2Y - h2 * (x2 - x1) / d;

            return true;

        }

        //------------------------ TwoCirclesIntersectionArea --------------------
        //
        //  Tests to see if two circles overlap and if so calculates the area
        //  defined by the union
        //
        // see http://mathforum.org/library/drmath/view/54785.html
        //-----------------------------------------------------------------------
        public static double TwoCirclesIntersectionArea(double x1, double y1, double r1,
                                          double x2, double y2, double r2)
        {
            //first calculate the intersection points
            double iX1 = 0.0;
            double iY1 = 0.0;
            double iX2 = 0.0;
            double iY2 = 0.0;

            if (!TwoCirclesIntersectionPoints(x1, y1, r1, x2, y2, r2, ref iX1, ref iY1, ref iX2, ref iY2))
            {
                return 0.0; //no overlap
            }

            //calculate the distance between the circle centers
            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            //find the angles given that A and B are the two circle centers
            //and C and D are the intersection points
            double CBD = 2 * Math.Acos((r2 * r2 + d * d - r1 * r1) / (r2 * d * 2));

            double CAD = 2 * Math.Acos((r1 * r1 + d * d - r2 * r2) / (r1 * d * 2));


            //Then we find the segment of each of the circles cut off by the 
            //chord CD, by taking the area of the sector of the circle BCD and
            //subtracting the area of triangle BCD. Similarly we find the area
            //of the sector ACD and subtract the area of triangle ACD.

            double area = 0.5f * CBD * r2 * r2 - 0.5f * r2 * r2 * Math.Sin(CBD) +
                          0.5f * CAD * r1 * r1 - 0.5f * r1 * r1 * Math.Sin(CAD);

            return area;
        }

        //-------------------------------- CircleArea ---------------------------
        //
        //  given the radius, calculates the area of a circle
        //-----------------------------------------------------------------------
        public static double CircleArea(double radius)
        {
            return Math.PI * radius * radius;
        }


        //----------------------- PointInCircle ----------------------------------
        //
        //  returns true if the point p is within the radius of the given circle
        //------------------------------------------------------------------------
        public static bool PointInCircle(Vector2D Pos,
                                  double radius,
                                  Vector2D p)
        {
            double DistFromCenterSquared = (p - Pos).LengthSq();

            if (DistFromCenterSquared < (radius * radius))
            {
                return true;
            }

            return false;
        }

        //--------------------- LineSegmentCircleIntersection ---------------------------
        //
        //  returns true if the line segemnt AB intersects with a circle at
        //  position P with radius radius
        //------------------------------------------------------------------------
        public static bool LineSegmentCircleIntersection(Vector2D A,
                                                    Vector2D B,
                                                    Vector2D P,
                                                    double radius)
        {
            //first determine the distance from the center of the circle to
            //the line segment (working in distance squared space)
            double DistToLineSq = DistToLineSegmentSq(A, B, P);

            if (DistToLineSq < radius * radius)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        //------------------- GetLineSegmentCircleClosestIntersectionPoint ------------
        //
        //  given a line segment AB and a circle position and radius, this function
        //  determines if there is an intersection and stores the position of the 
        //  closest intersection in the reference IntersectionPoint
        //
        //  returns false if no intersection point is found
        //-----------------------------------------------------------------------------
        public static bool GetLineSegmentCircleClosestIntersectionPoint(Vector2D A,
                                                                 Vector2D B,
                                                                 Vector2D pos,
                                                                 double radius,
                                                                 ref Vector2D IntersectionPoint)
        {
            Vector2D toBNorm = Vector2D.Vec2DNormalize(B - A);

            //move the circle into the local space defined by the vector B-A with origin
            //at A
            Vector2D LocalPos = PointToLocalSpace(pos, toBNorm, toBNorm.Perp(), A);

            bool ipFound = false;

            //if the local position + the radius is negative then the circle lays behind
            //point A so there is no intersection possible. If the local x pos minus the 
            //radius is greater than length A-B then the circle cannot intersect the 
            //line segment
            if ((LocalPos.X + radius >= 0) &&
               ((LocalPos.X - radius) * (LocalPos.X - radius) <= Vector2D.Vec2DDistanceSq(B, A)))
            {
                //if the distance from the x axis to the object's position is less
                //than its radius then there is a potential intersection.
                if (Math.Abs(LocalPos.Y) < radius)
                {
                    //now to do a line/circle intersection test. The center of the 
                    //circle is represented by A, B. The intersection points are 
                    //given by the formulae x = A +/-Math.Sqrt(r^2-B^2), y=0. We only 
                    //need to look at the smallest positive value of x.
                    double a = LocalPos.X;
                    double b = LocalPos.Y;

                    double ip = a - Math.Sqrt(radius * radius - b * b);

                    if (ip <= 0)
                    {
                        ip = a + Math.Sqrt(radius * radius - b * b);
                    }

                    ipFound = true;

                    IntersectionPoint = A + toBNorm * ip;
                }
            }

            return ipFound;
        }

        #endregion

        #region " Entity Functions "

        //------------------------- Overlapped -----------------------------------
        //
        //  tests to see if an entity is overlapping any of a number of entities
        //  stored in a std container
        //------------------------------------------------------------------------
        public static bool Overlapped(BaseGameEntity ob, List<BaseGameEntity> conOb, double MinDistBetweenObstacles)
        {
            foreach (BaseGameEntity it in conOb)
            {
                if (TwoCirclesOverlapped(ob.Pos, ob.BRadius + MinDistBetweenObstacles,
                                         it.Pos, it.BRadius))
                {
                    return true;
                }
            }

            return false;
        }

        //-------------------- GetEntityLineSegmentIntersections ----------------------
        //
        //  tests a line segment AB against a container of entities. First of all
        //  a test is made to confirm that the entity is within a specified range of 
        //  the one_to_ignore (positioned at A). If within range the intersection test
        //  is made.
        //
        //  returns a list of all the entities that tested positive for intersection
        //-----------------------------------------------------------------------------
        public static List<BaseGameEntity> GetEntityLineSegmentIntersections(List<BaseGameEntity> entities,
                                                       int the_one_to_ignore,
                                                       Vector2D A,
                                                       Vector2D B,
                                                       double range)
        {
            List<BaseGameEntity> hits = new List<BaseGameEntity>();

            //iterate through all entities checking against the line segment AB
            foreach (BaseGameEntity it in entities)
            {
                //if not within range or the entity being checked is the_one_to_ignore
                //just continue with the next entity
                if ((it.ID() == the_one_to_ignore) ||
                   (Vector2D.Vec2DDistanceSq(it.Pos, A) > range * range))
                {
                    continue;
                }

                //if the distance to AB is less than the entities bounding radius then
                //there is an intersection so add it to hits
                if (DistToLineSegment(A, B, it.Pos) < it.BRadius)
                {
                    hits.Add(it);
                }

            }

            return hits;
        }

        //------------------------ GetClosestEntityLineSegmentIntersection ------------
        //
        //  tests a line segment AB against a container of entities. First of all
        //  a test is made to confirm that the entity is within a specified range of 
        //  the one_to_ignore (positioned at A). If within range the intersection test
        //  is made.
        //
        //  returns the closest entity that tested positive for intersection or NULL
        //  if none found
        //-----------------------------------------------------------------------------
        public static BaseGameEntity GetClosestEntityLineSegmentIntersection(List<BaseGameEntity> entities,
                                                  int the_one_to_ignore,
                                                  Vector2D A,
                                                  Vector2D B,
                                                  double range)
        {
            BaseGameEntity ClosestEntity = null;

            double ClosestDist = Double.MaxValue;

            //iterate through all entities checking against the line segment AB
            foreach (BaseGameEntity it in entities)
            {
                double distSq = Vector2D.Vec2DDistanceSq(it.Pos, A);

                //if not within range or the entity being checked is the_one_to_ignore
                //just continue with the next entity
                if ((it.ID() == the_one_to_ignore) || (distSq > range * range))
                {
                    continue;
                }

                //if the distance to AB is less than the entities bounding radius then
                //there is an intersection so add it to hits
                if (DistToLineSegment(A, B, it.Pos) < it.BRadius)
                {
                    if (distSq < ClosestDist)
                    {
                        ClosestDist = distSq;

                        ClosestEntity = it;
                    }
                }

            }

            return ClosestEntity;
        }


        #endregion

    }
}
