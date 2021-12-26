using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace BankManager.Bao_Cao
{
    public partial class RpAccounts : DevExpress.XtraReports.UI.XtraReport
    {
        public RpAccounts(DateTime dateFrom, DateTime dateTo, Boolean all)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;
            this.sqlDataSource1.Queries[0].Parameters[0].Value = dateFrom;
            this.sqlDataSource1.Queries[0].Parameters[1].Value = dateTo;
            this.sqlDataSource1.Queries[0].Parameters[2].Value = all;
            this.sqlDataSource1.Fill();
        }

    }
}
