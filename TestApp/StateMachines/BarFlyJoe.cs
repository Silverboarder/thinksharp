using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;

namespace TestApp
{

    public class BarFlyJoe : BaseGameEntity
    {
        private location_type m_Location;

        //an instance of the state machine class
        private StateMachine m_pStateMachine;

        //the higher the value, the more drunk the bar fly
        private int m_iDrunkeness;

        private int m_iHP;

        public const int AlchoAngerSwitch = 5;

        public const int HPFull = 4;

        private bool m_angered;

        public BarFlyJoe(int entity_type)
            : base(entity_type, (int)EntityName.ent_BarFly)
        {
            m_Location = location_type.saloon;

            m_iDrunkeness = 0;

            m_iHP = HPFull;

            m_angered = false;

            //set up the state machine
            m_pStateMachine = new StateMachine(this);

            m_pStateMachine.CurrentState = new JoeDoChillin();

            /* NOTE, A GLOBAL STATE HAS NOT BEEN IMPLEMENTED FOR THE BarFly */
        }

        public StateMachine GetFSM() { return m_pStateMachine; }

        public location_type Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        public bool Angered
        {
            get { return m_angered; }
            set { m_angered = value; }
        } 

        public int HP
        {
            get { return m_iHP; }
            set { m_iHP = value; }
        }

        public int Drunkeness
        {
            get { return m_iDrunkeness; }
            set 
            { 
                m_iDrunkeness = value;
                if (m_iDrunkeness < 0) m_iDrunkeness = 0;
            }
        }

        public override bool HandleMessage(object msg)
        {
            return m_pStateMachine.HandleMessage((Telegram)msg);
        }

        public void Update()
        {
            m_pStateMachine.Update();
        }

    }
}
