using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace BankManager
{
    public partial class sp_BaoCaoDSKHcs : DevExpress.XtraReports.UI.XtraReport
    {
        public sp_BaoCaoDSKHcs(Boolean all)
        {
            InitializeComponent();
            this.sqlDataSource1.Connection.ConnectionString = Program.connstr;

            this.sqlDataSource1.Queries[0].Parameters[0].Value = all;
        }

    }
}
