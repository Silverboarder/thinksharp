using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    public class Wall2D
    {
        protected Vector2D m_vA;
        protected Vector2D m_vB;
        protected Vector2D m_vN; // the wall normal

        protected void CalculateNormal()
        {
            Vector2D temp = Vector2D.Vec2DNormalize(m_vB - m_vA);

            m_vN.X = -temp.Y;
            m_vN.Y = temp.X;
        }

        public Wall2D()
        {
            m_vN = new Vector2D();
        }

        public Wall2D(Vector2D A, Vector2D B)
        {
            m_vA = A;
            m_vB = B;

            m_vN = new Vector2D();
            CalculateNormal();
        }

        public Wall2D(Vector2D A, Vector2D B, Vector2D N)
        { 
            m_vA = A;
            m_vB = B;
            m_vN = N;
        }        

        public Vector2D From
        {
            get { return m_vA; }
            set 
            { 
                m_vA = value; 
                CalculateNormal();
            }
        }

        public Vector2D To
        {
            get { return m_vB; }
            set 
            { 
                m_vB = value; 
                CalculateNormal();
            }
        }

        public Vector2D Normal
        {
            get { return m_vN; }
            set 
            { 
                m_vN = value; 
            }
        }

        public Vector2D Center() { return (m_vA + m_vB) * 0.5; }
    }

}
