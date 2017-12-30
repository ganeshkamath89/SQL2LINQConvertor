using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL2LINQ {
    public partial class GetDBInformation : Form {
        public GetDBInformation() {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            Globals.Server = txtServer.Text;
            Globals.Database = txtDatabase.Text;
            Globals.UserName = txtUserName.Text;
            Globals.Password = txtPassword.Text;
            if (!string.IsNullOrEmpty(txtUserDefinedObjectName.Text)) {
                Globals.UserDefinedObjectName = txtUserDefinedObjectName.Text;
            }
            Close();
        }

        internal void LoadText() {
            txtServer.Text = Globals.Server;
            txtDatabase.Text = Globals.Database;
            txtUserName.Text = Globals.UserName;
            txtPassword.Text = Globals.Password;
            txtUserDefinedObjectName.Text = Globals.UserDefinedObjectName;

        }

        private void btnTestConnection_Click(object sender, EventArgs e) {
            var connectionString = string.Format(@"Server={0};Database={1};User id={2};Password={3}",
                 Globals.Server, Globals.Database, Globals.UserName, Globals.Password);

            using (var connection = new SqlConnection(connectionString)) {
                try {
                    connection.Open();
                } catch {
                    MessageBox.Show("Unsuccessful Test Connection", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                } finally {
                    connection.Close();
                }
            }

            MessageBox.Show("Successful Test Connection","Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
