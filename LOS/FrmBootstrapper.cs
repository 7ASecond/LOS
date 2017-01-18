using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LOS_DEFS;

namespace LOS
{
    public partial class FrmBootstrapper : Form
    {

        private readonly DEFS _fsDefs = new DEFS();

        enum LosApplications
        {
            Installer,
            Updater,
            Shell
        }


        public FrmBootstrapper()
        {
            InitializeComponent();

            //Give ourselves an easy way to exit the application when in DEBUG mode.
#if DEBUG
            btnDebugExit.Visible = true;
#endif
        }

        /// <summary>
        /// Shutsdown the computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShutdown_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false; // Stop the user from logging in now that the computer is going to shutdown
            var psi = new ProcessStartInfo("shutdown", "/s /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }


        /// <summary>
        /// Does the User have a valid local account?
        /// </summary>
        /// <returns>
        /// enum: UserStatus.UserDoesNotExist -> The user can not be found in the DEFS Account list
        /// enum: UserStatus.UserExists -> The User has been found and validated in the DEFS Account List
        /// enum: UserStatus.UserPanic -> The Panic Memorable Word has been used and User has been Delinked from the DEFS Filesystem - All Data is lost Permanently.
        /// </returns>
        private DEFS.UserStatus DoesUserExist(SecureString username, SecureString password, SecureString memorableWord)
        {
            return _fsDefs.FindUser(username, password, memorableWord);

        }

        /// <summary>
        /// Log the user in or create a new account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// 1) Check to see if the user exists on our External Database
        /// 2) Check to see if the User has a DEFS Partition
        /// 3) If not Registered Create Account and DEFS Partition - Resize DEFS
        /// 4) If Registered but wrong Password or wrong Memorable word - Do Action 3
        /// 5) If Registered and Panic Memorable Word is used - Destroy All Data.
        /// 6) If Registered and Security is good - Login and Mount DEFS - Resize DEFS
        /// </remarks>
        private void btnGo_Click_1(object sender, EventArgs e)
        {
            //TODO: Security Enhancements
            // Use SecureString TextBox
            // Throw the user details into secured memory.
            // Once User is logged in - zero out these values

            // Validate the User's details
            lblMessage.Text = string.Empty;

            if (tbMemorableWord.Text == string.Empty || tbMemorableWord.Text.Length < 4)
            {
                tbMemorableWord.Focus();
                lblMessage.Text = "Your Memorable Word must be at least 4 characters long";
                return;
            }

            SecureString ssMemorableWord = new SecureString();
            foreach (char c in tbMemorableWord.Text)
            {
                ssMemorableWord.AppendChar(c);
            }
            tbMemorableWord.Text = string.Empty;


            if (tbPassword.Text == string.Empty || tbPassword.Text.Length < 8)
            {
                tbPassword.Focus();
                lblMessage.Text = "Your Password must be at least 8 characters long";
                return;
            }

            SecureString ssPassword = new SecureString();
            foreach (char c in tbPassword.Text)
            {
                ssPassword.AppendChar(c);
            }
            tbPassword.Text = string.Empty;

            if (tbUsername.Text == string.Empty || tbUsername.Text.Length < 4)
            {
                tbUsername.Focus();
                lblMessage.Text = "Your Username must be at least 4 characters long";
                return;
            }

            SecureString ssUsername = new SecureString();
            foreach (char c in tbUsername.Text)
            {
                ssUsername.AppendChar(c);
            }
            tbUsername.Text = string.Empty;

            // Ok so we have passed the validation of the User's details

            // Check if the User Exists


            switch (DoesUserExist(ssUsername, ssPassword, ssMemorableWord))
            {
                case DEFS.UserStatus.UserDoesNotExist:
                    CreateUser(ssUsername, ssPassword, ssMemorableWord);
                    break;
                case DEFS.UserStatus.UserExists:
                    break;
                case DEFS.UserStatus.UserPanic:
                    DestroyData(ssUsername, ssPassword, ssMemorableWord);
                    break;
                default:
                    FatalCrash();
                    break;

            }
        }

        private void FatalCrash()
        {


        }

        private void DestroyData(SecureString ssUsername, SecureString ssPassword, SecureString ssMemorableWord)
        {


        }

        private void CreateUser(SecureString ssUsername, SecureString ssPassword, SecureString ssMemorableWord)
        {
            Run(LosApplications.Installer, ssUsername, ssPassword, ssMemorableWord);
            ssUsername.Dispose();
            ssPassword.Dispose();
            ssMemorableWord.Dispose();
        }

        private void Run(LosApplications app, SecureString ssUsername, SecureString ssPassword, SecureString ssMemorableWord)
        {
            string pathToApp = string.Empty;

            switch (app)
            {
                case LosApplications.Installer:
                    pathToApp = "C:\\LOS";
                    break;
                case LosApplications.Shell:
                    break;
                case LosApplications.Updater:
                    break;
                default:
                    FatalCrash();
                    break;
            }

            Process runProcess = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = "-U:" + ssUsername + ",-P:" + ssPassword + ",-M:" + ssMemorableWord,
                CreateNoWindow = false,
                FileName = "LOS-Installer.exe",
                LoadUserProfile = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                UseShellExecute = false,
                WorkingDirectory = pathToApp
            };

            // Don't know if this will work, It may need unsafe strings to be passed

            Hide();
            runProcess = System.Diagnostics.Process.Start(processStartInfo);
            if (runProcess != null)
            {
                runProcess.PriorityBoostEnabled = true;
                runProcess.WaitForExit();
            }
            Show();
        }

        private void btnDebugExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
