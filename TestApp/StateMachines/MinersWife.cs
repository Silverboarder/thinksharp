using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;

namespace TestApp
{
    public class MinersWife : BaseGameEntity
    {
        private location_type m_Location;

        //an instance of the state machine class
        private StateMachine m_pStateMachine;

        //is she presently cooking?
        private bool m_bCooking;

        public MinersWife(int id)
            : base(id)
        {
            m_Location = location_type.shack;
            m_bCooking = false;

            //set up the state machine
            m_pStateMachine = new StateMachine(this);

            m_pStateMachine.CurrentState = new DoHouseWork();

            m_pStateMachine.GlobalState = new WifesGlobalState();
        }

        public StateMachine GetFSM() { return m_pStateMachine; }

        public location_type Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        public bool Cooking
        {
            get { return m_bCooking; }
            set { m_bCooking = value; }
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
