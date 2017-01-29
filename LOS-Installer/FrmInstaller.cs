// 2017, 1, 18
// LOS-Installer:frminstaller.cs
// Copyright (c) 2017 PopulationX

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JCS;
using LOS_Installer.Classes;
using LOS_Updater;


namespace LOS_Installer
{
    public partial class FrmInstaller : Form
    {
        public FrmInstaller()
        {
            InitializeComponent();
            Log("Initializing Installer");
            var spinnerImg = Image.FromFile(@"Theme\Current\spinner.gif");

            pictureBox1.Image = spinnerImg;
            pictureBox2.Image = spinnerImg;
            pictureBox3.Image = spinnerImg;
            pictureBox4.Image = spinnerImg;
            pictureBox5.Image = spinnerImg;
            pictureBox6.Image = spinnerImg;
            pictureBox7.Image = spinnerImg;
            pictureBox8.Image = spinnerImg;
        }

        private void FrmInstaller_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            Log("Ignoring Update Check");
            //Log("Checking for updates");
            //var updateCheck = new UpdateCheck();
            //updateCheck.DoFullInstall(); // Run the installer (TODO: Convert this to a more controlled installation)
            pictureBox1.Image = Image.FromFile(@"theme\current\tick.png");
            pictureBox1.Refresh();
            // Create the LOS Super User on the Windows Host computer
            // Required to prevent or reduce the chances of the identification of the person running LOS
            pictureBox2.Visible = true;
            pictureBox2.Refresh();
            // Change: Check to see if LOSSystem Account Exists
            Log("Does LOSSystem Account Exist?");
            var accountGood = LOSSystemAccountExists();
            ManageLosSystemAccount(accountGood);
            // Change: Check to see if the LOSSystem Account is Administrator
            // Change: Check to see if this application is running as LOSSystem
            pictureBox2.Image = Image.FromFile(accountGood ? @"theme\current\tick.png" : @"theme\current\cross.png");

            // Impersonate the new LOSSystem User
            pictureBox4.Visible = true;
            pictureBox4.Refresh();
            if (Environment.UserName != "LOSSystem")
            {
                using (new Impersonator("LOSSystem", Environment.MachineName, "L05Sy73M@cK0unt"))
                {                                        
                    HardenWindowsSecurity();                   
                }
                pictureBox4.Image = Image.FromFile(@"theme\current\tick.png");
                pictureBox4.Refresh();
            }
            else
            {
                HardenWindowsSecurity();
                pictureBox4.Image = Image.FromFile(@"theme\current\tick.png");
                pictureBox4.Refresh();
            }

            // Harden Windows Security

            // Create DEFS

            // Create LOS User for the EU installing LOS - No Profile Yet

            // Change Windows Desktop to LOS

            // Reboot Computer

            Log("Installation Complete");
        }

        private void HardenWindowsSecurity()
        {
            Log("Hardening Windows Security");
            Log("Getting Windoes Version and type");
           Log(OSVersionInfo.Name);
            Log("Edition = "+ OSVersionInfo.Edition);
            if (OSVersionInfo.ServicePack != string.Empty)
                Log("Service Pack = "+ OSVersionInfo.ServicePack);
            else
                Log("Service Pack = None");
            Log("Version = "+ OSVersionInfo.VersionString);
            Log("ProcessorBits = "+ OSVersionInfo.ProcessorBits);
            Log("OSBits = "+ OSVersionInfo.OSBits);
            Log("ProgramBits = "+ OSVersionInfo.ProgramBits);
            Log("Killing Unwanted Services");
            KillServices();
            Log("Killing Unwanted Logs");
            KillLogs();
            Log("Killing Unwanted Processes");
            KillProcesses();
            Log("Killing Unwanted Caches");
            Killcaches();
            
        }

        private void Killcaches()
        {
            

        }

        private void KillProcesses()
        {
            
        }

        private void KillLogs()
        {
            
        }

        private void KillServices()
        {
            

        }

        private void ManageLosSystemAccount(bool accountGood)
        {

            
            if (accountGood)
            {
                Log("Is LOSSystem Account Administrator?");
                var administratorGood = LOSSystemIsAdministrator();
                if (administratorGood)
                {
                    pictureBox3.Visible = true;
                    pictureBox3.Refresh();
                    if (Environment.UserName == "LOSSystem")
                    {
                        // AutoLogon LOSSystem
                        Log("Autologon LOSSystem Account");
                        var autologonGood = AutoLogon();

                    }
                    else
                    {
                        Log("Impersonate LOSSystem Account");
                        // We need to impersonate the LOSSystem User before continuing or reboot into LOSSystem
                        using (new Impersonator("LOSSystem", Environment.MachineName, "L05Sy73M@cK0unt"))
                        {
                            Log("Autologon LOSSystem Account");
                            var autologonGood = AutoLogon();
                        }
                    }
                }
                else
                {
                    // Make LOSSystem User Account an Administrator.
                    Log("Make LOSSystem Account Administrator");
                    administratorGood = MakeLOSSystemAdministrator();

                }
            }
            else
            {
                Log("Does LOSSystem Account Does Not Exist?");
                Log("Create LOSSystem Account");
                CreateLocalWindowsAccount();
                Log("Is LOSSystem Account Administrator?");
                var administratorGood = LOSSystemIsAdministrator();
                if (administratorGood)
                {
                    if (Environment.UserName == "LOSSystem")
                    {
                        // AutoLogon LOSSystem
                        Log("Autologon LOSSystem Account");
                        var autologonGood = AutoLogon();
                    }
                    else
                    {
                        Log("Impersonate LOSSystem Account");
                        // We need to impersonate the LOSSystem User before continuing or reboot into LOSSystem
                        using (new Impersonator("LOSSystem", Environment.MachineName, "L05Sy73M@cK0unt"))
                        {
                            Log("Autologon LOSSystem Account");
                            var autologonGood = AutoLogon();
                        }
                    }
                }
                else
                {
                    // Make LOSSystem User Account an Administrator.
                    Log("Make LOSSystem Account Administrator");
                    administratorGood = MakeLOSSystemAdministrator();
                }
            }
            pictureBox3.Image = Image.FromFile(@"theme\current\tick.png");
            pictureBox3.Refresh();
        }

        private bool AutoLogon()
        {
            try
            {
                Process.Start(Path.Combine("C:\\LOS\\binaries\\AutoLogon\\", "AutoLogon.exe LOSSystem " + Environment.MachineName + " L05Sy73M@cK0unt"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Attempts to make LOSSystem Account and Administrator Account
        /// </summary>
        /// <returns>
        /// bool: True if successful
        /// bool: False if failed
        /// </returns>
        private bool MakeLOSSystemAdministrator()
        {
            var systemContext = new PrincipalContext(ContextType.Machine, null);
            var userPrincipal = UserPrincipal.FindByIdentity(systemContext, "LOSSystem");
            try
            {
                var groupPrincipal = GroupPrincipal.FindByIdentity(systemContext, "Administrators");

                if (groupPrincipal != null)
                {
                    //check if user is a member

                    if (groupPrincipal.Members.Contains(systemContext, IdentityType.SamAccountName, "LOSSystem"))
                    {
                        return true;
                    }
                    //Adding the user to the group

                    if (userPrincipal != null) groupPrincipal.Members.Add(userPrincipal);
                    groupPrincipal.Save();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                //TODO: LOG THIS

            }

            return false;
        }

        /// <summary>
        /// Detects if the LOSSystem User Account is Administrator
        /// </summary>
        /// <returns>
        /// bool: True the LOSSystem Account Is an Administrator Account
        /// bool: False the LOSSystem Account Is NOT an administrator account.
        /// </returns>
        private bool LOSSystemIsAdministrator()
        {
            var ret = false;

            try
            {
                var localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
                var userGroup = localMachine.Children.Find("Administrators", "group");

                var members = userGroup.Invoke("members", null);
                foreach (var groupMember in (IEnumerable)members)
                {
                    var member = new DirectoryEntry(groupMember);
                    if (member.Name.Equals("LOSSystem", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;


        }

        /// <summary>
        /// Does the LOSSystem Account Exist?
        /// </summary>
        /// <returns>
        /// bool: True the account does exists
        /// bool: False the account does not exist.
        /// </returns>
        private bool LOSSystemAccountExists()
        {
            var systemContext = new PrincipalContext(ContextType.Machine, null);
            UserPrincipal usr = UserPrincipal.FindByIdentity(systemContext, "LOSSystem");
            if (usr != null) return true;
            return false;

        }

        /// <summary>
        /// method to create a new local Windows user account
        /// </summary>        
        public static bool CreateLocalWindowsAccount()
        {
            try
            {
                var context = new PrincipalContext(ContextType.Machine);
                var user = new UserPrincipal(context);
                user.SetPassword("L05Sy73M@cK0unt");
                user.DisplayName = "LOSSystem";
                user.Name = "LOSSystem";
                user.Description = "The LOS System Account";
                user.UserCannotChangePassword = false;
                user.PasswordNeverExpires = false;
                user.Save();

                //now add user to "Users" group so it displays in Control Panel
                var group = GroupPrincipal.FindByIdentity(context, "Users");
                group?.Members.Add(user);
                group?.Save();


                //now add user to "Administrators" group so it displays in Control Panel
                group = GroupPrincipal.FindByIdentity(context, "Administrators");
                group?.Members.Add(user);
                group?.Save();
                //TODO: Will probably need to add user to other groups as well to have more control over the underlying system


                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating account: {0}", ex.Message);
                return false;
            }

        }

        private void Log(string msg)
        {
            fctb.InsertText(msg + Environment.NewLine);
            fctb.Focus();
            fctb.Refresh();
        }
    }
}
