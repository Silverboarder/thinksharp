using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkSharp.Common
{
    public class BaseGameEntity
    {
        public const int default_entity_type = -1;

        //each entity has a unique ID
        private int m_ID;

        //every entity has a type associated with it (health, troll, ammo etc)
        private int m_EntityType;

        //this is a generic flag. 
        private bool m_bTag;

        private static int NextID = 0;

        //used by the constructor to give each entity a unique ID
        private int NextValidID()
        {             
            return NextID++;
        }

        //its location in the environment
        protected Vector2D m_vPos;
        protected Vector2D m_vScale;

        //the length of this object's bounding radius
        protected double m_dBoundingRadius;

        public virtual bool HandleMessage(object objTelegram){return false;}

        public BaseGameEntity()
        {             
            m_ID = NextValidID();
            m_dBoundingRadius=0.0;
            m_vPos = new Vector2D();
            m_vScale = new Vector2D(1.0,1.0);
            m_EntityType = default_entity_type;
            m_bTag = false;
        }

        public BaseGameEntity(int entity_type)
        {
            m_ID = NextValidID();
            m_dBoundingRadius=0.0;
            m_vPos = new Vector2D();
            m_vScale = new Vector2D(1.0,1.0);
            m_EntityType = entity_type;
            m_bTag = false;
        }

        public BaseGameEntity(int entity_type, Vector2D pos, double r)
        {
            m_ID = NextValidID();
            m_dBoundingRadius = r;
            m_vPos = pos;
            m_vScale = new Vector2D(1.0, 1.0);
            m_EntityType = entity_type;
            m_bTag = false;
        }

        //this can be used to create an entity with a 'forced' ID. It can be used
        //when a previously created entity has been removed and deleted from the
        //game for some reason. For example, The Raven map editor uses this ctor 
        //in its undo/redo operations. 
        //USE WITH CAUTION!
        public BaseGameEntity(int entity_type, int ForcedID)
        {
            m_ID = ForcedID;
            m_dBoundingRadius = 0.0;
            m_vPos = new Vector2D();
            m_vScale = new Vector2D(1.0, 1.0);
            m_EntityType = entity_type;
            m_bTag = false;
        }
      
        public Vector2D Pos
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public double BRadius
        {
            get { return m_dBoundingRadius; }
            set { m_dBoundingRadius = value; }
        }

        public int ID()
        {
            return m_ID; 
        }

        public int EntityType
        {
            get { return m_EntityType; }
            set { m_EntityType = value; }
        }

        public bool IsTagged() {return m_bTag;}
        public void Tag(){m_bTag = true;}
        public void UnTag(){m_bTag = false;}

        public Vector2D Scale() {return m_vScale;}
        public void SetScale(Vector2D val) 
        { 
            m_dBoundingRadius *= Math.Max(val.X, val.Y) / Math.Max(m_vScale.X, m_vScale.Y); 
            m_vScale = val; 
        }
        public void SetScale(double val) 
        { 
            m_dBoundingRadius *= (val / Math.Max(m_vScale.X, m_vScale.Y)); 
            m_vScale = new Vector2D(val, val); 
        } 

    }
}
