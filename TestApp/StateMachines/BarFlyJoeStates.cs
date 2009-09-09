using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;
using System.Diagnostics;

namespace TestApp
{
    public class JoeDoChillin : State
    {

        public JoeDoChillin()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;

            // reset Stats
            joe.Drunkeness = -joe.Drunkeness;
            joe.Angered = false;

            DebugMessages.Instance.WriteLine(String.Format("{0}: Time to do some chillin'!", MainSM.GetEntityName(pEntity.ID())));
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;

            if (!joe.Angered)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: Ahh, jus' drinkin'", MainSM.GetEntityName(pEntity.ID())));
                joe.Drunkeness = joe.Drunkeness + 1;

                if (joe.Drunkeness > BarFlyJoe.AlchoAngerSwitch)
                {
                    DebugMessages.Instance.WriteLine(String.Format("{0}: Feelin drunk and angry! I need a fight...", MainSM.GetEntityName(pEntity.ID())));

                    joe.Angered = true;
                }
            }
           
            if (joe.Angered)
            {
                Miner bob = (Miner)MessageDispatcher.Instance.GetRegisteredEntityFromID((int)EntityName.ent_Miner_Bob);

                if (bob.Location != location_type.saloon)
                {
                    if (Utils.RandFloat() <= 0.5)
                    {
                        DebugMessages.Instance.WriteLine(String.Format("{0}: Damnit no one here to fight with!", MainSM.GetEntityName(pEntity.ID())));
                    }
                    else
                    {
                        DebugMessages.Instance.WriteLine(String.Format("{0}: Burp! Grr Im not a happy *hic* cowboy.", MainSM.GetEntityName(pEntity.ID())));
                    }
                }
                else
                {
                    joe.GetFSM().ChangeState(new JoeDoAntagonizin());
                }
            }      

        }

        public override void Exit(BaseGameEntity pEntity)
        {
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class JoeDoAntagonizin : State
    {

        public JoeDoAntagonizin()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Time to pick a fight!", MainSM.GetEntityName(pEntity.ID())));

            MessageDispatcher.Instance.DispatchMessage((int)MessageDispatcher.SEND_MSG_IMMEDIATELY, 
                                      pEntity.ID(),
                                      (int)EntityName.ent_Miner_Bob,
                                      (int)message_type.Msg_Antagonize,
                                      (int)MessageDispatcher.NO_ADDITIONAL_INFO);
        }

        public override void Execute(BaseGameEntity pEntity)
        {
        }

        public override void Exit(BaseGameEntity pEntity)
        {
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;

            switch (pTelegram.Msg)
            {
                case (int)message_type.Msg_DeclineFight:
                    {
                        DebugMessages.Instance.WriteLine(String.Format("{0}: Har har, ya'll a bunch o' yellow bellied chickens!", MainSM.GetEntityName(pEntity.ID())));

                        joe.GetFSM().ChangeState(new JoeDoChillin());

                        return true;
                    }

                case (int)message_type.Msg_AcceptFight:
                    {
                        joe.GetFSM().ChangeState(new JoeFight());

                        return true;
                    }
            }

            return false;
        }

    }

    public class JoeFight : State
    {
        public JoeFight()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Get over here!", MainSM.GetEntityName(pEntity.ID())));
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;

            if (joe.HP == 1)
            {
                // Lost fight! Tell Bob we are done.
                DebugMessages.Instance.WriteLine(String.Format("{0}: Ok ok Im beat! Mercy!", MainSM.GetEntityName(pEntity.ID())));

                MessageDispatcher.Instance.DispatchMessage((int)MessageDispatcher.SEND_MSG_IMMEDIATELY,
                                          pEntity.ID(),
                                          (int)EntityName.ent_Miner_Bob,
                                          (int)message_type.Msg_DeclineFight,
                                          (int)MessageDispatcher.NO_ADDITIONAL_INFO);                

                joe.GetFSM().ChangeState(new JoeDoChillin());
            }
            else
            {
                // Throw a punch
                DebugMessages.Instance.WriteLine(String.Format("{0}: Eat mah knuckle!", MainSM.GetEntityName(pEntity.ID())));

                MessageDispatcher.Instance.DispatchMessage((int)MessageDispatcher.SEND_MSG_IMMEDIATELY,
                                          pEntity.ID(),
                                          (int)EntityName.ent_Miner_Bob,
                                          (int)message_type.Msg_IncomingPunch,
                                          (int)MessageDispatcher.NO_ADDITIONAL_INFO);                

            }

        }

        public override void Exit(BaseGameEntity pEntity)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;
            joe.HP = Miner.HPFull;
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            BarFlyJoe joe = (BarFlyJoe)pEntity;

            switch (pTelegram.Msg)
            {
                case (int)message_type.Msg_IncomingPunch:
                    {
                        if (Utils.RandFloat() <= 0.5)
                        {
                            joe.HP = joe.HP - 1;

                            DebugMessages.Instance.WriteLine(String.Format("{0}: Gaah Im hit!", MainSM.GetEntityName(pEntity.ID())));
                        }
                        else
                        {
                            DebugMessages.Instance.WriteLine(String.Format("{0}: Missed me, sucker!", MainSM.GetEntityName(pEntity.ID())));
                        }

                        return true;
                    }
                case (int)message_type.Msg_DeclineFight:
                    {
                        DebugMessages.Instance.WriteLine(String.Format("{0}: That'll teach ya to mess with an alchoholic!", MainSM.GetEntityName(pEntity.ID())));

                        joe.GetFSM().ChangeState(new JoeDoChillin());

                        return true;
                    }
            }

            return false;
        }

    }

   }
