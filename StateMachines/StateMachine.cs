using System;
using System.Text;
using System.Diagnostics;
using ThinkSharp.Common;

namespace ThinkSharp.StateMachines
{
    public class StateMachine
    {
        //a pointer to the agent that owns this instance
        private BaseGameEntity m_pOwner;

        private State m_pCurrentState;

        //a record of the last state the agent was in
        private State m_pPreviousState;

        //this is called every time the FSM is updated
        private State m_pGlobalState;

        public StateMachine(BaseGameEntity owner)
        {
            m_pOwner = owner;
            m_pCurrentState = null;
            m_pPreviousState = null;
            m_pGlobalState = null;
        }

        public State CurrentState
        {
            get { return m_pCurrentState; }
            set { m_pCurrentState = value; }
        }

        public State GlobalState
        {
            get { return m_pGlobalState; }
            set { m_pGlobalState = value; }
        }

        public State PreviousState
        {
            get { return m_pPreviousState; }
            set { m_pPreviousState = value; }
        }

        //call this to update the FSM
        public void Update()
        {
            //if a global state exists, call its execute method, else do nothing
            if (m_pGlobalState != null) m_pGlobalState.Execute(m_pOwner);

            //same for the current state
            if (m_pCurrentState != null) m_pCurrentState.Execute(m_pOwner);
        }

        public bool HandleMessage(Telegram msg)
        {
            //first see if the current state is valid and that it can handle
            //the message
            if ((m_pCurrentState != null) && m_pCurrentState.OnMessage(m_pOwner, msg))
            {
                return true;
            }
            //if not, and if a global state has been implemented, send 
            //the message to the global state
            if ((m_pGlobalState != null) && m_pGlobalState.OnMessage(m_pOwner, msg))
            {
                return true;
            }

            return false;
        }

        //change to a new state
        public void ChangeState(State pNewState)
        {
            Debug.Assert(pNewState != null, "<StateMachine::ChangeState>:trying to assign null state to current");

            //keep a record of the previous state
            m_pPreviousState = m_pCurrentState;

            //call the exit method of the existing state
            m_pCurrentState.Exit(m_pOwner);

            //change state to the new state
            m_pCurrentState = pNewState;

            //call the entry method of the new state
            m_pCurrentState.Enter(m_pOwner);
        }

        //change state back to the previous state
        public void RevertToPreviousState()
        {
            ChangeState(m_pPreviousState);
        }

        public bool isInState(Type typeState)
        {
            if (m_pCurrentState.GetType() == typeState) return true;
            return false;
        }

        //only ever used during debugging to grab the name of the current state
        internal string GetNameOfCurrentState()
        {
            string s = m_pCurrentState.GetType().Name;

            //remove the 'class ' part from the front of the string
            if (s.Length > 5)
            {
                s.Remove(0, 6);
            }

            return s;
        }

    }
}
