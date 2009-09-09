using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestApp
{
    public enum location_type
    {
        shack,
        goldmine,
        bank,
        saloon
    };

    public enum message_type
    {
        Msg_HiHoneyImHome,
        Msg_StewReady,
        Msg_Antagonize,
        Msg_DeclineFight,
        Msg_AcceptFight,
        Msg_IncomingPunch
    };

    public enum EntityName
    {
        ent_Miner_Bob,
        ent_Elsa,
        ent_BarFly
    };

    public static class MainSM
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMinerDemo());
        }

        public static string GetEntityName(int n)
        {
            switch (n)
            {
                case (int)EntityName.ent_Miner_Bob:

                    return "Miner Bob";

                case (int)EntityName.ent_Elsa:

                    return "Elsa";

                case (int)EntityName.ent_BarFly:

                    return "BarFly Joe";

                default:

                    return "UNKNOWN!";
            }
        }

        public static string GetLocation(int n)
        {
            return Enum.GetName(typeof(location_type), n);
        }

        public static string GetMessageType(int n)
        {
            return Enum.GetName(typeof(message_type), n);
        }
    }
}