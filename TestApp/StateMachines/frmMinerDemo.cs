using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

using ThinkSharp.Common;
using ThinkSharp.StateMachines;

namespace TestApp
{
    public partial class frmMinerDemo : Form
    {
        private BackgroundWorker m_AsyncWorker;
        private Miner Bob;
        private MinersWife Elsa;
        private BarFlyJoe Joe;

        public frmMinerDemo()
        {
            InitializeComponent();

            // m_AsyncWorker will be used to perform the AI work on a background thread.
            m_AsyncWorker = new BackgroundWorker();
            m_AsyncWorker.WorkerReportsProgress = true;
            m_AsyncWorker.ProgressChanged += new ProgressChangedEventHandler(Thread_ProgressChanged);
            m_AsyncWorker.DoWork += new DoWorkEventHandler(Thread_DoWork);
            m_AsyncWorker.WorkerSupportsCancellation = true;

            // Just direct all debug output to our Message Writer
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new TextWriterTraceListener(DebugMessages.Instance));

            //create a miner
            Bob = new Miner((int)EntityName.ent_Miner_Bob);

            //create his wife
            Elsa = new MinersWife((int)EntityName.ent_Elsa);

            Joe = new BarFlyJoe((int)EntityName.ent_BarFly);

            //register them with the Message Dispatcher
            MessageDispatcher.Instance.RegisterEntity(Bob);
            MessageDispatcher.Instance.RegisterEntity(Elsa);
            MessageDispatcher.Instance.RegisterEntity(Joe);
        }

        private void frmMinerDemo_Load(object sender, EventArgs e)
        {
            m_AsyncWorker.RunWorkerAsync();
        }

        private void Thread_DoWork(object sender, DoWorkEventArgs e)
        {
            // The Sender is the BackgroundWorker object we need it to
            // report progress and check for cancellation.
            BackgroundWorker bwAsync = (BackgroundWorker)sender;

            HighResTimer.Instance.Start();

            try
            {
                //run Characters through a few Update calls
                for (int i = 0; i < 40; ++i)
                {
                    if (bwAsync.CancellationPending)
                        break;

                    HighResTimer.Instance.Update();

                    Bob.Update();
                    Elsa.Update();
                    Joe.Update();

                    //dispatch any delayed messages
                    MessageDispatcher.Instance.DispatchDelayedMessages();

                    bwAsync.ReportProgress((int)((double)(i / 50) * 100));

                    System.Threading.Thread.Sleep(800);
                }
            }
            catch (Exception exc)
            {
                DebugMessages.Instance.WriteLine(String.Format("Exception Caught: {0}{1}", Environment.NewLine, exc.ToString()));
            }

            bwAsync.ReportProgress(100);
        }

        private void Thread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ArrayList arrReturnValue = DebugMessages.Instance.FlushMessages();

            foreach (string strLine in arrReturnValue)
            {
                if (strLine.StartsWith(MainSM.GetEntityName((int)EntityName.ent_Miner_Bob)))
                {
                    txtUpdates.SelectionColor = Color.Blue;
                }
                else if (strLine.StartsWith(MainSM.GetEntityName((int)EntityName.ent_Elsa)))
                {
                    txtUpdates.SelectionColor = Color.DarkViolet;
                }
                else if (strLine.StartsWith(MainSM.GetEntityName((int)EntityName.ent_BarFly)))
                {
                    txtUpdates.SelectionColor = Color.DarkRed;
                }
                else
                {
                    txtUpdates.SelectionColor = Color.Black;
                }

                txtUpdates.AppendText(strLine + Environment.NewLine);
            }

            if (e.ProgressPercentage == 100)
            {
                txtUpdates.SelectionColor = Color.Black;
                txtUpdates.AppendText(Environment.NewLine + "Demo Complete!" + Environment.NewLine);
            }
            else
            {
                lblLocationBob.Text = MainSM.GetLocation((int)Bob.Location);
                lblStatusBob.Text = Bob.GetFSM().CurrentState.GetType().Name.ToString();

                lblLocationWife.Text = MainSM.GetLocation((int)Elsa.Location);
                lblStatusWife.Text = Elsa.GetFSM().CurrentState.GetType().Name.ToString();

                lblLocationBarFly.Text = MainSM.GetLocation((int)Joe.Location);
                lblStatusBarFly.Text = Joe.GetFSM().CurrentState.GetType().Name.ToString();
            }
        }

        private void frmMinerDemo_Closing(Object sender, FormClosingEventArgs e)
        {
            if (m_AsyncWorker.IsBusy)
            {
                m_AsyncWorker.CancelAsync();
                e.Cancel = true;
            }
        }
    }
}
