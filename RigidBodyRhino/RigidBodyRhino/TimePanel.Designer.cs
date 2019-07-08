namespace RigidBodyRhino
{
    partial class TimePanel
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimePanel));
            this.Play = new System.Windows.Forms.Button();
            this.Pause = new System.Windows.Forms.Button();
            this.Restart = new System.Windows.Forms.Button();
            this.PropertiesGroup = new System.Windows.Forms.GroupBox();
            this.IsStaticCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FrameTrackBar = new System.Windows.Forms.TrackBar();
            this.FrameProgressBar = new System.Windows.Forms.ProgressBar();
            this.TimeBarBox = new System.Windows.Forms.GroupBox();
            this.ActualFrame = new System.Windows.Forms.Label();
            this.MaxFrameBox = new System.Windows.Forms.NumericUpDown();
            this.CommandsBox = new System.Windows.Forms.GroupBox();
            this.JCompoundButton = new System.Windows.Forms.Button();
            this.JCylinderButton = new System.Windows.Forms.Button();
            this.JShpereButton = new System.Windows.Forms.Button();
            this.JBoxButton = new System.Windows.Forms.Button();
            this.RigidBodyButton = new System.Windows.Forms.Button();
            this.PropertiesGroup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameTrackBar)).BeginInit();
            this.TimeBarBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxFrameBox)).BeginInit();
            this.CommandsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Play
            // 
            this.Play.Image = ((System.Drawing.Image)(resources.GetObject("Play.Image")));
            this.Play.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Play.Location = new System.Drawing.Point(3, 3);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(100, 50);
            this.Play.TabIndex = 0;
            this.Play.Text = "Play";
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // Pause
            // 
            this.Pause.Image = ((System.Drawing.Image)(resources.GetObject("Pause.Image")));
            this.Pause.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Pause.Location = new System.Drawing.Point(109, 3);
            this.Pause.Name = "Pause";
            this.Pause.Size = new System.Drawing.Size(100, 50);
            this.Pause.TabIndex = 1;
            this.Pause.Text = "Pause";
            this.Pause.UseVisualStyleBackColor = true;
            this.Pause.Click += new System.EventHandler(this.Pause_Click);
            // 
            // Restart
            // 
            this.Restart.Image = ((System.Drawing.Image)(resources.GetObject("Restart.Image")));
            this.Restart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Restart.Location = new System.Drawing.Point(3, 61);
            this.Restart.Name = "Restart";
            this.Restart.Size = new System.Drawing.Size(100, 50);
            this.Restart.TabIndex = 2;
            this.Restart.Text = "Restart";
            this.Restart.UseVisualStyleBackColor = true;
            this.Restart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // PropertiesGroup
            // 
            this.PropertiesGroup.Controls.Add(this.IsStaticCheckBox);
            this.PropertiesGroup.Location = new System.Drawing.Point(109, 61);
            this.PropertiesGroup.Name = "PropertiesGroup";
            this.PropertiesGroup.Size = new System.Drawing.Size(100, 50);
            this.PropertiesGroup.TabIndex = 3;
            this.PropertiesGroup.TabStop = false;
            this.PropertiesGroup.Text = "Properties";
            // 
            // IsStaticCheckBox
            // 
            this.IsStaticCheckBox.AutoSize = true;
            this.IsStaticCheckBox.Location = new System.Drawing.Point(6, 19);
            this.IsStaticCheckBox.Name = "IsStaticCheckBox";
            this.IsStaticCheckBox.Size = new System.Drawing.Size(61, 17);
            this.IsStaticCheckBox.TabIndex = 0;
            this.IsStaticCheckBox.Text = "IsStatic";
            this.IsStaticCheckBox.UseVisualStyleBackColor = true;
            this.IsStaticCheckBox.CheckedChanged += new System.EventHandler(this.IsStaticCheckBox_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.96356F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.03644F));
            this.tableLayoutPanel1.Controls.Add(this.Play, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Pause, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Restart, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.PropertiesGroup, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.15385F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.84615F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(226, 126);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // FrameTrackBar
            // 
            this.FrameTrackBar.Location = new System.Drawing.Point(4, 45);
            this.FrameTrackBar.Name = "FrameTrackBar";
            this.FrameTrackBar.Size = new System.Drawing.Size(214, 45);
            this.FrameTrackBar.TabIndex = 5;
            this.FrameTrackBar.ValueChanged += new System.EventHandler(this.FrameTrackBar_ValueChanged);
            // 
            // FrameProgressBar
            // 
            this.FrameProgressBar.Location = new System.Drawing.Point(15, 64);
            this.FrameProgressBar.Name = "FrameProgressBar";
            this.FrameProgressBar.Size = new System.Drawing.Size(195, 23);
            this.FrameProgressBar.TabIndex = 7;
            // 
            // TimeBarBox
            // 
            this.TimeBarBox.Controls.Add(this.ActualFrame);
            this.TimeBarBox.Controls.Add(this.MaxFrameBox);
            this.TimeBarBox.Controls.Add(this.FrameProgressBar);
            this.TimeBarBox.Controls.Add(this.FrameTrackBar);
            this.TimeBarBox.Location = new System.Drawing.Point(6, 153);
            this.TimeBarBox.Name = "TimeBarBox";
            this.TimeBarBox.Size = new System.Drawing.Size(223, 123);
            this.TimeBarBox.TabIndex = 8;
            this.TimeBarBox.TabStop = false;
            this.TimeBarBox.Text = "Timebar";
            // 
            // ActualFrame
            // 
            this.ActualFrame.AutoSize = true;
            this.ActualFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActualFrame.Location = new System.Drawing.Point(103, 21);
            this.ActualFrame.Name = "ActualFrame";
            this.ActualFrame.Size = new System.Drawing.Size(60, 16);
            this.ActualFrame.TabIndex = 8;
            this.ActualFrame.Text = "Frame  0";
            // 
            // MaxFrameBox
            // 
            this.MaxFrameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaxFrameBox.Location = new System.Drawing.Point(8, 19);
            this.MaxFrameBox.Name = "MaxFrameBox";
            this.MaxFrameBox.Size = new System.Drawing.Size(70, 22);
            this.MaxFrameBox.TabIndex = 6;
            // 
            // CommandsBox
            // 
            this.CommandsBox.Controls.Add(this.JCompoundButton);
            this.CommandsBox.Controls.Add(this.JCylinderButton);
            this.CommandsBox.Controls.Add(this.JShpereButton);
            this.CommandsBox.Controls.Add(this.JBoxButton);
            this.CommandsBox.Controls.Add(this.RigidBodyButton);
            this.CommandsBox.Location = new System.Drawing.Point(6, 290);
            this.CommandsBox.Name = "CommandsBox";
            this.CommandsBox.Size = new System.Drawing.Size(243, 168);
            this.CommandsBox.TabIndex = 9;
            this.CommandsBox.TabStop = false;
            this.CommandsBox.Text = "Commands";
            // 
            // JCompoundButton
            // 
            this.JCompoundButton.Image = ((System.Drawing.Image)(resources.GetObject("JCompoundButton.Image")));
            this.JCompoundButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.JCompoundButton.Location = new System.Drawing.Point(123, 117);
            this.JCompoundButton.Name = "JCompoundButton";
            this.JCompoundButton.Size = new System.Drawing.Size(110, 40);
            this.JCompoundButton.TabIndex = 4;
            this.JCompoundButton.Text = "JCompound";
            this.JCompoundButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.JCompoundButton.UseVisualStyleBackColor = true;
            this.JCompoundButton.Click += new System.EventHandler(this.JCompound_Click);
            // 
            // JCylinderButton
            // 
            this.JCylinderButton.Image = ((System.Drawing.Image)(resources.GetObject("JCylinderButton.Image")));
            this.JCylinderButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.JCylinderButton.Location = new System.Drawing.Point(123, 65);
            this.JCylinderButton.Name = "JCylinderButton";
            this.JCylinderButton.Size = new System.Drawing.Size(110, 40);
            this.JCylinderButton.TabIndex = 3;
            this.JCylinderButton.Text = "JCylinder";
            this.JCylinderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.JCylinderButton.UseVisualStyleBackColor = true;
            this.JCylinderButton.Click += new System.EventHandler(this.JCylinderButton_Click);
            // 
            // JShpereButton
            // 
            this.JShpereButton.Image = ((System.Drawing.Image)(resources.GetObject("JShpereButton.Image")));
            this.JShpereButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.JShpereButton.Location = new System.Drawing.Point(10, 117);
            this.JShpereButton.Name = "JShpereButton";
            this.JShpereButton.Size = new System.Drawing.Size(110, 40);
            this.JShpereButton.TabIndex = 2;
            this.JShpereButton.Text = "JSphere";
            this.JShpereButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.JShpereButton.UseVisualStyleBackColor = true;
            this.JShpereButton.Click += new System.EventHandler(this.JShpereButton_Click);
            // 
            // JBoxButton
            // 
            this.JBoxButton.Image = ((System.Drawing.Image)(resources.GetObject("JBoxButton.Image")));
            this.JBoxButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.JBoxButton.Location = new System.Drawing.Point(10, 65);
            this.JBoxButton.Name = "JBoxButton";
            this.JBoxButton.Size = new System.Drawing.Size(110, 40);
            this.JBoxButton.TabIndex = 1;
            this.JBoxButton.Text = "Jbox";
            this.JBoxButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.JBoxButton.UseVisualStyleBackColor = true;
            this.JBoxButton.Click += new System.EventHandler(this.JBoxButton_Click);
            // 
            // RigidBodyButton
            // 
            this.RigidBodyButton.Location = new System.Drawing.Point(65, 19);
            this.RigidBodyButton.Name = "RigidBodyButton";
            this.RigidBodyButton.Size = new System.Drawing.Size(110, 40);
            this.RigidBodyButton.TabIndex = 0;
            this.RigidBodyButton.Text = "RigidBody";
            this.RigidBodyButton.UseVisualStyleBackColor = true;
            this.RigidBodyButton.Click += new System.EventHandler(this.RigidBodyButton_Click);
            // 
            // TimePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CommandsBox);
            this.Controls.Add(this.TimeBarBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TimePanel";
            this.Size = new System.Drawing.Size(259, 468);
            this.Load += new System.EventHandler(this.TimePanel_Load);
            this.SizeChanged += new System.EventHandler(this.TimePanel_SizeChanged);
            this.PropertiesGroup.ResumeLayout(false);
            this.PropertiesGroup.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FrameTrackBar)).EndInit();
            this.TimeBarBox.ResumeLayout(false);
            this.TimeBarBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxFrameBox)).EndInit();
            this.CommandsBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Pause;
        private System.Windows.Forms.Button Restart;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.GroupBox PropertiesGroup;
        private System.Windows.Forms.CheckBox IsStaticCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TrackBar FrameTrackBar;
        private System.Windows.Forms.ProgressBar FrameProgressBar;
        private System.Windows.Forms.GroupBox TimeBarBox;
        private System.Windows.Forms.NumericUpDown MaxFrameBox;
        private System.Windows.Forms.Label ActualFrame;
        private System.Windows.Forms.GroupBox CommandsBox;
        private System.Windows.Forms.Button JCylinderButton;
        private System.Windows.Forms.Button JShpereButton;
        private System.Windows.Forms.Button JBoxButton;
        private System.Windows.Forms.Button RigidBodyButton;
        private System.Windows.Forms.Button JCompoundButton;
    }
}
