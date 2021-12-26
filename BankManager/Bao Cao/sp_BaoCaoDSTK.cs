using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace BankManager
{
    public partial class ReportDSTK : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportDSTK(DateTime from, DateTime to, Boolean all)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = from;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = to;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = all;
        }

    }
}
