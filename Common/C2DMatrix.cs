using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Common
{
    public class C2DMatrix
    {
        private struct Matrix
        {
            public double m_11, m_12, m_13;
            public double m_21, m_22, m_23;
            public double m_31, m_32, m_33;

            #region Properties
            public double _11
            {
                get { return m_11; }
                set { m_11 = value; }
            }

            public double _12
            {
                get { return m_12; }
                set { m_12 = value; }
            }

            public double _13
            {
                get { return m_13; }
                set { m_13 = value; }
            }

            public double _21
            {
                get { return m_21; }
                set { m_21 = value; }
            }

            public double _22
            {
                get { return m_22; }
                set { m_22 = value; }
            }

            public double _23
            {
                get { return m_23; }
                set { m_23 = value; }
            }

            public double _31
            {
                get { return m_31; }
                set { m_31 = value; }
            }

            public double _32
            {
                get { return m_32; }
                set { m_32 = value; }
            }

            public double _33
            {
                get { return m_33; }
                set { m_33 = value; }
            }

            #endregion
        };

        private Matrix m_Matrix;

        //multiply two matrices together
        private void MatrixMultiply(Matrix mIn)
        {
          Matrix mat_temp = new Matrix();
          
          //first row
          mat_temp._11 = (m_Matrix._11*mIn._11) + (m_Matrix._12*mIn._21) + (m_Matrix._13*mIn._31);
          mat_temp._12 = (m_Matrix._11*mIn._12) + (m_Matrix._12*mIn._22) + (m_Matrix._13*mIn._32);
          mat_temp._13 = (m_Matrix._11*mIn._13) + (m_Matrix._12*mIn._23) + (m_Matrix._13*mIn._33);

          //second
          mat_temp._21 = (m_Matrix._21*mIn._11) + (m_Matrix._22*mIn._21) + (m_Matrix._23*mIn._31);
          mat_temp._22 = (m_Matrix._21*mIn._12) + (m_Matrix._22*mIn._22) + (m_Matrix._23*mIn._32);
          mat_temp._23 = (m_Matrix._21*mIn._13) + (m_Matrix._22*mIn._23) + (m_Matrix._23*mIn._33);

          //third
          mat_temp._31 = (m_Matrix._31*mIn._11) + (m_Matrix._32*mIn._21) + (m_Matrix._33*mIn._31);
          mat_temp._32 = (m_Matrix._31*mIn._12) + (m_Matrix._32*mIn._22) + (m_Matrix._33*mIn._32);
          mat_temp._33 = (m_Matrix._31*mIn._13) + (m_Matrix._32*mIn._23) + (m_Matrix._33*mIn._33);

          m_Matrix = mat_temp;
        }

        public C2DMatrix()
        {
            //initialize the matrix to an identity matrix
            Identity();
        }
        
        //accessors to the matrix elements
        public void _11(double val) { m_Matrix._11 = val; }
        public void _12(double val) { m_Matrix._12 = val; }
        public void _13(double val) { m_Matrix._13 = val; }

        public void _21(double val) { m_Matrix._21 = val; }
        public void _22(double val) { m_Matrix._22 = val; }
        public void _23(double val) { m_Matrix._23 = val; }

        public void _31(double val) { m_Matrix._31 = val; }
        public void _32(double val) { m_Matrix._32 = val; }
        public void _33(double val) { m_Matrix._33 = val; }

        //create an identity matrix
        public void Identity()
        {
            m_Matrix._11 = 1; m_Matrix._12 = 0; m_Matrix._13 = 0;    
            m_Matrix._21 = 0; m_Matrix._22 = 1; m_Matrix._23 = 0;
            m_Matrix._31 = 0; m_Matrix._32 = 0; m_Matrix._33 = 1;
        }

        //applies a 2D transformation matrix to a list of Vector2Ds
        public void TransformVector2Ds(List<Vector2D> vPoints)
        {
            for (int i = 0; i < vPoints.Count; ++i)
            {
                double tempX = (m_Matrix._11 * vPoints[i].X) + (m_Matrix._21 * vPoints[i].Y) + (m_Matrix._31);
                double tempY = (m_Matrix._12 * vPoints[i].X) + (m_Matrix._22 * vPoints[i].Y) + (m_Matrix._32);

                vPoints[i].X = tempX;
                vPoints[i].Y = tempY;
            }
        }

        //applies a 2D transformation matrix to a single Vector2D
        public void TransformVector2D(Vector2D vPoint)
        {
            double tempX = (m_Matrix._11 * vPoint.X) + (m_Matrix._21 * vPoint.Y) + (m_Matrix._31);
            double tempY = (m_Matrix._12 * vPoint.X) + (m_Matrix._22 * vPoint.Y) + (m_Matrix._32);

            vPoint.X = tempX;
            vPoint.Y = tempY;
        }


        //create a transformation matrix
        public void Translate(double x, double y)
        {
            Matrix mat = new Matrix();
          
            mat._11 = 1; 
            mat._12 = 0; 
            mat._13 = 0;
              
            mat._21 = 0; 
            mat._22 = 1; 
            mat._23 = 0;
              
            mat._31 = x;    
            mat._32 = y;   
            mat._33 = 1;
          
            //and multiply
            MatrixMultiply(mat);
        }

        //create a scale matrix
        public void Scale(double xScale, double yScale)
        {
            Matrix mat = new Matrix();

            mat._11 = xScale; 
            mat._12 = 0; 
            mat._13 = 0;

            mat._21 = 0; 
            mat._22 = yScale; 
            mat._23 = 0;

            mat._31 = 0; 
            mat._32 = 0; 
            mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //create a rotation matrix
        public void Rotate(double rot)
        {
            Matrix mat = new Matrix();

            double _Sin = Math.Sin(rot);
            double _Cos = Math.Cos(rot);

            mat._11 = _Cos;
            mat._12 = _Sin; 
            mat._13 = 0;

            mat._21 = -_Sin;
            mat._22 = _Cos; 
            mat._23 = 0;

            mat._31 = 0;
            mat._32 = 0;
            mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //create a rotation matrix from a 2D vector
        public void Rotate(Vector2D fwd, Vector2D side)
        {
            Matrix mat = new Matrix();

            mat._11 = fwd.X;  
            mat._12 = fwd.Y; 
            mat._13 = 0;
            mat._21 = side.X; 
            mat._22 = side.Y; 
            mat._23 = 0;
            mat._31 = 0; 
            mat._32 = 0;
            mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

    }
}
