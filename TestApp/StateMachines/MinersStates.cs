using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;
using System.Diagnostics;

namespace TestApp
{
    public class EnterMineAndDigForNugget : State
    {
        public EnterMineAndDigForNugget()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            if (pMiner.Location != location_type.goldmine)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: Walkin' to the goldmine", MainSM.GetEntityName(pEntity.ID())));

                pMiner.Location = location_type.goldmine;
            }
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            //Now the miner is at the goldmine he digs for gold until he
            //is carrying in excess of MaxNuggets. If he gets thirsty during
            //his digging he packs up work for a while and changes state to
            //gp to the saloon for a whiskey.
            pMiner.AddToGoldCarried(1);

            pMiner.IncreaseFatigue();

            DebugMessages.Instance.WriteLine(String.Format("{0}: Pickin' up a nugget", MainSM.GetEntityName(pEntity.ID())));

            //if enough gold mined, go and put it in the bank
            if (pMiner.PocketsFull())
            {
                pMiner.GetFSM().ChangeState(new VisitBankAndDepositGold());
            }

            if (pMiner.Thirsty())
            {
                pMiner.GetFSM().ChangeState(new QuenchThirst());
            }
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Ah'm leavin' the goldmine with mah pockets full o' sweet gold", MainSM.GetEntityName(pEntity.ID())));
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class VisitBankAndDepositGold : State
    {
        public VisitBankAndDepositGold()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            //on entry the miner makes sure he is located at the bank
            if (pMiner.Location != location_type.bank)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: Goin' to the bank. Yes siree", MainSM.GetEntityName(pEntity.ID())));

                pMiner.Location = location_type.bank;
            }
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            //deposit the gold
            pMiner.AddToWealth(pMiner.GoldCarried);

            pMiner.GoldCarried = 0;

            DebugMessages.Instance.WriteLine(String.Format("{0}: Depositing gold. Total savings now: {1}", MainSM.GetEntityName(pEntity.ID()), pMiner.Wealth));

            //wealthy enough to have a well earned rest?
            if (pMiner.Wealth >= Miner.ComfortLevel)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: WooHoo! Rich enough for now. Back home to mah li'lle lady", MainSM.GetEntityName(pEntity.ID())));

                pMiner.GetFSM().ChangeState(new GoHomeAndSleepTilRested());
            }

            //otherwise get more gold
            else
            {
                pMiner.GetFSM().ChangeState(new EnterMineAndDigForNugget());
            }
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Leavin' the bank", MainSM.GetEntityName(pEntity.ID())));
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class GoHomeAndSleepTilRested : State
    {
        public GoHomeAndSleepTilRested()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            if (pMiner.Location != location_type.shack)
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: Walkin' home", MainSM.GetEntityName(pEntity.ID())));

                pMiner.Location = location_type.shack;

                //let the wife know I'm home
                MessageDispatcher.Instance.DispatchMessage((int)MessageDispatcher.SEND_MSG_IMMEDIATELY, //time delay
                                          pMiner.ID(),        //ID of sender
                                          (int)EntityName.ent_Elsa,            //ID of recipient
                                          (int)message_type.Msg_HiHoneyImHome,   //the message
                                          (int)MessageDispatcher.NO_ADDITIONAL_INFO);
            }
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            //if miner is not fatigued start to dig for nuggets again.
            if (!pMiner.Fatigued())
            {
                DebugMessages.Instance.WriteLine(String.Format("{0}: All mah fatigue has drained away. Time to find more gold!", MainSM.GetEntityName(pEntity.ID())));

                pMiner.GetFSM().ChangeState(new EnterMineAndDigForNugget());
            }

            else
            {
                //sleep
                pMiner.DecreaseFatigue();

                DebugMessages.Instance.WriteLine(String.Format("{0}: ZZZZ... ", MainSM.GetEntityName(pEntity.ID())));
            }
        }

        public override void Exit(BaseGameEntity pEntity)
        { }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            Miner pMiner = (Miner)pEntity;

            switch (pTelegram.Msg)
            {
                case (int)message_type.Msg_StewReady:

                    DebugMessages.Instance.WriteLine(String.Format("Message handled by {0} at time {1}", MainSM.GetEntityName(pEntity.ID()), HighResTimer.Instance.RunningTime));

                    DebugMessages.Instance.WriteLine(String.Format("{0}: Okay Hun, ahm a comin'", MainSM.GetEntityName(pEntity.ID())));

                    pMiner.GetFSM().ChangeState(new EatStew());

                    return true;

            }//end switch

            return false; //send message to global message handler
        }

    }

    public class QuenchThirst : State
    {
        public QuenchThirst()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            if (pMiner.Location != location_type.saloon)
            {
                pMiner.Location = location_type.saloon;

                DebugMessages.Instance.WriteLine(String.Format("{0}: Boy, ah sure is thusty! Walking to the saloon", MainSM.GetEntityName(pEntity.ID()))); 
            }
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            pMiner.BuyAndDrinkAWhiskey();

            DebugMessages.Instance.WriteLine(String.Format("{0}: That's mighty fine sippin' liquer", MainSM.GetEntityName(pEntity.ID()))); 

            pMiner.GetFSM().ChangeState(new EnterMineAndDigForNugget());
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Leaving the saloon, feelin' good", MainSM.GetEntityName(pEntity.ID()))); 
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

    public class EatStew : State
    {
        public EatStew()
        { }

        public override void Enter(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Smells Reaaal goood Elsa!", MainSM.GetEntityName(pEntity.ID()))); 
        }

        public override void Execute(BaseGameEntity pEntity)
        {
            Miner pMiner = (Miner)pEntity;

            DebugMessages.Instance.WriteLine(String.Format("{0}: Tastes real good too!", MainSM.GetEntityName(pEntity.ID()))); 

            pMiner.GetFSM().RevertToPreviousState();
        }

        public override void Exit(BaseGameEntity pEntity)
        {
            DebugMessages.Instance.WriteLine(String.Format("{0}: Thankya li'lle lady. Ah better get back to whatever ah wuz doin'", MainSM.GetEntityName(pEntity.ID()))); 
        }

        public override bool OnMessage(BaseGameEntity pEntity, Telegram pTelegram)
        {
            return false;
        }

    }

}
