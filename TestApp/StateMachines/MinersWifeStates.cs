using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;
using System.Diagnostics;

namespace TestApp
{
    public class WifesGlobalState : State
    {

        public WifesGlobalState()
        { }

        public override void Execute(BaseGameEntity pEntity)
        {
            MinersWife wife = (MinersWife)pEntity;

            //1 in 10 chance of needing the bathroom (provided she is not already
            //in the bathroom)
            if ((Utils.RandFloat() < 0.1) && !wife.GetFSM().isInState(typeof(VisitBathroom)))
            {
                wife.GetFSM().ChangeState(new VisitBathroom());
            }
        }

        public override void Enter(BaseGameEntity pEntity)
        { }

        public override void Exit(BaseGameEntity pEntity)
        { }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            MinersWife wife = (MinersWife)pEntity;

            switch (pTelegram.Msg)
            {
                case (int)message_type.Msg_HiHoneyImHome:
                    {
                        double CurrentTime = MessageDispatcher.Instance.GetRunningTime();

                        DebugMessages.Instance.WriteLine(String.Format("Message handled by {0} at time: {1}", MainSM.GetEntityName(wife.ID()), CurrentTime));

                        DebugMessages.Instance.WriteLine(String.Format("{0}: Hi honey. Let me make you some of mah fine country stew", MainSM.GetEntityName(wife.ID())));

                        wife.GetFSM().ChangeState(new CookStew());

                        return true;
                    }                    
            }

            return false;

        }

    }

    public class DoHouseWork : State
    {

        public DoHouseWork()
        { }

        public override void Execute(BaseGameEntity pEntity)
        {
            switch (Utils.RandInt(0, 2))
            {
                case 0:

                    DebugMessages.Instance.WriteLine(String.Format("{0}: Moppin' the floor", MainSM.GetEntityName(pEntity.ID())));

                    break;

                case 1:

                    DebugMessages.Instance.WriteLine(String.Format("{0}: Washin' the dishes", MainSM.GetEntityName(pEntity.ID())));

                    break;

                case 2:

                    DebugMessages.Instance.WriteLine(String.Format("{0}: Makin' the bed", MainSM.GetEntityName(pEntity.ID())));

                    break;
            }
        }

        public override void Enter(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Time to do some more housework!", MainSM.GetEntityName(pEntity.ID())));
        }

        public override void Exit(BaseGameEntity pEntity)
        {

        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class VisitBathroom : State
    {

        public VisitBathroom()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Walkin' to the can. Need to powda mah pretty li'lle nose", MainSM.GetEntityName(pEntity.ID())));
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            MinersWife wife = (MinersWife)pEntity;
            DebugMessages.Instance.WriteLine(String.Format("{0}: Ahhhhhh! Sweet relief! ", MainSM.GetEntityName(pEntity.ID())));
            wife.GetFSM().RevertToPreviousState();
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Leavin' the Jon. Ah better get back to whatever ah wuz doin'", MainSM.GetEntityName(pEntity.ID())));
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class CookStew : State
    {

        public CookStew()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            MinersWife wife = (MinersWife)pEntity;

            //if not already cooking put the stew in the oven
            if (!wife.Cooking)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: Putting the stew in the oven", MainSM.GetEntityName(pEntity.ID())));

                //send a delayed message myself so that I know when to take the stew
                //out of the oven
                MessageDispatcher.Instance.DispatchMessage(1.5,                                 //time delay
                                                          wife.ID(),                            //sender ID
                                                          wife.ID(),                            //receiver ID
                                                          (int)message_type.Msg_StewReady,    //msg
                                                          (int)MessageDispatcher.NO_ADDITIONAL_INFO);

                wife.Cooking = true;
            }

        }

        public override void Execute(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Fussin' over food", MainSM.GetEntityName(pEntity.ID())));
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Puttin' the stew on the table", MainSM.GetEntityName(pEntity.ID())));
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            MinersWife wife = (MinersWife)pEntity;

            switch (pTelegram.Msg)
            {

                case (int)message_type.Msg_StewReady:
                    {
                        DebugMessages.Instance.WriteLine(String.Format("Message received by {0} at time {1}", MainSM.GetEntityName(pEntity.ID()), MessageDispatcher.Instance.GetRunningTime()));
                        DebugMessages.Instance.WriteLine(String.Format("{0}: StewReady! Lets eat", MainSM.GetEntityName(pEntity.ID())));

                        //let hubby know the stew is ready
                        MessageDispatcher.Instance.DispatchMessage((int)MessageDispatcher.SEND_MSG_IMMEDIATELY,
                                                  wife.ID(),
                                                  (int)EntityName.ent_Miner_Bob,
                                                  (int)message_type.Msg_StewReady,
                                                  (int)MessageDispatcher.NO_ADDITIONAL_INFO);

                        wife.Cooking = false;
                        wife.GetFSM().ChangeState(new DoHouseWork());

                        return true;
                    }                    

            }//end switch

            return false;
        }

    }

}
