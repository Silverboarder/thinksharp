using System;
using ThinkSharp.Common;
using ThinkSharp.StateMachines;

namespace TestApp
{

    public class Miner : BaseGameEntity
    {
        //the amount of gold a miner must have before he feels he can go home
        public const int ComfortLevel = 5;

        //the amount of nuggets a miner can carry
        public const int MaxNuggets = 3;

        //above this value a miner is thirsty
        public const int ThirstLevel = 4;

        //above this value a miner is sleepy
        public const int TirednessThreshold = 3;

        private location_type m_Location;

        //an instance of the state machine class
        private StateMachine m_pStateMachine;

        //how many nuggets the miner has in his pockets
        private int m_iGoldCarried;

        private int m_iMoneyInBank;

        //the higher the value, the thirstier the miner
        private int m_iThirst;

        //the higher the value, the more tired the miner
        private int m_iFatigue;

        public const int HPFull = 4;
        private int m_iHP;

        public Miner(int entity_type)
            : base(entity_type, (int)EntityName.ent_Miner_Bob)
        {
            m_Location = location_type.shack;

            m_iGoldCarried = 0;
            m_iMoneyInBank = 0;
            m_iThirst = 0;
            m_iFatigue = 0;
            m_iHP = HPFull;

            //set up the state machine
            m_pStateMachine = new StateMachine(this);

            m_pStateMachine.CurrentState = new GoHomeAndSleepTilRested();

            m_pStateMachine.GlobalState = new BobsGlobalState();
        }

        public StateMachine GetFSM() { return m_pStateMachine; }

        public location_type Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        public int HP
        {
            get { return m_iHP; }
            set { m_iHP = value; }
        }

        public int GoldCarried
        {
            get { return m_iGoldCarried; }
            set { m_iGoldCarried = value; }
        }

        public void AddToGoldCarried(int val)
        {
            m_iGoldCarried += val;
            if (m_iGoldCarried < 0) m_iGoldCarried = 0;
        }

        public bool PocketsFull()
        {
            return m_iGoldCarried >= MaxNuggets;
        }

        public bool Fatigued()
        {
            return m_iFatigue > TirednessThreshold;
        }

        public void DecreaseFatigue()
        {
            m_iFatigue -= 2;
        }

        public void IncreaseFatigue()
        {
            m_iFatigue += 1;
        }

        public int Wealth
        {
            get { return m_iMoneyInBank; }
            set { m_iMoneyInBank = value; }
        }

        public void AddToWealth(int val)
        {
            m_iMoneyInBank += val;
            if (m_iMoneyInBank < 0) m_iMoneyInBank = 0;
        }

        public bool Thirsty()
        {
            if (m_iThirst >= ThirstLevel) { return true; }
            return false;
        }

        public void BuyAndDrinkAWhiskey() 
        { 
            m_iThirst = 0; 
            m_iMoneyInBank -= 2; 
        }

        public override bool HandleMessage(object msg)
        {
            return m_pStateMachine.HandleMessage((Telegram)msg);
        }

        public void Update()
        {
            m_iThirst += 1;
            m_pStateMachine.Update();
        }

    }
}
