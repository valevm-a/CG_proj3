namespace CG_proj3
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            drawToolStripMenuItem = new ToolStripMenuItem();
            lineToolStripMenuItem = new ToolStripMenuItem();
            circleToolStripMenuItem = new ToolStripMenuItem();
            polygonToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            deleteShapeToolStripMenuItem = new ToolStripMenuItem();
            clearAllToolStripMenuItem = new ToolStripMenuItem();
            changeColorToolStripMenuItem = new ToolStripMenuItem();
            changeThicknessToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            antiAliasingToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            label1 = new Label();
            numericUpDown1 = new NumericUpDown();
            pictureBox1 = new PictureBox();
            colorDialog1 = new ColorDialog();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, drawToolStripMenuItem, editToolStripMenuItem, optionsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveAsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(185, 34);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(185, 34);
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // drawToolStripMenuItem
            // 
            drawToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lineToolStripMenuItem, circleToolStripMenuItem, polygonToolStripMenuItem });
            drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            drawToolStripMenuItem.Size = new Size(69, 29);
            drawToolStripMenuItem.Text = "Draw";
            // 
            // lineToolStripMenuItem
            // 
            lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            lineToolStripMenuItem.Size = new Size(270, 34);
            lineToolStripMenuItem.Text = "Line";
            lineToolStripMenuItem.Click += lineToolStripMenuItem_Click;
            // 
            // circleToolStripMenuItem
            // 
            circleToolStripMenuItem.Name = "circleToolStripMenuItem";
            circleToolStripMenuItem.Size = new Size(270, 34);
            circleToolStripMenuItem.Text = "Circle";
            circleToolStripMenuItem.Click += circleToolStripMenuItem_Click;
            // 
            // polygonToolStripMenuItem
            // 
            polygonToolStripMenuItem.Name = "polygonToolStripMenuItem";
            polygonToolStripMenuItem.Size = new Size(270, 34);
            polygonToolStripMenuItem.Text = "Polygon";
            polygonToolStripMenuItem.Click += polygonToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { deleteShapeToolStripMenuItem, clearAllToolStripMenuItem, changeColorToolStripMenuItem, changeThicknessToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // deleteShapeToolStripMenuItem
            // 
            deleteShapeToolStripMenuItem.Name = "deleteShapeToolStripMenuItem";
            deleteShapeToolStripMenuItem.Size = new Size(254, 34);
            deleteShapeToolStripMenuItem.Text = "Delete Shape";
            deleteShapeToolStripMenuItem.Click += deleteShapeToolStripMenuItem_Click;
            // 
            // clearAllToolStripMenuItem
            // 
            clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            clearAllToolStripMenuItem.Size = new Size(254, 34);
            clearAllToolStripMenuItem.Text = "Clear All";
            clearAllToolStripMenuItem.Click += clearAllToolStripMenuItem_Click;
            // 
            // changeColorToolStripMenuItem
            // 
            changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
            changeColorToolStripMenuItem.Size = new Size(254, 34);
            changeColorToolStripMenuItem.Text = "Change Color";
            changeColorToolStripMenuItem.Click += changeColorToolStripMenuItem_Click;
            // 
            // changeThicknessToolStripMenuItem
            // 
            changeThicknessToolStripMenuItem.Name = "changeThicknessToolStripMenuItem";
            changeThicknessToolStripMenuItem.Size = new Size(254, 34);
            changeThicknessToolStripMenuItem.Text = "Change Thickness";
            changeThicknessToolStripMenuItem.Click += changeThicknessToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { antiAliasingToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(92, 29);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // antiAliasingToolStripMenuItem
            // 
            antiAliasingToolStripMenuItem.Name = "antiAliasingToolStripMenuItem";
            antiAliasingToolStripMenuItem.Size = new Size(215, 34);
            antiAliasingToolStripMenuItem.Text = "Anti-Aliasing";
            antiAliasingToolStripMenuItem.Click += antiAliasingToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.SlateGray;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(numericUpDown1);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 33);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(5);
            panel1.Size = new Size(800, 426);
            panel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.Window;
            label1.Location = new Point(8, 389);
            label1.Name = "label1";
            label1.Size = new Size(94, 25);
            label1.TabIndex = 2;
            label1.Text = "Brush size:";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(108, 387);
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(66, 31);
            numericUpDown1.TabIndex = 1;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Location = new Point(5, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(790, 377);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 459);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximumSize = new Size(822, 515);
            MinimumSize = new Size(822, 515);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private Panel panel1;
        private PictureBox pictureBox1;
        private ToolStripMenuItem drawToolStripMenuItem;
        private ToolStripMenuItem lineToolStripMenuItem;
        private ToolStripMenuItem circleToolStripMenuItem;
        private ToolStripMenuItem polygonToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteShapeToolStripMenuItem;
        private ToolStripMenuItem clearAllToolStripMenuItem;
        private ToolStripMenuItem changeColorToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem antiAliasingToolStripMenuItem;
        private NumericUpDown numericUpDown1;
        private ToolStripMenuItem changeThicknessToolStripMenuItem;
        private ColorDialog colorDialog1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private Label label1;
    }
}
