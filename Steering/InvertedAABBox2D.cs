using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Steering
{
    public class InvertedAABBox2D
    {

        private Vector2D m_vTopLeft;
        private Vector2D m_vBottomRight;
        private Vector2D m_vCenter;
        private Rectangle m_Rectangle;

        public InvertedAABBox2D(Vector2D topLeft, Vector2D bottomRight)
        {
            m_vTopLeft = topLeft; 
            m_vBottomRight = bottomRight;
            m_vCenter = (topLeft + bottomRight) * 0.5;

            m_Rectangle = new Rectangle((int)m_vTopLeft.X, (int)m_vTopLeft.Y, (int)(m_vBottomRight.X - m_vTopLeft.X), (int)(m_vBottomRight.Y - m_vTopLeft.Y));
        }

        //returns true if the bbox described by other intersects with this one
        public bool isOverlappedWith(InvertedAABBox2D other)
        {
            return !((other.Top() > this.Bottom()) ||
            (other.Bottom() < this.Top()) ||
            (other.Left() > this.Right()) ||
            (other.Right() < this.Left()));
        }

        public Vector2D TopLeft() {return m_vTopLeft;}
        public Vector2D BottomRight() { return m_vBottomRight; }
        public Vector2D Center() { return m_vCenter; }

        public Rectangle Rect() { return m_Rectangle; }

        public double Top() { return m_vTopLeft.Y; }
        public double Left() { return m_vTopLeft.X; }
        public double Bottom() { return m_vBottomRight.Y; }
        public double Right() { return m_vBottomRight.X; }

    }
}
