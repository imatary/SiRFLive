﻿namespace SiRFLive.Utilities
{
    using System;
    using System.Windows.Forms;

    public class ObjectInterface
    {
        public void FileBrowser(TextBox anything)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "File Locator";
            dialog.InitialDirectory = @"c:\";
            dialog.Filter = "All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.SetTextBoxText(anything, dialog.FileName);
            }
        }

        public void FileBrowser(TextBox anything, string initialDir)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "File Locator";
            dialog.InitialDirectory = initialDir;
            dialog.Filter = "All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.SetTextBoxText(anything, dialog.FileName);
            }
        }

        public void FileBrowserNewFile(TextBox anything)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "File Locator";
            dialog.InitialDirectory = @"c:\";
            dialog.Filter = "RF Capture files (*.PCM)|*.pcm";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.SetTextBoxText(anything, dialog.FileName);
            }
        }

        public string GetSelectedCheckListBoxIdx(CheckedListBox anything, int idx)
        {
            string rStr = string.Empty;
            anything.BeginInvoke((MethodInvoker)delegate {
                try
                {
                    rStr = (string) anything.Items[idx];
                }
                catch
                {
                }
            });
            return rStr;
        }

        public string GetSelectedListBoxIdx(ListBox anything, int idx)
        {
            string rStr = string.Empty;
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    rStr = (string) anything.Items[idx];
                }
                catch
                {
                }
            });
            return rStr;
        }

        public string GetTextBoxText(TextBox anything)
        {
            string myText = string.Empty;
			anything.Invoke((MethodInvoker)delegate
			{
                try
                {
                    myText = anything.Text.ToString();
                }
                catch
                {
                }
            });
            return myText;
        }

        public void SetButtonState(Button anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Enabled = state;
                }
                catch
                {
                }
            });
        }

        public void SetButtonText(Button anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Text = text;
                }
                catch
                {
                }
            });
        }

        public void SetCheckBoxChecked(CheckBox anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Checked = state;
                }
                catch
                {
                }
            });
        }

        public void SetCheckBoxState(CheckBox anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Enabled = state;
                }
                catch
                {
                }
            });
        }

        public void SetComboBoxSelectItem(ComboBox anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.SelectedItem = text;
                }
                catch
                {
                }
            });
        }

        public void SetComboBoxState(ComboBox anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Enabled = state;
                }
                catch
                {
                }
            });
        }

        public void SetComboBoxText(ComboBox anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Text = text;
                }
                catch
                {
                }
            });
        }

        public void SetLabelText(Label anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Text = text;
                }
                catch
                {
                }
            });
        }

        public void SetRadioBoxChecked(RadioButton anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Checked = state;
                }
                catch
                {
                }
            });
        }

        public void SetSelectedListBoxIdx(ListBox anything, int idx, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.SetSelected(idx, state);
                }
                catch
                {
                }
            });
        }

        public void SetTextBoxState(TextBox anything, bool state)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Enabled = state;
                }
                catch
                {
                }
            });
        }

        public void SetTextBoxText(TextBox anything, string text)
        {
			anything.BeginInvoke((MethodInvoker)delegate
			{
                try
                {
                    anything.Text = text;
                }
                catch
                {
                }
            });
        }
    }
}

