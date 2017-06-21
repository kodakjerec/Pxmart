namespace PxmartMonitor
{
    partial class FormMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader colStatus;
            System.Windows.Forms.ColumnHeader colName;
            System.Windows.Forms.ColumnHeader colIP;
            System.Windows.Forms.ColumnHeader colPort;
            System.Windows.Forms.ColumnHeader colMessage;
            System.Windows.Forms.ColumnHeader colarguments;
            System.Windows.Forms.ColumnHeader colPath;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.view_DevicesList = new System.Windows.Forms.ListView();
            this.img_StatusList = new System.Windows.Forms.ImageList(this.components);
            colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colarguments = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            colStatus.Width = 100;
            // 
            // colName
            // 
            colName.Text = "Program";
            colName.Width = 100;
            // 
            // colIP
            // 
            colIP.Text = "IP Address";
            colIP.Width = 100;
            // 
            // colPort
            // 
            colPort.Text = "Port";
            // 
            // colMessage
            // 
            colMessage.Text = "Message";
            colMessage.Width = 200;
            // 
            // colarguments
            // 
            colarguments.Text = "arguments";
            // 
            // colPath
            // 
            colPath.Text = "Path";
            // 
            // view_DevicesList
            // 
            this.view_DevicesList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.view_DevicesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            colStatus,
            colName,
            colIP,
            colPort,
            colMessage,
            colarguments,
            colPath});
            this.view_DevicesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_DevicesList.GridLines = true;
            this.view_DevicesList.Location = new System.Drawing.Point(0, 0);
            this.view_DevicesList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.view_DevicesList.Name = "view_DevicesList";
            this.view_DevicesList.Size = new System.Drawing.Size(659, 292);
            this.view_DevicesList.StateImageList = this.img_StatusList;
            this.view_DevicesList.TabIndex = 0;
            this.view_DevicesList.UseCompatibleStateImageBehavior = false;
            this.view_DevicesList.View = System.Windows.Forms.View.Details;
            this.view_DevicesList.ItemActivate += new System.EventHandler(this.view_DevicesList_ItemActivate);
            // 
            // img_StatusList
            // 
            this.img_StatusList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("img_StatusList.ImageStream")));
            this.img_StatusList.TransparentColor = System.Drawing.Color.Transparent;
            this.img_StatusList.Images.SetKeyName(0, "check_mark.png");
            this.img_StatusList.Images.SetKeyName(1, "delete.png");
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 292);
            this.Controls.Add(this.view_DevicesList);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.Text = "全聯-設備監控系統";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView view_DevicesList;
        private System.Windows.Forms.ImageList img_StatusList;
    }
}

