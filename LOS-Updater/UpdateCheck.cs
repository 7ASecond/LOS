// 2017, 1, 18
// LOS-Updater:UpdateCheck.cs
// Copyright (c) 2017 PopulationX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS_Updater
{
    //TODO: Needs to be made secure so that unauthorised updates to the system are not permitted. And that installation cannot be tracked back to a real user's computer.
    /// <summary>
    /// We have two types of installation file - The Latest Full Install and the Latest Cumulative Update since last Full Install
    /// These are just script files that explain which files go where and where they can be found on our servers
    /// If we do not have a Full(un)Install file on our system then we get the latest full install
    /// If we have a full(un)Install file but not the Latest Cumulative Update then get the latest Cumulative Update
    /// We differentiate between one file version and another by its file name.Full Installs start with F, Cumulative start with C.
    /// Currently these files are manually created - it will be necessary to automate the process before Public Release 1.
    ///
    /// Steps Get Latest Full and Cumulative release files
    ///       See if we have a Full Release Install File Locally.
    ///       If Not do a New Install (Full Release + Cumulatives)
    ///       If a Full release is local see if we have the latest cumulative
    ///       If Not do the latest cumulative release install
    ///       If Nothing needs updated exit
    ///       If Core files were updated Restart LOS
    ///       If Bootstrap was updated Reboot computer.
    /// </summary>
    public class UpdateCheck
    {

        public void GetLatestFullReleaseScript()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void GetLatestCumulativeReleaseScript()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void FindLatestLocalFullReleaseScript()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void FindLatestLocalCumulativeReleaseScript()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void DoFullInstall()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void DoCumulativeInstall()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void RollBackCumulative()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        public void RollBackFull()
        {
#if DEBUG
            DoDebugInstall();
#endif
        }

        private void DoDebugInstall()
        {
            
        }

    }
}
