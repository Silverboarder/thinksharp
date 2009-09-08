using System;

namespace ThinkSharp.StateMachines
{
    public class Telegram
    {
        //the entity that sent this telegram
        private int m_Sender;

        //the entity that is to receive this telegram
        private int m_Receiver;

        //the message itself.
        private int m_Msg;

        //messages can be dispatched immediately or delayed for a specified amount
        //of time. If a delay is necessary this field is stamped with the time 
        //the message should be dispatched.
        private double m_DispatchTime;

        //any additional information that may accompany the message
        private object m_ExtraInfo;

        //Note how the Dispatch Times must be smaller than
        //SmallestDelay apart before two Telegrams are considered unique.
        private const double SmallestDelay = 0.25;

        public int Sender
        {
            get { return m_Sender; }
            set { m_Sender = value; }
        }

        public int Receiver
        {
            get { return m_Receiver; }
            set { m_Receiver = value; }
        }

        public int Msg
        {
            get { return m_Msg; }
            set { m_Msg = value; }
        }

        public double DispatchTime
        {
            get { return m_DispatchTime; }
            set { m_DispatchTime = value; }
        }

        public object ExtraInfo
        {
            get { return m_ExtraInfo; }
            set { m_ExtraInfo = value; }
        }

        public Telegram()
        {
            m_DispatchTime = -1;
            m_Sender = -1;
            m_Receiver = -1;
            m_Msg = -1;
        }

        public Telegram(double time, int sender, int receiver, int msg, object info)
        {
            m_DispatchTime = time;
            m_Sender = sender;
            m_Receiver = receiver;
            m_Msg = msg;
            m_ExtraInfo = info;
        }

        public override bool Equals(object obj)
        {
            if (obj is Telegram && obj != null)
            {
                Telegram objTelegram = (Telegram)obj;

                return (Math.Abs(m_DispatchTime - objTelegram.DispatchTime) < SmallestDelay) &&
                        (m_Sender == objTelegram.Sender) &&
                        (m_Receiver == objTelegram.Receiver) &&
                        (m_Msg == objTelegram.Msg);

            }
            return false;
        }

    }
}
