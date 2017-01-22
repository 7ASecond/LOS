using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using FastColoredTextBoxNS;
using LOS_Command.Commands;
using LOS_Command.Models;

namespace LOS_Command
{
    public partial class FrmMain : Form
    {
        private readonly TextStyle _commandStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Regular);
        private readonly TextStyle _responseStyle = new TextStyle(Brushes.White, null, FontStyle.Bold);

        private const string NoSuchFile = "No Such File Found ";

        private bool _useTeleType = false;

        private int _commandIndex = 0;
        private int _previousCommand = 0;

        private List<string> _commandHistory = new List<string>();
        private string _command = "";


        public FrmMain()
        {
            InitializeComponent();
            ConIO.Focus();
            ConIO.Select();
        }

        private void ConIO_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                // _previousCommand = _commandHistory.Count;
                JumpToEndOfLine();
                _command = ConIO.Lines[_commandIndex - 1];
                if (_command == string.Empty)
                {
                    ProcessEmptyCommand();
                }
                else
                    ProcessCommand(_command);
            }
            else if (e.KeyCode == Keys.Up)
            {
                SendKeys.Send("{DOWN}");
            }
        }

        private void JumpToEndOfLine()
        {
            ConIO.GoEnd();
        }

        private void ProcessEmptyCommand()
        {
            ConIO.InsertText("");
        }

        private void ProcessCommand(string command)
        {
            _commandHistory.Add(command);
            switch (command.ToLower().Split(' ')[0])
            {
                case "version":
                    string[] parts = command.ToLower().Split(' ');

                    if (parts.Length == 1)
                    {
                        VersionInfo[] versionList = GetVersion.Version();
                        DisplayVersion(versionList);
                    }
                    else if (parts.Length > 1)
                    {
                        VersionInfo[] versionList = GetVersion.Version(command.ToLower().Split()[1]);
                        DisplayFullVersion(versionList);
                    }

                    break;
                case "teletype":
                    _useTeleType = !_useTeleType; // Toggle Teletype mode
                    DisplayText("Teletype mode = " + _useTeleType.ToString());
                    break;
                default:
                    DisplayText(command.ToLower().Split(' ')[0].ToUpperInvariant() + " is not a recognized command");
                    break;
            }
        }

        private void DisplayText(string text)
        {
            if (_useTeleType)
            {
                foreach (char c in text)
                {
                    ConIO.InsertText(c.ToString(), _responseStyle);
                    ConIO.Refresh();
                }
            }
            else
            {
                ConIO.InsertText(text);
            }

            ConIO.InsertText(Environment.NewLine, _commandStyle);
        }

        private void DisplayVersion(VersionInfo[] versionList)
        {
            foreach (VersionInfo version in versionList)
            {
                DisplayText("File: " + version?.Filename);

            }
        }

        private void DisplayFullVersion(VersionInfo[] versionList)
        {
            foreach (VersionInfo version in versionList)
            {
                if (version?.VersionNumber == "N/A")
                    DisplayText(NoSuchFile + version?.Filename);
                else
                    DisplayText("File: " + version?.Filename + "\tVersion: " + version?.VersionNumber);

            }
        }

        private void ConIO_LineInserted(object sender, FastColoredTextBoxNS.LineInsertedEventArgs e)
        {
            _commandIndex = e.Index;

        }
    }
}
