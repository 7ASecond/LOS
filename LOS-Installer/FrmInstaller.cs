// 2017, 1, 18
// LOS-Installer:frminstaller.cs
// Copyright (c) 2017 PopulationX
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LOS_Updater;


namespace LOS_Installer
{
    public partial class FrmInstaller : Form
    {
        public FrmInstaller()
        {
            InitializeComponent();

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
            var updateCheck = new UpdateCheck();
            updateCheck.DoFullInstall(); // Run the installer (TODO: Convert this to a more controlled installation)
            pictureBox1.Image = Image.FromFile(@"theme\current\tick.png");

            // Create the LOS Super User on the Windows Host computer
            // Required to prevent or reduce the chances of the identification of the person running LOS
            pictureBox2.Visible = true;

            // Change: Check to see if LOSSystem Account Exists
            var accountGood = LOSSystemAccountExists();
            if (accountGood)
            {
                var administratorGood = LOSSystemIsAdministrator();
            }
            // Change: Check to see if the LOSSystem Account is Administrator
            // Change: Check to see if this application is running as LOSSystem
            pictureBox2.Image = Image.FromFile(accountGood ? @"theme\current\tick.png" : @"theme\current\cross.png");

            // Impersonate the new LOSSystem User

            // Harden Windows Security

            // Create DEFS

            // Create LOS User for the EU installing LOS - No Profile Yet

            // Change Windows Desktop to LOS

            // Reboot Computer
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
                var userGroup = localMachine.Children.Find("Administrator", "group");

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
            var bRet = false;

            try
            {
                var acct = new NTAccount("LOSSystem");
                var id = (SecurityIdentifier)acct.Translate(typeof(SecurityIdentifier));

                bRet = id.IsAccountSid();
            }
            catch (IdentityNotMappedException)
            {
                /* Invalid user account */
            }

            return bRet;
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
    }
}
