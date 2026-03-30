using System.Text;
using PrevodCore;

namespace Prevod;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title  = "Select subtitle file",
            Filter = "Subtitle files (*.srt;*.sub)|*.srt;*.sub|All files (*.*)|*.*"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            txtInputFile.Text  = dlg.FileName;
            lblStatus.Text     = "File selected.";
            lblStatus.ForeColor = Color.Black;
        }
    }

    private void btnProcess_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtInputFile.Text) || !File.Exists(txtInputFile.Text))
        {
            lblStatus.Text      = "Please select a valid input file.";
            lblStatus.ForeColor = Color.Red;
            return;
        }

        double offsetSeconds = (double)nudOffset.Value;
        string inputPath     = txtInputFile.Text;
        string ext           = Path.GetExtension(inputPath);
        string suggestedName = Path.GetFileNameWithoutExtension(inputPath) + "_fixed" + ext;
        string suggestedDir  = Path.GetDirectoryName(inputPath) ?? "";

        using var dlg = new SaveFileDialog
        {
            Title            = "Save fixed subtitle file",
            Filter           = "Subtitle files (*.srt;*.sub)|*.srt;*.sub|All files (*.*)|*.*",
            FileName         = suggestedName,
            InitialDirectory = suggestedDir
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            string rawText       = File.ReadAllText(inputPath, SubtitleProcessor.DetectEncoding(inputPath));
            var (fixedText, report) = SubtitleProcessor.Process(rawText, offsetSeconds);

            File.WriteAllText(dlg.FileName, fixedText, new UTF8Encoding(true));

            rtbLog.Text         = report;
            lblStatus.Text      = "Done! File saved.";
            lblStatus.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            lblStatus.Text      = "Error: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
            rtbLog.Text         = ex.ToString();
        }
    }
}
