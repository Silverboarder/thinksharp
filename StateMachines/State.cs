using System;
using ThinkSharp.Common;

namespace ThinkSharp.StateMachines
{
    public abstract class State
    {
        //this will execute when the state is entered
        public abstract void Enter(BaseGameEntity pEntity);

        //this is the states normal update function
        public abstract void Execute(BaseGameEntity pEntity);

        //this will execute when the state is exited. 
        public abstract void Exit(BaseGameEntity pEntity);

        //this executes if the agent receives a message from the 
        //message dispatcher
        public abstract bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram);
    }
}
