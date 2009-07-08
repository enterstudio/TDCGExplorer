﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public partial class EditSystemDatabase : Form
    {
        public EditSystemDatabase()
        {
            InitializeComponent();
        }

        public string textArcPath
        {
            get { return tbArcsDirectory.Text; }
            set { tbArcsDirectory.Text = value; }
        }

        public string textZipPath
        {
            get { return tbZipDirectory.Text; }
            set { tbZipDirectory.Text = value; }
        }

        public string textModDbUrl
        {
            get { return tbModRefServer.Text; }
            set { tbModRefServer.Text = value; }
        }

        public string textZipRegexp
        {
            get { return tbZipRegexp.Text; }
            set { tbZipRegexp.Text = value; }
        }

        public string textArcnamesServer
        {
            get { return tbArcnamesServer.Text; }
            set { tbArcnamesServer.Text = value; }
        }

        public string textWorkPath
        {
            get { return tbWorkPath.Text; }
            set { tbWorkPath.Text = value; }
        }

        public bool lookupmodref
        {
            get { return checkBoxLookupModRef.Checked; }
            set { checkBoxLookupModRef.Checked = value; }
        }

        public string textModRegexp
        {
            get { return textBoxModRegexp.Text; }
            set { textBoxModRegexp.Text = value; }
        }

        public string textTagnamesServer
        {
            get { return textBoxTagNameServer.Text; }
            set { textBoxTagNameServer.Text = value; }
        }
    }
}