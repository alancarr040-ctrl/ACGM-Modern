using ACGM.ModernClient.Configuration;

namespace ACGM.ModernClient.Forms;

public sealed class PlayerLoginForm : Form
{
    private const int PasswordMin = 6;
    private const int PasswordMax = 10;

    private readonly AcgmSettings _settings;
    private readonly ComboBox _serverCombo = new() { DropDownStyle = ComboBoxStyle.DropDown, Width = 445 };
    private readonly ComboBox _characterCombo = new() { DropDownStyle = ComboBoxStyle.DropDown, Width = 326 };
    private readonly TextBox _passwordText = new() { Width = 326, UseSystemPasswordChar = true, MaxLength = PasswordMax };
    private readonly Button _okButton = new() { Text = "OK", DialogResult = DialogResult.None, Width = 153, Height = 28 };
    private readonly Button _cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 153, Height = 28 };

    public string ServerAddress { get; private set; } = "";
    public string CharacterName { get; private set; } = "";
    public string Password { get; private set; } = "";
    public Uri ServerEndpoint => AcgmSettings.NormalizeServerForConnection(ServerAddress);

    public PlayerLoginForm(AcgmSettings settings)
    {
        _settings = settings;

        Text = "Player Login";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(375, 286);
        AcceptButton = _okButton;
        CancelButton = _cancelButton;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        BuildUi();
        LoadSettingsIntoControls();

        _serverCombo.SelectedIndexChanged += (_, _) => PopulateCharactersForSelectedServer();
        _serverCombo.TextChanged += (_, _) => PopulateCharactersForSelectedServer(false);
        _okButton.Click += (_, _) => TryAccept();
    }

    private void BuildUi()
    {
        var serverFrame = new GroupBox
        {
            Text = "Server Address",
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(8, 8),
            Size = new Size(360, 100)
        };

        var serverLabel = new Label
        {
            Text = "Please enter the URL of the ACGM server you wish to access.",
            Font = new Font(Font, FontStyle.Regular),
            Location = new Point(40, 24),
            Size = new Size(300, 18)
        };

        _serverCombo.Font = new Font(Font, FontStyle.Regular);
        _serverCombo.Location = new Point(40, 56);
        _serverCombo.Size = new Size(300, 23);
        serverFrame.Controls.Add(serverLabel);
        serverFrame.Controls.Add(_serverCombo);

        var charFrame = new GroupBox
        {
            Text = "Character Setup",
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(8, 124),
            Size = new Size(360, 100)
        };

        var charLabel = new Label
        {
            Text = "Character Name:",
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(20, 29),
            Size = new Size(120, 18)
        };

        _characterCombo.Font = new Font(Font, FontStyle.Regular);
        _characterCombo.Location = new Point(140, 26);
        _characterCombo.Size = new Size(200, 23);

        var passwordLabel = new Label
        {
            Text = "Password:",
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(20, 69),
            Size = new Size(120, 18)
        };

        _passwordText.Font = new Font(Font, FontStyle.Regular);
        _passwordText.Location = new Point(140, 66);
        _passwordText.Size = new Size(200, 23);

        charFrame.Controls.Add(charLabel);
        charFrame.Controls.Add(_characterCombo);
        charFrame.Controls.Add(passwordLabel);
        charFrame.Controls.Add(_passwordText);

        _okButton.Location = new Point(32, 242);
        _okButton.Font = new Font(Font, FontStyle.Bold);
        _cancelButton.Location = new Point(193, 242);
        _cancelButton.Font = new Font(Font, FontStyle.Bold);

        Controls.Add(serverFrame);
        Controls.Add(charFrame);
        Controls.Add(_okButton);
        Controls.Add(_cancelButton);
    }

    private void LoadSettingsIntoControls()
    {
        _serverCombo.Items.Clear();
        foreach (var server in _settings.Servers.Keys.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
            _serverCombo.Items.Add(server);

        if (!string.IsNullOrWhiteSpace(_settings.LastServerUsed))
            _serverCombo.Text = _settings.LastServerUsed;
        else if (_serverCombo.Items.Count > 0)
            _serverCombo.SelectedIndex = 0;

        PopulateCharactersForSelectedServer();

        if (!string.IsNullOrWhiteSpace(_settings.LastPlayerUsed))
            _characterCombo.Text = _settings.LastPlayerUsed;
    }

    private void PopulateCharactersForSelectedServer(bool selectFirst = true)
    {
        var currentCharacter = _characterCombo.Text;
        _characterCombo.Items.Clear();

        var server = AcgmSettings.NormalizeServerForStorage(_serverCombo.Text);
        if (_settings.Servers.TryGetValue(server, out var players))
        {
            foreach (var player in players.OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                _characterCombo.Items.Add(player);
        }

        if (!string.IsNullOrWhiteSpace(currentCharacter))
            _characterCombo.Text = currentCharacter;
        else if (selectFirst && _characterCombo.Items.Count > 0)
            _characterCombo.SelectedIndex = 0;
    }

    private void TryAccept()
    {
        var server = _serverCombo.Text.Trim();
        var character = _characterCombo.Text.Trim();
        var password = _passwordText.Text;

        if (server.Length == 0)
        {
            MessageBox.Show(this, "Please type in the address of the ACGM server", "ACGM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _serverCombo.Focus();
            return;
        }

        if (character.Length == 0)
        {
            MessageBox.Show(this, "Please type in your character's name", "ACGM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _characterCombo.Focus();
            return;
        }

        if (password.Length == 0)
        {
            MessageBox.Show(this, "Please type in your password", "ACGM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordText.Focus();
            return;
        }

        if (password.Length < PasswordMin)
        {
            MessageBox.Show(this, $"Your password is too short!  It must be at least {PasswordMin} characters long.", "ACGM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordText.Focus();
            return;
        }

        if (password.Length > PasswordMax)
        {
            MessageBox.Show(this, $"Your password is too long!  It can be at most {PasswordMax} characters long.", "ACGM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordText.Focus();
            return;
        }

        ServerAddress = AcgmSettings.NormalizeServerForStorage(server);
        CharacterName = character;
        Password = password;

        _settings.LastServerUsed = ServerAddress;
        _settings.LastPlayerUsed = CharacterName;
        _settings.AddPlayer(ServerAddress, CharacterName);
        _settings.Save();

        DialogResult = DialogResult.OK;
        Close();
    }
}
