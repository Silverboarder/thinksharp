

using System;
using System.Drawing;

namespace ThinkSharp.Steering
{
    /// <summary>
    /// Summary description for Vector2D.
    /// </summary>
    public class Vector2D
    {
        private double x, y;        

        public Vector2D()
        {
            this.X = 0;
            this.Y = 0;
        }

        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        #region Properties
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }
        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            if (obj is Vector2D)
            {
                Vector2D v = (Vector2D)obj;
                if (v.X == x && v.Y == y)
                    return obj.GetType().Equals(this.GetType());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{{X={0}, Y={1}}}", x, y);
        }
        #endregion

        public double NormOriginal()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static bool operator ==(Vector2D u, Vector2D v)
        {
            if (u.X == v.X && u.Y == v.Y)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector2D u, Vector2D v)
        {
            return u != v;
        }

        public static Vector2D operator +(Vector2D u, Vector2D v)
        {
            return new Vector2D(u.X + v.x, u.Y + v.Y);
        }

        public static Vector2D operator -(Vector2D u, Vector2D v)
        {
            return new Vector2D(u.X - v.x, u.Y - v.Y);
        }

        public static Vector2D operator *(Vector2D u, double a)
        {
            return new Vector2D(a * u.x, a * u.Y);
        }

        public static Vector2D operator /(Vector2D u, double a)
        {
            return new Vector2D(u.X / a, u.Y / a);
        }

        public static Vector2D operator -(Vector2D u)
        {
            return new Vector2D(-u.x, -u.Y);
        }

        public static explicit operator PointF(Vector2D u)
        {
            return new PointF((float)u.x, (float)u.Y);
        }

        public static implicit operator Vector2D(PointF p)
        {
            return new Vector2D(p.X, p.Y);
        }

        //sets x and y to zero
        public void Zero() { x = 0.0; y = 0.0; }

        //returns true if both x and y are zero
        public bool isZero()
        {
            return (x*x + y*y) < Double.MinValue;
        }

        //------------------------- Length ---------------------------------------
        //
        //  returns the length of a 2D vector
        //------------------------------------------------------------------------
        public double Length()
        {
          return Math.Sqrt(x * x + y * y);
        }

        //------------------------- LengthSq -------------------------------------
        //
        //  returns the squared length of a 2D vector
        //------------------------------------------------------------------------
        public double LengthSq()
        {
          return (x * x + y * y);
        }

        //------------------------- Vec2DDot -------------------------------------
        //
        //  calculates the dot product
        //------------------------------------------------------------------------
        public double Dot(Vector2D v2)
        {
          return x * v2.X + y * v2.y;
        }

        //------------------------ Sign ------------------------------------------
        //
        //  returns positive if v2 is clockwise of this vector,
        //  minus if anticlockwise (Y axis pointing down, X axis to right)
        //------------------------------------------------------------------------
        public enum Enum_Sign {clockwise = 1, anticlockwise = -1};

        public int Sign(Vector2D v2)
        {
          if (y*v2.X > x*v2.Y)
          {
              return (int)Enum_Sign.anticlockwise;
          }
          else 
          {
              return (int)Enum_Sign.clockwise;
          }
        }

        //------------------------------ Perp ------------------------------------
        //
        //  Returns a vector perpendicular to this vector
        //------------------------------------------------------------------------
        public Vector2D Perp()
        {
            return new Vector2D(-y, x);
        }

        //------------------------------ Distance --------------------------------
        //
        //  calculates the euclidean distance between two vectors
        //------------------------------------------------------------------------
        public double Distance(Vector2D v2)
        {
          double ySeparation = v2.Y - y;
          double xSeparation = v2.X - x;

          return Math.Sqrt(ySeparation*ySeparation + xSeparation*xSeparation);
        }

        //------------------------------ DistanceSq ------------------------------
        //
        //  calculates the euclidean distance squared between two vectors 
        //------------------------------------------------------------------------
        public double DistanceSq(Vector2D v2)
        {
          double ySeparation = v2.Y - y;
          double xSeparation = v2.X - x;

          return ySeparation*ySeparation + xSeparation*xSeparation;
        }

        //------------------------- Normalize ------------------------------------
        //
        //  normalizes a 2D Vector
        //------------------------------------------------------------------------
        public void Normalize()
        { 
            double vector_length = this.Length();

            if (vector_length > double.Epsilon)
            {
                this.X /= vector_length;
                this.Y /= vector_length;
            }
        }

        //----------------------------- Truncate ---------------------------------
        //
        //  truncates a vector so that its length does not exceed max
        //------------------------------------------------------------------------
        public void Truncate(double max)
        {
            if (this.Length() > max)
            {
                this.Normalize();
                this.X = this.X * max;
                this.Y = this.Y * max;
            } 
        }        

        //----------------------- GetReverse ----------------------------------------
        //
        //  returns the vector that is the reverse of this vector
        //------------------------------------------------------------------------
        public Vector2D GetReverse()
        {
            return new Vector2D(-this.x, -this.Y);
        }

        //--------------------------- Reflect ------------------------------------
        //
        //  given a normalized vector this method reflects the vector it
        //  is operating upon. (like the path of a ball bouncing off a wall)
        //------------------------------------------------------------------------
        public void Reflect(Vector2D norm)
        {
            //this += 2.0 * this.Dot(norm) * norm.GetReverse();

            Vector2D vecTemp = norm.GetReverse() * this.Dot(norm) * 2.0;            

            this.X = vecTemp.X;
            this.Y = vecTemp.Y;
        }

    #region " non member functions "

        public static bool IsNull(Vector2D v)
        {
            if ((object)v == null) return true;
            return false;
        }

        public static Vector2D Vec2DNormalize(Vector2D v)
        {
          Vector2D vec = new Vector2D(v.X, v.Y);

          double vector_length = vec.Length();

          if (vector_length > double.Epsilon)
          {
            vec.X /= vector_length;
            vec.Y /= vector_length;
          }

          return vec;
        }

        public static double Vec2DDistance(Vector2D v1, Vector2D v2)
        {

          double ySeparation = v2.Y - v1.y;
          double xSeparation = v2.X - v1.x;

          return Math.Sqrt(ySeparation*ySeparation + xSeparation*xSeparation);
        }

        public static double Vec2DDistanceSq(Vector2D v1, Vector2D v2)
        {

          double ySeparation = v2.Y - v1.y;
          double xSeparation = v2.X - v1.x;

          return ySeparation*ySeparation + xSeparation*xSeparation;
        }

        public static double Vec2DLength(Vector2D v)
        {
          return Math.Sqrt(v.x*v.X + v.y*v.Y);
        }

        public static double Vec2DLengthSq(Vector2D v)
        {
          return (v.x*v.X + v.y*v.Y);
        }

        //treats a window as a toroid
        public static Vector2D WrapAround(Vector2D pos, int MaxX, int MaxY)
        {
            Vector2D newPos = pos;

            if (pos.X > MaxX) { newPos.X = 0.0; }

            if (pos.X < 0) { newPos.X = (double)MaxX; }

            if (pos.Y < 0) { newPos.Y = (double)MaxY; }

            if (pos.Y > MaxY) { newPos.Y = 0.0; }

            return newPos;
        }

        //returns true if the point p is not inside the region defined by top_left and bot_rgt
        public static bool NotInsideRegion(Vector2D p,
                                    Vector2D top_left,
                                    Vector2D bot_rgt)
        {
          return (p.X < top_left.X) || (p.X > bot_rgt.X) || 
                 (p.Y < top_left.Y) || (p.Y > bot_rgt.Y);
        }

        public static bool InsideRegion(Vector2D p,
                                 Vector2D top_left,
                                 Vector2D bot_rgt)
        {
          return !((p.X < top_left.X) || (p.X > bot_rgt.X) || 
                 (p.Y < top_left.Y) || (p.Y > bot_rgt.Y));
        }

        public static bool InsideRegion(Vector2D p, int left, int top, int right, int bottom)
        {
          return !( (p.X < left) || (p.X > right) || (p.Y < top) || (p.Y > bottom) );
        }

        //------------------ isSecondInFOVOfFirst -------------------------------------
        //
        //  returns true if the target position is in the field of view of the entity
        //  positioned at posFirst facing in facingFirst
        //-----------------------------------------------------------------------------
        public static bool isSecondInFOVOfFirst(Vector2D posFirst,
                                         Vector2D facingFirst,
                                         Vector2D posSecond,
                                         double    fov)
        {
          Vector2D toTarget = Vec2DNormalize(posSecond - posFirst);

          return facingFirst.Dot(toTarget) >= Math.Cos(fov/2.0);
        }

    #endregion        

    }
}

