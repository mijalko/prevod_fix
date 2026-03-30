namespace Prevod;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // --- Controls ---
        lblInputFile   = new Label();
        txtInputFile   = new TextBox();
        btnBrowse      = new Button();
        lblOffset      = new Label();
        nudOffset      = new NumericUpDown();
        lblSeconds     = new Label();
        btnProcess     = new Button();
        lblStatus      = new Label();
        lblLog         = new Label();
        rtbLog         = new RichTextBox();

        ((System.ComponentModel.ISupportInitialize)nudOffset).BeginInit();
        SuspendLayout();

        // --- Form ---
        this.Text          = "Subtitle Fixer";
        this.ClientSize    = new Size(620, 460);
        string iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
        if (File.Exists(iconPath))
            this.Icon = new Icon(iconPath);
        this.MinimumSize   = new Size(636, 498);
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.StartPosition = FormStartPosition.CenterScreen;

        // --- lblInputFile ---
        lblInputFile.Text      = "Input file:";
        lblInputFile.Location  = new Point(12, 18);
        lblInputFile.Size      = new Size(70, 23);
        lblInputFile.TextAlign = ContentAlignment.MiddleLeft;

        // --- txtInputFile ---
        txtInputFile.Location  = new Point(88, 16);
        txtInputFile.Size      = new Size(400, 23);
        txtInputFile.ReadOnly  = true;
        txtInputFile.TabStop   = false;

        // --- btnBrowse ---
        btnBrowse.Text     = "Browse...";
        btnBrowse.Location = new Point(496, 15);
        btnBrowse.Size     = new Size(108, 26);
        btnBrowse.Click   += btnBrowse_Click;

        // --- lblOffset ---
        lblOffset.Text      = "Time correction:";
        lblOffset.Location  = new Point(12, 60);
        lblOffset.Size      = new Size(105, 23);
        lblOffset.TextAlign = ContentAlignment.MiddleLeft;

        // --- nudOffset ---
        nudOffset.Location      = new Point(124, 58);
        nudOffset.Size          = new Size(100, 23);
        nudOffset.DecimalPlaces = 3;
        nudOffset.Increment     = 0.5m;
        nudOffset.Minimum       = -9999;
        nudOffset.Maximum       = 9999;
        nudOffset.Value         = 0;

        // --- lblSeconds ---
        lblSeconds.Text      = "seconds  (negative = earlier, positive = later)";
        lblSeconds.Location  = new Point(232, 60);
        lblSeconds.Size      = new Size(360, 23);
        lblSeconds.TextAlign = ContentAlignment.MiddleLeft;
        lblSeconds.ForeColor = Color.DimGray;

        // --- btnProcess ---
        btnProcess.Text     = "Fix && Save...";
        btnProcess.Location = new Point(12, 100);
        btnProcess.Size     = new Size(592, 34);
        btnProcess.Font     = new Font(btnProcess.Font, FontStyle.Bold);
        btnProcess.Click   += btnProcess_Click;

        // --- lblStatus ---
        lblStatus.Text      = "Ready.";
        lblStatus.Location  = new Point(12, 144);
        lblStatus.Size      = new Size(592, 20);
        lblStatus.ForeColor = Color.DimGray;

        // --- lblLog ---
        lblLog.Text      = "Processing log:";
        lblLog.Location  = new Point(12, 170);
        lblLog.Size      = new Size(150, 20);

        // --- rtbLog ---
        rtbLog.Location  = new Point(12, 192);
        rtbLog.Size      = new Size(592, 252);
        rtbLog.ReadOnly  = true;
        rtbLog.BackColor = SystemColors.Window;
        rtbLog.Font      = new Font("Consolas", 9f);
        rtbLog.Anchor    = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        // --- Add controls ---
        this.Controls.AddRange(new Control[]
        {
            lblInputFile, txtInputFile, btnBrowse,
            lblOffset, nudOffset, lblSeconds,
            btnProcess,
            lblStatus,
            lblLog, rtbLog
        });

        ((System.ComponentModel.ISupportInitialize)nudOffset).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label           lblInputFile;
    private TextBox         txtInputFile;
    private Button          btnBrowse;
    private Label           lblOffset;
    private NumericUpDown   nudOffset;
    private Label           lblSeconds;
    private Button          btnProcess;
    private Label           lblStatus;
    private Label           lblLog;
    private RichTextBox     rtbLog;
}
