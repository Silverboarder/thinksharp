using System;
using System.Collections.Generic;
using System.Diagnostics;
using ThinkSharp.Common;

namespace ThinkSharp.StateMachines
{
    public sealed class MessageDispatcher
    {
        #region " Singleton class implementation "

        private static readonly MessageDispatcher instance = new MessageDispatcher();

        public static MessageDispatcher Instance
        {
            get { return instance; }
        }

        #endregion

        public const double SEND_MSG_IMMEDIATELY = 0.0f;
        public const int NO_ADDITIONAL_INFO = 0;

        // List of messages, which are sorted by their dispatch time.
        private List<Telegram> m_PriorityQ;

        // All registered BaseGameEntity's that may want to deal with messages
        private Dictionary<int, BaseGameEntity> m_dictEntities;

        double m_CurrentRunningTime;

        private MessageDispatcher()
        {
            m_PriorityQ = new List<Telegram>();
            m_dictEntities = new Dictionary<int, BaseGameEntity>();
        }

        public BaseGameEntity GetRegisteredEntityFromID(int id)
        {
            try
            {
                return m_dictEntities[id];
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine(String.Format("Key of id {0} not registered!", id));
            }

            return null;
        }

        public void RemoveRegisteredEntity(int id)
        {
            m_dictEntities.Remove(id);
        }

        public void RegisterEntity(BaseGameEntity NewEntity)
        {
            m_dictEntities[NewEntity.ID()] = NewEntity;
        }

        public void UpdateRunningTime(double newTime)
        {
            m_CurrentRunningTime = newTime;
        }

        public double GetRunningTime()
        {
            return m_CurrentRunningTime;
        }

        //this method is utilized by DispatchMessage or DispatchDelayedMessages.
        //This method calls the message handling member function of the receiving
        //entity, pReceiver, with the newly created telegram
        public void Discharge(BaseGameEntity pReceiver, Telegram telegram)
        {
            if (!pReceiver.HandleMessage(telegram))
            {
                Debug.WriteLine("Telegram could not be handled");
            }
        }

        //---------------------------- DispatchMessage ---------------------------
        //
        //  given a message, a receiver, a sender and any time delay , this function
        //  routes the message to the correct agent (if no delay) or stores
        //  in the message queue to be dispatched at the correct time
        //------------------------------------------------------------------------
        public void DispatchMessage(double delay, int sender, int receiver, int msg, object ExtraInfo)
        {
            //get pointers to the sender and receiver
            BaseGameEntity pSender = GetRegisteredEntityFromID(sender);
            BaseGameEntity pReceiver = GetRegisteredEntityFromID(receiver);

            //make sure the receiver is valid
            if (pReceiver == null)
            {
                Debug.WriteLine(String.Format("Warning! No Receiver with ID of {0} found", receiver.ToString()));
                return;
            }

            //create the telegram
            Telegram telegram = new Telegram(0, sender, receiver, msg, ExtraInfo);

            //if there is no delay, route telegram immediately
            if (delay <= 0.0f)
            {
                Debug.WriteLine(String.Format("Instant telegram dispatched at time: {0} by {1} for {2}. Msg is {3}", m_CurrentRunningTime, pSender.ID().ToString(), pReceiver.ID().ToString(), msg));

                //send the telegram to the recipient
                Discharge(pReceiver, telegram);
            }
            //else calculate the time when the telegram should be dispatched
            else
            {
                telegram.DispatchTime = m_CurrentRunningTime + delay;

                if (!m_PriorityQ.Contains(telegram))
                {
                    // put it in the queue
                    m_PriorityQ.Add(telegram);

                    // and sort the queue
                    m_PriorityQ.Sort(CompareByDispatchTime);

                    Debug.WriteLine(String.Format("Delayed telegram from {0} stored at time {1} for {2}. Msg is {3}", pSender.ID(), m_CurrentRunningTime.ToString(), pReceiver.ID(), msg));
                }
            }
        }

        private static int CompareByDispatchTime(Telegram x, Telegram y)
        {
            if (x == y)
            {
                return 0;
            }
            else if (x.DispatchTime > y.DispatchTime)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        //---------------------- DispatchDelayedMessages -------------------------
        //
        //  This function dispatches any telegrams with a timestamp that has
        //  expired. Any dispatched telegrams are removed from the queue
        //------------------------------------------------------------------------
        public void DispatchDelayedMessages()
        {
            //now peek at the queue to see if any telegrams need dispatching.
            //remove all telegrams from the front of the queue that have gone
            //past their sell by date
            while (!(m_PriorityQ.Count == 0) && (m_PriorityQ[0].DispatchTime < m_CurrentRunningTime) && (m_PriorityQ[0].DispatchTime > 0))
            {
                //read the telegram from the front of the queue
                Telegram telegram = m_PriorityQ[0];

                //find the recipient
                BaseGameEntity pReceiver = GetRegisteredEntityFromID(telegram.Receiver);

                Debug.WriteLine(String.Format("Queued telegram ready for dispatch: Sent to {0}. Msg is {1}", pReceiver.ID().ToString(), telegram.Msg.ToString()));

                //send the telegram to the recipient
                Discharge(pReceiver, telegram);

                //remove it from the queue
                m_PriorityQ.RemoveAt(0);
            }
        }

    }
}
