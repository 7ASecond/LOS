// 2017, 1, 18
// LOS-Installer:frminstaller.cs
// Copyright (c) 2017 PopulationX
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
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

            Image spinnerImg = Image.FromFile(@"Theme\Current\spinner.gif");

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
            UpdateCheck updateCheck = new UpdateCheck();
            updateCheck.DoFullInstall(); // Run the installer (TODO: Convert this to a more controlled installation)
            pictureBox1.Image = Image.FromFile(@"theme\current\tick.png");

            // Create the LOS Super User on the Windows Host computer
            // Required to prevent or reduce the chances of the identification of the person running LOS
            pictureBox2.Visible = true;
            pictureBox2.Image = Image.FromFile(CreateLocalWindowsAccount() ? @"theme\current\tick.png" : @"theme\current\cross.png");

            // Impersonate the new LOSSystem User

            // Harden Windows Security

            // Create DEFS

            // Create LOS User for the EU installing LOS - No Profile Yet

            // Change Windows Desktop to LOS

            // Reboot Computer
        }

        /// <summary>
        /// method to create a new local Windows user account
        /// </summary>        
        public static bool CreateLocalWindowsAccount()
        {
            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Machine);
                UserPrincipal user = new UserPrincipal(context);
                user.SetPassword("L05Sy73M@cK0unt");
                user.DisplayName = "LOSSystem";
                user.Name = "LOSSystem";
                user.Description = "The LOS System Account";
                user.UserCannotChangePassword = false;
                user.PasswordNeverExpires = false;
                user.Save();
                
                //now add user to "Users" group so it displays in Control Panel
                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "Users");
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
