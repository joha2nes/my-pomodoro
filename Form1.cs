using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Pomodoro
{
	public partial class Form1 : Form
	{
		enum State { Break, Work }

		const string RegistryName = "Johannes Pomodoro";

		State currentState;
		State CurrentState
		{
			get { return currentState; }
			set
			{
				if (value != currentState)
					_CurrentState = value;
			}
		}
		State _CurrentState
		{
			get { return currentState; }
			set
			{
				currentState = value;
				label.Text = currentState.ToString();
				notifyIcon.BalloonTipText = currentState.ToString();
				notifyIcon.ShowBalloonTip(10_000);
				
				// test
				this.WindowState = FormWindowState.Minimized;
				this.Show();
				this.WindowState = FormWindowState.Normal;
			}
		}

		Timer timer = new Timer();

		public Form1()
		{
			Func<State> checkState = () =>
				//(DateTime.Now.Minute >= 55 && DateTime.Now.Minute <= 59) ||
				//(DateTime.Now.Minute >= 25 && DateTime.Now.Minute <= 29) ?
				(DateTime.Now.Minute >= 50 && DateTime.Now.Minute <= 59) ?
					State.Break :
					State.Work;

			InitializeComponent();
			timer.Interval = 1000;
			timer.Tick += (obj, args) => CurrentState = checkState();
			timer.Start();
			label.Text = CurrentState.ToString();

			_CurrentState = checkState();

			using (var registry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
			{
				autostartToolStripMenuItem.Checked = registry.GetValue(RegistryName) != null;
			}
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
				Hide();
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void AutostartToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			using (var registry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
			{
				if (autostartToolStripMenuItem.Checked)
					registry.SetValue(RegistryName, Application.ExecutablePath.ToString());
				else
					registry.DeleteValue(RegistryName, false);
			}
		}
	}
}
