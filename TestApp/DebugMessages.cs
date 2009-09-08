using System;
using System.Text;
using System.Collections;

namespace TestApp
{
    public class DebugMessages : System.IO.TextWriter
    {
        #region " Singleton class implementation "

        private static readonly DebugMessages instance = new DebugMessages();

        public static DebugMessages Instance
        {
            get { return instance; }
        }

        #endregion

        private ArrayList arrMessages;

        private DebugMessages()
        {
            arrMessages = new ArrayList();
        }

        public override void Write(char ch)
        {
            Write(ch.ToString());
        }

        public override void Write(string s)
        {
            arrMessages.Add(s);
        }

        public override void WriteLine(string s)
        {
            arrMessages.Add(s);
        }

        public ArrayList FlushMessages()
        {
            ArrayList arrReturnValue = new ArrayList(arrMessages);

            arrMessages.Clear();

            return arrReturnValue;
        }

        public override System.Text.Encoding Encoding
        {
            get { return Encoding.Default; }
        }

    }
}
